﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DemoGame.Extensions;
using NetGore.Extensions;

namespace DemoGame.Server
{
    /// <summary>
    /// A thread-safe object that is used to get and track free guids. It makes use of the database
    /// to find the free guids when needed.
    /// </summary>
    public abstract class GuidCreatorBase
    {
        readonly string _column;
        readonly DbConnection _conn;

        readonly int _criticalSize;
        // FUTURE: Use the CriticalSize, which will automatically get the next free values asynchronously in the background

        readonly Stack<int> _freeIndices;

        readonly object _stackLock = new object();

        readonly string _table;

        bool _isRefilling;

        /// <summary>
        /// Gets the table column that is being used to track free guids.
        /// </summary>
        public string Column
        {
            get { return _column; }
        }

        /// <summary>
        /// Gets the DbConnection used for this GuidCreatorBase.
        /// </summary>
        public DbConnection DbConnection
        {
            get { return _conn; }
        }

        /// <summary>
        /// Gets the table that is being used to track free guids.
        /// </summary>
        public string Table
        {
            get { return _table; }
        }

        /// <summary>
        /// GuidCreatorBase constructor.
        /// </summary>
        /// <param name="conn">DbConnection to use to connect to the database.</param>
        /// <param name="table">Table containing the column to track the values in.</param>
        /// <param name="column">Column containing the guids to track.</param>
        /// <param name="stackSize">Maximum size of the free guid stack.</param>
        /// <param name="criticalSize">When there is less than this many guids available, the free guid
        /// stack will be replenished. If this is non-zero, the free guid stack will attempt to replenish
        /// asynchronously. If this is zero, the stack will only replenish on when it is empty.</param>
        protected GuidCreatorBase(DbConnection conn, string table, string column, int stackSize, int criticalSize)
        {
            if (conn == null)
                throw new ArgumentNullException("conn");
            if (stackSize < 1)
                throw new ArgumentOutOfRangeException("stackSize", "stackSize must be >= 1.");
            if (criticalSize < 0)
                throw new ArgumentOutOfRangeException("criticalSize", "stackSize must be >= 0.");

            _conn = conn;
            _table = table;
            _column = column;
            _freeIndices = new Stack<int>(stackSize);
            _criticalSize = criticalSize;

            // Perform the initial fill
            BeginRefill();
        }

        void BeginRefill()
        {
            lock (_stackLock)
            {
                // Make sure we are not already refilling
                if (_isRefilling)
                    return;

                _isRefilling = true;

                // Start the refill thread
                Thread refillThread = new Thread(Refill);
                refillThread.Start();
            }
        }

        /// <summary>
        /// Checks if a database field contains any of the specified values.
        /// </summary>
        /// <typeparam name="T">Type of value to enumerate through.</typeparam>
        /// <param name="conn">DbConnection to use the query on.</param>
        /// <param name="table">Table containing the field to check..</param>
        /// <param name="column">Field to check.</param>
        /// <param name="values">IEnumerable of values to check if in the column.</param>
        /// <returns>True if any of the <paramref name="values"/> are in the <paramref name="column"/>, else false.</returns>
        static bool FieldContainsValues<T>(DbConnection conn, string table, string column, IEnumerable<T> values)
        {
            string valuesStr = values.Implode(',');

            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = string.Format("SELECT `{0}` FROM `{1}` WHERE `{0}` IN ({2})", column, table, valuesStr);
                using (DbDataReader r = cmd.ExecuteReader())
                {
                    return r.Read();
                }
            }
        }

        /// <summary>
        /// Returns a guid to the collection to be reused. Only call this if you are positive the guid is free.
        /// Not every guid has to be freed, and it is assumed at least some will be lost.
        /// </summary>
        /// <param name="guid">Guid value to free.</param>
        public virtual void FreeGuid(int guid)
        {
            lock (_stackLock)
            {
                _freeIndices.Push(guid);
            }
        }

        /// <summary>
        /// Finds free indices for a given field from the database.
        /// </summary>
        /// <param name="conn">DbConnection to use the query on.</param>
        /// <param name="table">Table to find the free indicies in.</param>
        /// <param name="column">Column to find the free indices for. This field must contain a numeric
        /// type that can be read as an Int32.</param>
        /// <param name="amount">Total number of values to find from the database.</param>
        /// <returns>A collection with a total of <paramref name="amount"/> Int32s, where each value is
        /// a free index on the <paramref name="column"/> in <paramref name="table"/>. This
        /// collection is sorted ascending by nature.</returns>
        protected static List<int> GetFreeFromDB(DbConnection conn, string table, string column, int amount)
        {
            // This is just used as a wrapper to easily check and validate the values returned
            // when the DEBUG flag is defined

            var ret = InternalGetFreeFromDB(conn, table, column, amount);

            Debug.Assert(ret.Count == amount, "The return count should always equal amount exactly.");
            Debug.Assert(!FieldContainsValues(conn, table, column, ret), "Used indices found in return collection.");

            return ret;
        }

