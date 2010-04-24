using System;
using System.Data;
using System.Linq;

namespace NetGore.Db
{
    public class DataReaderContainer : IDataReader
    {
        readonly IDbCommand _command;
        readonly IDataReader _dataReader;
        bool _isDisposed = false;

        internal DataReaderContainer(IDbCommand command, IDataReader dataReader)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");

            _command = command;
            _dataReader = dataReader;
        }

        protected IDbCommand Command
        {
            get { return _command; }
        }

        protected IDataReader DataReader
        {
            get { return _dataReader; }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposeManaged"><c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                _dataReader.Dispose();
                _command.Dispose();
            }
        }

        #region IDataReader Members

        ///<summary>
        ///
        ///                    Gets the column located at the specified index.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The column located at the specified index as an <see cref="T:System.Object" />.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based index of the column to get. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public object this[int i]
        {
            get { return _dataReader[i]; }
        }

        ///<summary>
        ///
        ///                    Gets the column with the specified name.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The column with the specified name as an <see cref="T:System.Object" />.
        ///                
        ///</returns>
        ///
        ///<param name="name">
        ///                    The name of the column to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    No column with the specified name was found. 
        ///                </exception>
        public object this[string name]
        {
            get { return _dataReader[name]; }
        }

        ///<summary>
        ///
        ///                    Gets a value indicating the depth of nesting for the current row.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The level of nesting.
        ///                
        ///</returns>
        ///
        public int Depth
        {
            get { return _dataReader.Depth; }
        }

        ///<summary>
        ///
        ///                    Gets the number of columns in the current row.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.
        ///                
        ///</returns>
        ///
        public int FieldCount
        {
            get { return _dataReader.FieldCount; }
        }

        ///<summary>
        ///
        ///                    Gets a value indicating whether the data reader is closed.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///true if the data reader is closed; otherwise, false.
        ///                
        ///</returns>
        ///
        public bool IsClosed
        {
            get { return _dataReader.IsClosed; }
        }

        ///<summary>
        ///
        ///                    Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.
        ///                
        ///</returns>
        ///
        public int RecordsAffected
        {
            get { return _dataReader.RecordsAffected; }
        }

        ///<summary>
        ///
        ///                    Closes the <see cref="T:System.Data.IDataReader" /> Object.
        ///                
        ///</summary>
        ///
        public void Close()
        {
            _dataReader.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.             
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            Dispose(true);
        }

        ///<summary>
        ///
        ///                    Gets the value of the specified column as a Boolean.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The value of the column.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based column ordinal. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public bool GetBoolean(int i)
        {
            return _dataReader.GetBoolean(i);
        }

        ///<summary>
        ///
        ///                    Gets the 8-bit unsigned integer value of the specified column.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The 8-bit unsigned integer value of the specified column.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based column ordinal. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public byte GetByte(int i)
        {
            return _dataReader.GetByte(i);
        }

        ///<summary>
        ///
        ///                    Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The actual number of bytes read.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based column ordinal. 
        ///                </param>
        ///<param name="fieldOffset">
        ///                    The index within the field from which to start the read operation. 
        ///                </param>
        ///<param name="buffer">
        ///                    The buffer into which to read the stream of bytes. 
        ///                </param>
        ///<param name="bufferoffset">
        ///                    The index for <paramref name="buffer" /> to start the read operation. 
        ///                </param>
        ///<param name="length">
        ///                    The number of bytes to read. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        ///<summary>
        ///
        ///                    Gets the character value of the specified column.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The character value of the specified column.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based column ordinal. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public char GetChar(int i)
        {
            return _dataReader.GetChar(i);
        }

        ///<summary>
        ///
        ///                    Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The actual number of characters read.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The zero-based column ordinal. 
        ///                </param>
        ///<param name="fieldoffset">
        ///                    The index within the row from which to start the read operation. 
        ///                </param>
        ///<param name="buffer">
        ///                    The buffer into which to read the stream of bytes. 
        ///                </param>
        ///<param name="bufferoffset">
        ///                    The index for <paramref name="buffer" /> to start the read operation. 
        ///                </param>
        ///<param name="length">
        ///                    The number of bytes to read. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        ///<summary>
        ///
        ///                    Returns an <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    An <see cref="T:System.Data.IDataReader" />.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public IDataReader GetData(int i)
        {
            return _dataReader.GetData(i);
        }

        ///<summary>
        ///
        ///                    Gets the data type information for the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The data type information for the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public string GetDataTypeName(int i)
        {
            return _dataReader.GetDataTypeName(i);
        }

        ///<summary>
        ///
        ///                    Gets the date and time data value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The date and time data value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public DateTime GetDateTime(int i)
        {
            return _dataReader.GetDateTime(i);
        }

        ///<summary>
        ///
        ///                    Gets the fixed-position numeric value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The fixed-position numeric value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public decimal GetDecimal(int i)
        {
            return _dataReader.GetDecimal(i);
        }

        ///<summary>
        ///
        ///                    Gets the double-precision floating point number of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The double-precision floating point number of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public double GetDouble(int i)
        {
            return _dataReader.GetDouble(i);
        }

        ///<summary>
        ///
        ///                    Gets the <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public Type GetFieldType(int i)
        {
            return _dataReader.GetFieldType(i);
        }

        ///<summary>
        ///
        ///                    Gets the single-precision floating point number of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The single-precision floating point number of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public float GetFloat(int i)
        {
            return _dataReader.GetFloat(i);
        }

        ///<summary>
        ///
        ///                    Returns the GUID value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The GUID value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public Guid GetGuid(int i)
        {
            return _dataReader.GetGuid(i);
        }

        ///<summary>
        ///
        ///                    Gets the 16-bit signed integer value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The 16-bit signed integer value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public short GetInt16(int i)
        {
            return _dataReader.GetInt16(i);
        }

        ///<summary>
        ///
        ///                    Gets the 32-bit signed integer value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The 32-bit signed integer value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public int GetInt32(int i)
        {
            return _dataReader.GetInt32(i);
        }

        ///<summary>
        ///
        ///                    Gets the 64-bit signed integer value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The 64-bit signed integer value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public long GetInt64(int i)
        {
            return _dataReader.GetInt64(i);
        }

        ///<summary>
        ///
        ///                    Gets the name for the field to find.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The name of the field or the empty string (""), if there is no value to return.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public string GetName(int i)
        {
            return _dataReader.GetName(i);
        }

        ///<summary>
        ///
        ///                    Return the index of the named field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The index of the named field.
        ///                
        ///</returns>
        ///
        ///<param name="name">
        ///                    The name of the field to find. 
        ///                </param>
        public int GetOrdinal(string name)
        {
            return _dataReader.GetOrdinal(name);
        }

        ///<summary>
        ///
        ///                    Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        ///                
        ///</returns>
        ///
        ///<exception cref="T:System.InvalidOperationException">
        ///                    The <see cref="T:System.Data.IDataReader" /> is closed. 
        ///                </exception>
        public DataTable GetSchemaTable()
        {
            return _dataReader.GetSchemaTable();
        }

        ///<summary>
        ///
        ///                    Gets the string value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The string value of the specified field.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public string GetString(int i)
        {
            return _dataReader.GetString(i);
        }

        ///<summary>
        ///
        ///                    Return the value of the specified field.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The <see cref="T:System.Object" /> which will contain the field value upon return.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public object GetValue(int i)
        {
            return _dataReader.GetValue(i);
        }

        ///<summary>
        ///
        ///                    Gets all the attribute fields in the collection for the current record.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///
        ///                    The number of instances of <see cref="T:System.Object" /> in the array.
        ///                
        ///</returns>
        ///
        ///<param name="values">
        ///                    An array of <see cref="T:System.Object" /> to copy the attribute fields into. 
        ///                </param>
        public int GetValues(object[] values)
        {
            return _dataReader.GetValues(values);
        }

        ///<summary>
        ///
        ///                    Return whether the specified field is set to null.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///true if the specified field is set to null; otherwise, false.
        ///                
        ///</returns>
        ///
        ///<param name="i">
        ///                    The index of the field to find. 
        ///                </param>
        ///<exception cref="T:System.IndexOutOfRangeException">
        ///                    The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />. 
        ///                </exception>
        public bool IsDBNull(int i)
        {
            return _dataReader.IsDBNull(i);
        }

        ///<summary>
        ///
        ///                    Advances the data reader to the next result, when reading the results of batch SQL statements.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///true if there are more rows; otherwise, false.
        ///                
        ///</returns>
        ///
        public bool NextResult()
        {
            return _dataReader.NextResult();
        }

        ///<summary>
        ///
        ///                    Advances the <see cref="T:System.Data.IDataReader" /> to the next record.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///true if there are more rows; otherwise, false.
        ///                
        ///</returns>
        ///
        public bool Read()
        {
            return _dataReader.Read();
        }

        #endregion
    }
}