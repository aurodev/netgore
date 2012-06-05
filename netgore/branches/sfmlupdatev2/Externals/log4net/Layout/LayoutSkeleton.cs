#region Copyright & License

//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using System.IO;
using System.Linq;
using log4net.Core;

namespace log4net.Layout
{
    /// <summary>
    /// Extend this abstract class to create your own log layout format.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the base implementation of the <see cref="ILayout"/>
    /// interface. Most layout objects should extend this class.
    /// </para>
    /// </remarks>
    /// <remarks>
    /// <note type="inheritinfo">
    /// <para>
    /// Subclasses must implement the <see cref="Format(TextWriter,LoggingEvent)"/>
    /// method.
    /// </para>
    /// <para>
    /// Subclasses should set the <see cref="IgnoresException"/> in their default
    /// constructor.
    /// </para>
    /// </note>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public abstract class LayoutSkeleton : ILayout, IOptionHandler
    {
        #region Member Variables

        /// <summary>
        /// The footer text
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <see cref="Footer"/> for more information.
        /// </para>
        /// </remarks>
        string m_footer = null;

        /// <summary>
        /// The header text
        /// </summary>
        /// <remarks>
        /// <para>
        /// See <see cref="Header"/> for more information.
        /// </para>
        /// </remarks>
        string m_header = null;

        /// <summary>
        /// Flag indicating if this layout handles exceptions
        /// </summary>
        /// <remarks>
        /// <para>
        /// <c>false</c> if this layout handles exceptions
        /// </para>
        /// </remarks>
        bool m_ignoresException = true;

        #endregion

        #region Constructors

        #endregion

        #region Implementation of IOptionHandler

        /// <summary>
        /// Activate component options
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// <para>
        /// This method must be implemented by the subclass.
        /// </para>
        /// </remarks>
        public abstract void ActivateOptions();

        #endregion

        #region Implementation of ILayout

        /// <summary>
        /// The content type output by this layout. 
        /// </summary>
        /// <value>The content type is <c>"text/plain"</c></value>
        /// <remarks>
        /// <para>
        /// The content type output by this layout.
        /// </para>
        /// <para>
        /// This base class uses the value <c>"text/plain"</c>.
        /// To change this value a subclass must override this
        /// property.
        /// </para>
        /// </remarks>
        public virtual string ContentType
        {
            get { return "text/plain"; }
        }

        /// <summary>
        /// The footer for the layout format.
        /// </summary>
        /// <value>the layout footer</value>
        /// <remarks>
        /// <para>
        /// The Footer text will be appended after all the logging events
        /// have been formatted and appended.
        /// </para>
        /// </remarks>
        public virtual string Footer
        {
            get { return m_footer; }
            set { m_footer = value; }
        }

        /// <summary>
        /// The header for the layout format.
        /// </summary>
        /// <value>the layout header</value>
        /// <remarks>
        /// <para>
        /// The Header text will be appended before any logging events
        /// are formatted and appended.
        /// </para>
        /// </remarks>
        public virtual string Header
        {
            get { return m_header; }
            set { m_header = value; }
        }

        /// <summary>
        /// Flag indicating if this layout handles exceptions
        /// </summary>
        /// <value><c>false</c> if this layout handles exceptions</value>
        /// <remarks>
        /// <para>
        /// If this layout handles the exception object contained within
        /// <see cref="LoggingEvent"/>, then the layout should return
        /// <c>false</c>. Otherwise, if the layout ignores the exception
        /// object, then the layout should return <c>true</c>.
        /// </para>
        /// <para>
        /// Set this value to override a this default setting. The default
        /// value is <c>true</c>, this layout does not handle the exception.
        /// </para>
        /// </remarks>
        public virtual bool IgnoresException
        {
            get { return m_ignoresException; }
            set { m_ignoresException = value; }
        }

        /// <summary>
        /// Implement this method to create your own layout format.
        /// </summary>
        /// <param name="writer">The TextWriter to write the formatted event to</param>
        /// <param name="loggingEvent">The event to format</param>
        /// <remarks>
        /// <para>
        /// This method is called by an appender to format
        /// the <paramref name="loggingEvent"/> as text.
        /// </para>
        /// </remarks>
        public abstract void Format(TextWriter writer, LoggingEvent loggingEvent);

        #endregion
    }
}