        /// <summary>
        /// Gets the next free guid.
        /// </summary>
        /// <returns>The next free guid.</returns>
        public virtual int GetNext()
        {
            // Just keep looping until we return something
            while (true)
            {
                lock (_stackLock)
                {
                    // Return only if we have something available
                    if (_freeIndices.Count > 0)
                        return _freeIndices.Pop();
                }

                // Nothing was available, so we ensure we're in the process of refilling and keep trying
                BeginRefill();
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Finds free indices for a given field from the database.
        /// </summary>
        /// <param name="conn">DbConnection to use the query on.</param>
        /// <param name="table">Table to find the free indicies in.</param>
        /// <param name="column">Column to find the free indices for. This field must contain a numeric
        /// type that can be read as an Int32.</param>
        /// <param name="amount">Total number of values to find from the database.</param>
        /// <returns>A collection with a total of <paramref name="amount"/> Int32s, where each value is
        /// a free index on the <paramref name="column"/> in <paramref name="table"/>. This
        /// collection is sorted ascending by nature.</returns>
        static List<int> InternalGetFreeFromDB(DbConnection conn, string table, string column, int amount)
        {
            if (conn == null)
                throw new ArgumentNullException("conn");

            if (string.IsNullOrEmpty(table))
                throw new ArgumentNullException("table");

            if (string.IsNullOrEmpty(column))
                throw new ArgumentNullException("column");

            if (amount < 1)
                throw new ArgumentOutOfRangeException("amount", "The amount must be greater than or equal to 1.");

            var returnValues = new List<int>(amount);
            int lastValue = -1;

            // Create the command for the database
            using (DbCommand cmd = conn.CreateCommand())
            {
                // Grab every row for the field, sorted ascending
                cmd.CommandText = string.Format("SELECT `{0}` FROM `{1}` ORDER BY `{0}` ASC", column, table);

                using (DbDataReader r = cmd.ExecuteReader())
                {
                    // Read until we run out of rows
                    while (r.Read())
                    {
                        // Get the value of the index from the row
                        int value = r.GetInt32(0);

                        // If the value is greater than the last value used + 1, this means that there
                        // was a gap of 2 or more numbers between the last value and the current value.
                        // Since the records are sorted, we know that this means all the values in that
                        // gap are free, so add each one to our return collection.
                        if (value > lastValue + 1)
                        {
                            // Add all the values in the gap to the return collection. We must also make
                            // sure we don't add more than we need, so if the gap contains more values than
                            // we need, only grab the amount we need. This is just an alternative to adding
                            // a "returnValues.Count < amount" check on each loop iteration.
                            int needed = amount - returnValues.Count;
                            int gapSize = value - (lastValue + 1);
                            int loopEnd;

                            if (needed > gapSize)
                                loopEnd = value;
                            else
                                loopEnd = lastValue + 1 + needed;

                            for (int i = lastValue + 1; i < loopEnd; i++)
                            {
                                returnValues.Add(i);
                            }

                            // Because we know that we will only add to the return collection at this point
                            // in this loop, we can safely check only here if we have hit the desired
                            // number of return values. If we have, return.
                            if (returnValues.Count >= amount)
                                return returnValues;
                        }

                        // Update the last value to the current value
                        lastValue = value;
                    }
                }
            }

            // If we made it this far, that means we read through every record from the database. At this
            // point, we can conclude the highest index used is equal to lastValue, so we are safe to just
            // keep adding every value greater than lastValue until we have enough values.
            int valuesRemaining = amount - returnValues.Count;
            for (int i = 0; i < valuesRemaining; i++)
            {
                returnValues.Add(++lastValue);
            }

            return returnValues;
        }

        /// <summary>
        /// A blocking, thread-safe method that will refill the free guid stack.
        /// </summary>
        void Refill()
        {
            // Get the free values from the database
            int amount = _criticalSize - _freeIndices.Count;
            var freeValues = GetFreeFromDB(DbConnection, Table, Column, amount);

            // Reverse the values so we end up using the lowest values first
            freeValues.Reverse();

            // Lock the stack and add all the new values
            lock (_stackLock)
            {
                foreach (int value in freeValues)
                {
                    _freeIndices.Push(value);
                }
            }

            // Done refilling
            _isRefilling = false;
        }
    }
}