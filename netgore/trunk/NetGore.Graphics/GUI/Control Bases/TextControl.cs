using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NetGore.Graphics.GUI
{
    /// <summary>
    /// Base of a <see cref="Control"/> that contains a text display.
    /// </summary>
    public abstract class TextControl : Control
    {
        static readonly object _eventChangeFont = new object();
        static readonly object _eventChangeText = new object();
        SpriteFont _font = null;
        Color _foreColor = Color.Black;
        string _text = string.Empty;

        /// <summary>
        /// Notifies listeners when the <see cref="TextControl.Font"/> has changed.
        /// </summary>
        public event ControlEventHandler OnChangeFont
        {
            add { Events.AddHandler(_eventChangeFont, value); }
            remove { Events.RemoveHandler(_eventChangeFont, value); }
        }

        /// <summary>
        /// Notifies listeners when the <see cref="TextControl.Text"/> has changed.
        /// </summary>
        public event ControlEventHandler OnChangeText
        {
            add { Events.AddHandler(_eventChangeText, value); }
            remove { Events.RemoveHandler(_eventChangeText, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextControl"/> class.
        /// </summary>
        /// <param name="parent">Parent <see cref="Control"/> of this <see cref="Control"/>.</param>
        /// <param name="position">Position of the Control reletive to its parent.</param>
        /// <param name="clientSize">The size of the <see cref="Control"/>'s client area.</param>
        /// <exception cref="NullReferenceException"><paramref name="parent"/> is null.</exception>
        protected TextControl(Control parent, Vector2 position, Vector2 clientSize) : base(parent, position, clientSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextControl"/> class.
        /// </summary>
        /// <param name="guiManager">The GUI manager this <see cref="Control"/> will be managed by.</param>
        /// <param name="position">Position of the Control reletive to its parent.</param>
        /// <param name="clientSize">The size of the <see cref="Control"/>'s client area.</param>
        /// <exception cref="ArgumentNullException"><paramref name="guiManager"/> is null.</exception>
        protected TextControl(IGUIManager guiManager, Vector2 position, Vector2 clientSize)
            : base(guiManager, position, clientSize)
        {
        }

        /// <summary>
        /// Gets or sets the SpriteFont used by the TextControl.
        /// </summary>
        public virtual SpriteFont Font
        {
            get { return _font; }
            set
            {
                // Ensure the value has changed
                if (_font == value)
                    return;

                _font = value;

                InvokeChangeFont();
            }
        }

        /// <summary>
        /// Gets or sets the default foreground color used by the TextControl.
        /// </summary>
        [SyncValue]
        public Color ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }

        /// <summary>
        /// Gets or sets the text of the TextControl.
        /// </summary>
        public virtual string Text
        {
            get { return _text; }
            set
            {
                if (_text == value)
                    return;

                _text = value;

                InvokeChangeText();
            }
        }

        /// <summary>
        /// Handles when the <see cref="TextControl.Font"/> has changed.
        /// This is called immediately before <see cref="TextControl.OnChangeFont"/>.
        /// Override this method instead of using an event hook on <see cref="TextControl.OnChangeFont"/> when possible.
        /// </summary>
        protected virtual void ChangeFont()
        {
        }

        /// <summary>
        /// Handles when the <see cref="TextControl.Text"/> has changed.
        /// This is called immediately before <see cref="TextControl.OnChangeText"/>.
        /// Override this method instead of using an event hook on <see cref="TextControl.OnChangeText"/> when possible.
        /// </summary>
        protected virtual void ChangeText()
        {
        }

        /// <summary>
        /// Draws the text for the control.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw to.</param>
        /// <param name="position">Position relative to the Control to draw the text.</param>
        protected void DrawText(SpriteBatch spriteBatch, Vector2 position)
        {
            if (!string.IsNullOrEmpty(Text) && Font != null)
                spriteBatch.DrawString(Font, Text, ScreenPosition + position, _foreColor);
        }

        /// <summary>
        /// Gets the string for a key if the key represents a valid ASCII character.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <param name="state">Current KeyboardState.</param>
        /// <returns>String for the key, or String.Empty if invalid.</returns>
        public static string GetKeyString(Keys key, KeyboardState state)
        {
            return GetKeyString(key, state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift));
        }

        /// <summary>
        /// Gets the string for a key if the key represents a valid ASCII character.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <param name="shift">If true, shift will be treated as being pressed.</param>
        /// <returns>String for the key, or String.Empty if invalid.</returns>
        public static string GetKeyString(Keys key, bool shift)
        {
            if (shift)
            {
                // Shift is down
                return GetKeyStringShift(key);
            }
            else
            {
                // Shift is not down
                return GetKeyStringNoShift(key);
            }
        }

        /// <summary>
        /// Gets the string for a key if the key represents a valid ASCII character with shift up.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>String for the key, or String.Empty if invalid.</returns>
        static string GetKeyStringNoShift(Keys key)
        {
            switch (key)
            {
                    // Alpha
                case Keys.A:
                    return "a";
                case Keys.B:
                    return "b";
                case Keys.C:
                    return "c";
                case Keys.D:
                    return "d";
                case Keys.E:
                    return "e";
                case Keys.F:
                    return "f";
                case Keys.G:
                    return "g";
                case Keys.H:
                    return "h";
                case Keys.I:
                    return "i";
                case Keys.J:
                    return "j";
                case Keys.K:
                    return "k";
                case Keys.L:
                    return "l";
                case Keys.M:
                    return "m";
                case Keys.N:
                    return "n";
                case Keys.O:
                    return "o";
                case Keys.P:
                    return "p";
                case Keys.Q:
                    return "q";
                case Keys.R:
                    return "r";
                case Keys.S:
                    return "s";
                case Keys.T:
                    return "t";
                case Keys.U:
                    return "u";
                case Keys.V:
                    return "v";
                case Keys.W:
                    return "w";
                case Keys.X:
                    return "x";
                case Keys.Y:
                    return "y";
                case Keys.Z:
                    return "z";

                    // Numeric
                case Keys.D0:
                case Keys.NumPad0:
                    return "0";
                case Keys.D1:
                case Keys.NumPad1:
                    return "1";
                case Keys.D2:
                case Keys.NumPad2:
                    return "2";
                case Keys.D3:
                case Keys.NumPad3:
                    return "3";
                case Keys.D4:
                case Keys.NumPad4:
                    return "4";
                case Keys.D5:
                case Keys.NumPad5:
                    return "5";
                case Keys.D6:
                case Keys.NumPad6:
                    return "6";
                case Keys.D7:
                case Keys.NumPad7:
                    return "7";
                case Keys.D8:
                case Keys.NumPad8:
                    return "8";
                case Keys.D9:
                case Keys.NumPad9:
                    return "9";

                    // Misc
                case Keys.Decimal:
                    return ".";
                case Keys.Divide:
                    return "/";
                case Keys.Enter:
                    return Environment.NewLine;
                case Keys.Multiply:
                    return "*";
                case Keys.OemBackslash:
                    return "\\";
                case Keys.OemCloseBrackets:
                    return "]";
                case Keys.OemComma:
                    return ",";
                case Keys.OemMinus:
                    return "-";
                case Keys.OemOpenBrackets:
                    return "[";
                case Keys.OemPeriod:
                    return ".";
                case Keys.OemPipe:
                    return "\\";
                case Keys.OemPlus:
                    return "=";
                case Keys.OemQuestion:
                    return "/";
                case Keys.OemQuotes:
                    return "'";
                case Keys.OemSemicolon:
                    return ";";
                case Keys.OemTilde:
                    return "`";
                case Keys.Space:
                    return " ";
                    //case Keys.Tab:
                    //    return "\t";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the string for a key if the key represents a valid ASCII character with shift down.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>String for the key, or String.Empty if invalid.</returns>
        static string GetKeyStringShift(Keys key)
        {
            switch (key)
            {
                    // Alpha
                case Keys.A:
                    return "A";
                case Keys.B:
                    return "B";
                case Keys.C:
                    return "C";
                case Keys.D:
                    return "D";
                case Keys.E:
                    return "E";
                case Keys.F:
                    return "F";
                case Keys.G:
                    return "G";
                case Keys.H:
                    return "H";
                case Keys.I:
                    return "I";
                case Keys.J:
                    return "J";
                case Keys.K:
                    return "K";
                case Keys.L:
                    return "L";
                case Keys.M:
                    return "M";
                case Keys.N:
                    return "N";
                case Keys.O:
                    return "O";
                case Keys.P:
                    return "P";
                case Keys.Q:
                    return "Q";
                case Keys.R:
                    return "R";
                case Keys.S:
                    return "S";
                case Keys.T:
                    return "T";
                case Keys.U:
                    return "U";
                case Keys.V:
                    return "V";
                case Keys.W:
                    return "W";
                case Keys.X:
                    return "X";
                case Keys.Y:
                    return "Y";
                case Keys.Z:
                    return "Z";

                    // Numeric
                case Keys.D0:
                    return ")";
                case Keys.D1:
                    return "!";
                case Keys.D2:
                    return "@";
                case Keys.D3:
                    return "#";
                case Keys.D4:
                    return "$";
                case Keys.D5:
                    return "%";
                case Keys.D6:
                    return "^";
                case Keys.D7:
                    return "&";
                case Keys.D8:
                    return "*";
                case Keys.D9:
                    return "(";

                    // Misc
                case Keys.Decimal:
                    return ">";
                case Keys.Divide:
                    return "/";
                case Keys.Enter:
                    return Environment.NewLine;
                case Keys.Multiply:
                    return "*";
                case Keys.OemBackslash:
                    return "\\";
                case Keys.OemCloseBrackets:
                    return "}";
                case Keys.OemComma:
                    return "<";
                case Keys.OemMinus:
                    return "_";
                case Keys.OemOpenBrackets:
                    return "{";
                case Keys.OemPeriod:
                    return ">";
                case Keys.OemPipe:
                    return "|";
                case Keys.OemPlus:
                    return "+";
                case Keys.OemQuestion:
                    return "?";
                case Keys.OemQuotes:
                    return "\"";
                case Keys.OemSemicolon:
                    return ":";
                case Keys.OemTilde:
                    return "~";
                case Keys.Space:
                    return " ";
                    //case Keys.Tab:
                    //    return "\t";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Invokes the corresponding virtual method and event for the given event. Use this instead of invoking
        /// the virtual method and event directly to ensure that the event is invoked correctly.
        /// </summary>
        void InvokeChangeFont()
        {
            ChangeFont();
            var handler = Events[_eventChangeFont] as ControlEventHandler;
            if (handler != null)
                handler(this);
        }

        /// <summary>
        /// Invokes the corresponding virtual method and event for the given event. Use this instead of invoking
        /// the virtual method and event directly to ensure that the event is invoked correctly.
        /// </summary>
        void InvokeChangeText()
        {
            ChangeText();
            var handler = Events[_eventChangeText] as ControlEventHandler;
            if (handler != null)
                handler(this);
        }

        /// <summary>
        /// Checks if a key is part of the alphabet
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if part of the alphabet, else false</returns>
        public static bool IsKeyAlpha(Keys key)
        {
            switch (key)
            {
                case Keys.A:
                case Keys.B:
                case Keys.C:
                case Keys.D:
                case Keys.E:
                case Keys.F:
                case Keys.G:
                case Keys.H:
                case Keys.I:
                case Keys.J:
                case Keys.K:
                case Keys.L:
                case Keys.M:
                case Keys.N:
                case Keys.O:
                case Keys.P:
                case Keys.Q:
                case Keys.R:
                case Keys.S:
                case Keys.T:
                case Keys.U:
                case Keys.V:
                case Keys.W:
                case Keys.X:
                case Keys.Y:
                case Keys.Z:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if a key is numeric
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True if numeric, else false</returns>
        public static bool IsKeyNumeric(Keys key)
        {
            switch (key)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Processes the key input on the Control's Text
        /// </summary>
        /// <param name="cursorIndex">Cursor index in the Text</param>
        /// <param name="key">Key to process</param>
        /// <returns>Amount the cursor needs to be shifted from its original index</returns>
        protected int ProcessKeyInput(int cursorIndex, Keys key)
        {
            if (key == Keys.Delete || key == Keys.Back)
            {
                // Process a deletion or backspace press
                if (cursorIndex > 0)
                {
                    // Delete the previous character
                    Text = Text.Remove(cursorIndex - 1, 1);
                    return -1;
                }
                else
                {
                    // We are at the start of the text - nothing to delete
                    return 0;
                }
            }
            else
            {
                // Handle any other character
                string value = GetKeyString(key, GUIManager.KeyboardState);
                if (value != null)
                {
                    // A character was found for the given key, so add it into the text
                    Text = Text.Insert(cursorIndex, value);

                    // Don't handle Enter internally - for any other character, just send the length
                    // of the character added to the text (which is likely 1)
                    if (key == Keys.Enter)
                        return 0;
                    else
                        return value.Length;
                }
            }

            // Character couldn't be added
            return 0;
        }

        /// <summary>
        /// Sets the default values for the <see cref="Control"/>. This should always begin with a call to the
        /// base class's method to ensure that changes to settings are hierchical.
        /// </summary>
        protected override void SetDefaultValues()
        {
            base.SetDefaultValues();

            Text = string.Empty;
            Font = GUIManager.Font;
            ForeColor = Color.Black;
        }
    }
}