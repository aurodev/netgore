﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NetGore.Extensions
{
    /// <summary>
    /// Extensions for the IEnumerable class.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Combines all items in an IEnumerable together into a delimited string.
        /// </summary>
        /// <param name="source">A sequence that contains elements to be imploded.</param>
        /// <param name="delimiter">Character to use when combining the characters.</param>
        /// <returns>All items in an IEnumerable together into a delimited string.</returns>
        public static string Implode(this IEnumerable source, char delimiter)
        {
            StringBuilder sb = new StringBuilder(128);

            // Add all to the StringBuilder
            foreach (object item in source)
            {
                sb.Append(item);
                sb.Append(delimiter);
            }

            // Remove the last delimiter, or else our list will look like: a,b,c,d,f,
            sb.Remove(sb.Length - 1, 1);

            // Return the built string
            return sb.ToString();
        }

        /// <summary>
        /// Combines all items in an IEnumerable together into a delimited string.
        /// </summary>
        /// <param name="source">A sequence that contains elements to be imploded.</param>
        /// <param name="delimiter">Character to use when combining the characters.</param>
        /// <returns>All items in an IEnumerable together into a delimited string.</returns>
        public static string Implode(this IEnumerable source, string delimiter)
        {
            StringBuilder sb = new StringBuilder(128);

            // Add all to the StringBuilder
            foreach (object item in source)
            {
                sb.Append(item);
                sb.Append(delimiter);
            }

            // Remove the last delimiter, or else our list will look like: a,b,c,d,f,
            sb.Remove(sb.Length - delimiter.Length, delimiter.Length);

            // Return the built string
            return sb.ToString();
        }

        /// <summary>
        /// Combines all items in an IEnumerable together into a delimited string.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="source">A sequence that contains elements to be imploded.</param>
        /// <param name="delimiter">Character to use when combining the characters.</param>
        /// <returns>All items in an IEnumerable together into a delimited string.</returns>
        public static string Implode<T>(this IEnumerable<T> source, char delimiter)
        {
            // Allocate 16 characters for each value, plus room for the delimiter
            StringBuilder sb = new StringBuilder(source.Count() * (8 + 1));

            // Add all to the StringBuilder
            foreach (T item in source)
            {
                sb.Append(item);
                sb.Append(delimiter);
            }

            // Remove the last delimiter, or else our list will look like: a,b,c,d,f,
            sb.Remove(sb.Length - 1, 1);

            // Return the built string
            return sb.ToString();
        }

        /// <summary>
        /// Combines all items in an IEnumerable together into a delimited string.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="source">A sequence that contains elements to be imploded.</param>
        /// <param name="delimiter">Character to use when combining the characters.</param>
        /// <returns>All items in an IEnumerable together into a delimited string.</returns>
        public static string Implode<T>(this IEnumerable<T> source, string delimiter)
        {
            // Allocate 8 characters for each value, plus room for the delimiter
            StringBuilder sb = new StringBuilder(source.Count() * (8 + delimiter.Length));

            // Add all to the StringBuilder
            foreach (T item in source)
            {
                sb.Append(item);
                sb.Append(delimiter);
            }

            // Remove the last delimiter, or else our list will look like: a,b,c,d,f,
            sb.Remove(sb.Length - delimiter.Length, delimiter.Length);

            // Return the built string
            return sb.ToString();
        }
    }
}