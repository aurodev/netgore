﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using log4net;

namespace NetGore
{
    /// <summary>
    /// Extension methods for the <see cref="EventHandler"/>.
    /// </summary>
    public static class EventHandlerExtensions
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Safely raises an event, even if there are no listeners.
        /// </summary>
        /// <param name="handler">The <see cref="EventHandler"/> to raise the event on.</param>
        /// <param name="sender">The sender event parameter.</param>
        /// <param name="e">The <see cref="EventArgs"/> event parameter.</param>
        /// <param name="swallowExceptions">When true, any <see cref="Exception"/> thrown while invoking the
        /// <paramref name="handler"/> will be swallowed. This prevents event listeners from creating an <see cref="Exception"/>
        /// that bubbles up to the handler of the event. When false, events will be thrown like normal. By default, this is true
        /// because events typically do not care about the behavior of the listeners and do not handle <see cref="Exception"/>s
        /// created by their listeners. This is especially true for events that have a no return value and pass an immutable
        /// <see cref="EventArgs"/>.</param>
        /// <exception cref="Exception"><paramref name="swallowExceptions"/> is <c>false</c> and an <see cref="Exception"/> has occured
        /// either while trying to invoke the <paramref name="handler"/>, or inside the <paramref name="handler"/> itself.</exception>
        public static void Raise(this EventHandler handler, object sender, EventArgs e, bool swallowExceptions = true)
        {
            Debug.Assert(e != null, "A null EventArgs should never be used! Use EventArgs.Empty instead.");

            try
            {
                // Check for a valid handler
                if (handler == null)
                    return;

                // Raise event
                handler(sender, e);
            }
            catch (Exception ex)
            {
                // Log any exception that occured
                const string errmsg =
                    "Exception occured in event listener on event handler `{0}` (sender: {1}; EventArgs: {2}). Exception: {3}";
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, handler, sender, e, ex);
                Debug.Fail(string.Format(errmsg, handler, sender, e, ex));

                // If not swallowing exceptions, then throw so it can continue to bubble up
                if (!swallowExceptions)
                    throw;
            }
        }

        /// <summary>
        /// Safely raises an event, even if there are no listeners.
        /// </summary>
        /// <typeparam name="T">The type of the event data generated by the event.</typeparam>
        /// <param name="handler">The <see cref="EventHandler{T}"/> to raise the event on.</param>
        /// <param name="sender">The sender event parameter.</param>
        /// <param name="e">The <see cref="EventArgs"/> event parameter.</param>
        /// <param name="swallowExceptions">When true, any <see cref="Exception"/> thrown while invoking the
        /// <paramref name="handler"/> will be swallowed. This prevents event listeners from creating an <see cref="Exception"/>
        /// that bubbles up to the handler of the event. When false, events will be thrown like normal. By default, this is true
        /// because events typically do not care about the behavior of the listeners and do not handle <see cref="Exception"/>s
        /// created by their listeners. This is especially true for events that have a no return value and pass an immutable
        /// <see cref="EventArgs"/>.</param>
        /// <exception cref="Exception"><paramref name="swallowExceptions"/> is <c>false</c> and an <see cref="Exception"/> has occured
        /// either while trying to invoke the <paramref name="handler"/>, or inside the <paramref name="handler"/> itself.</exception>
        public static void Raise<T>(this EventHandler<T> handler, object sender, T e, bool swallowExceptions = true)
            where T : EventArgs
        {
            Debug.Assert(e != null, "A null EventArgs should never be used! Use EventArgs.Empty instead.");

            try
            {
                // Check for a valid handler
                if (handler == null)
                    return;

                // Raise event
                handler(sender, e);
            }
            catch (Exception ex)
            {
                // Log any exception that occured
                const string errmsg =
                    "Exception occured in event listener on event handler `{0}` (sender: {1}; EventArgs: {2}). Exception: {3}";
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, handler, sender, e, ex);
                Debug.Fail(string.Format(errmsg, handler, sender, e, ex));

                // If not swallowing exceptions, then throw so it can continue to bubble up
                if (!swallowExceptions)
                    throw;
            }
        }
    }

    /// <summary>
    /// Extension methods for the <see cref="TypedEventHandler{T}"/>.
    /// </summary>
    public static class TypedEventHandlerExtensions
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Safely raises an event, even if there are no listeners.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender of the event.</typeparam>
        /// <param name="handler">The <see cref="TypedEventHandler{T}"/> to raise the event on.</param>
        /// <param name="sender">The sender event parameter.</param>
        /// <param name="e">The <see cref="EventArgs"/> event parameter.</param>
        /// <param name="swallowExceptions">When true, any <see cref="Exception"/> thrown while invoking the
        /// <paramref name="handler"/> will be swallowed. This prevents event listeners from creating an <see cref="Exception"/>
        /// that bubbles up to the handler of the event. When false, events will be thrown like normal. By default, this is true
        /// because events typically do not care about the behavior of the listeners and do not handle <see cref="Exception"/>s
        /// created by their listeners. This is especially true for events that have a no return value and pass an immutable
        /// <see cref="EventArgs"/>.</param>
        /// <exception cref="Exception"><paramref name="swallowExceptions"/> is <c>false</c> and an <see cref="Exception"/> has occured
        /// either while trying to invoke the <paramref name="handler"/>, or inside the <paramref name="handler"/> itself.</exception>
        public static void Raise<TSender>(this TypedEventHandler<TSender> handler, TSender sender, EventArgs e,
                                          bool swallowExceptions = true)
        {
            Debug.Assert(e != null, "A null EventArgs should never be used! Use EventArgs.Empty instead.");

            try
            {
                // Check for a valid handler
                if (handler == null)
                    return;

                // Raise event
                handler(sender, e);
            }
            catch (Exception ex)
            {
                // Log any exception that occured
                const string errmsg =
                    "Exception occured in event listener on event handler `{0}` (sender: {1}; EventArgs: {2}). Exception: {3}";
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, handler, sender, e, ex);
                Debug.Fail(string.Format(errmsg, handler, sender, e, ex));

                // If not swallowing exceptions, then throw so it can continue to bubble up
                if (!swallowExceptions)
                    throw;
            }
        }

        /// <summary>
        /// Safely raises an event, even if there are no listeners.
        /// </summary>
        /// <typeparam name="TSender">The type of the sender of the event.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event data generated by the event.</typeparam>
        /// <param name="handler">The <see cref="TypedEventHandler{T,U}"/> to raise the event on.</param>
        /// <param name="sender">The sender event parameter.</param>
        /// <param name="e">The <see cref="EventArgs"/> event parameter.</param>
        /// <param name="swallowExceptions">When true, any <see cref="Exception"/> thrown while invoking the
        /// <paramref name="handler"/> will be swallowed. This prevents event listeners from creating an <see cref="Exception"/>
        /// that bubbles up to the handler of the event. When false, events will be thrown like normal. By default, this is true
        /// because events typically do not care about the behavior of the listeners and do not handle <see cref="Exception"/>s
        /// created by their listeners. This is especially true for events that have a no return value and pass an immutable
        /// <see cref="EventArgs"/>.</param>
        /// <exception cref="Exception"><paramref name="swallowExceptions"/> is <c>false</c> and an <see cref="Exception"/> has occured
        /// either while trying to invoke the <paramref name="handler"/>, or inside the <paramref name="handler"/> itself.</exception>
        public static void Raise<TSender, TEventArgs>(this TypedEventHandler<TSender, TEventArgs> handler, TSender sender,
                                                      TEventArgs e, bool swallowExceptions = true) where TEventArgs : EventArgs
        {
            Debug.Assert(e != null, "A null EventArgs should never be used! Use EventArgs.Empty instead.");

            try
            {
                // Check for a valid handler
                if (handler == null)
                    return;

                // Raise event
                handler(sender, e);
            }
            catch (Exception ex)
            {
                // Log any exception that occured
                const string errmsg =
                    "Exception occured in event listener on event handler `{0}` (sender: {1}; EventArgs: {2}). Exception: {3}";
                if (log.IsErrorEnabled)
                    log.ErrorFormat(errmsg, handler, sender, e, ex);
                Debug.Fail(string.Format(errmsg, handler, sender, e, ex));

                // If not swallowing exceptions, then throw so it can continue to bubble up
                if (!swallowExceptions)
                    throw;
            }
        }
    }
}