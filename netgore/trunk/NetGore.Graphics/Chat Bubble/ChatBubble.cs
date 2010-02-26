﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NetGore.Graphics
{
    /// <summary>
    /// A bubble of text that appears near an <see cref="Entity"/> and is used to denote chatting text.
    /// </summary>
    public class ChatBubble
    {
        readonly int _deathTime;
        readonly ChatBubbleManagerBase _manager;
        readonly Entity _owner;
        readonly string _text;
        readonly Vector2 _textSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatBubble"/> class.
        /// </summary>
        public ChatBubble()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatBubble"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="ChatBubble"/> manager.</param>
        /// <param name="owner">The <see cref="Entity"/> the <see cref="ChatBubble"/> is attached to.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="currentTime">The current game time.</param>
        public ChatBubble(ChatBubbleManagerBase manager, Entity owner, string text, int currentTime)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            _manager = manager;
            _owner = owner;
            _text = text;
            _deathTime = currentTime + manager.Lifespan;

            _textSize = CalculateSize();

            IsExpired = false;
        }

        /// <summary>
        /// Gets if the <see cref="ChatBubble"/> has expired.
        /// </summary>
        public bool IsExpired { get; private set; }

        /// <summary>
        /// Gets the <see cref="Entity"/> that this <see cref="ChatBubble"/> is attached to.
        /// </summary>
        public Entity Owner
        {
            get { return _owner; }
        }

        /// <summary>
        /// Gets the size of the <see cref="ChatBubble"/>.
        /// </summary>
        public Vector2 Size
        {
            get { return _textSize + _manager.Border.Size; }
        }

        /// <summary>
        /// Gets the text that is displayed in this <see cref="ChatBubble"/>.
        /// </summary>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        /// Calculates the size of the <see cref="ChatBubble"/>'s text (Border not included).
        /// </summary>
        /// <returns>The size of the <see cref="ChatBubble"/>'s text (Border not included).</returns>
        Vector2 CalculateSize()
        {
            return _manager.Font.MeasureString(Text);
        }

        /// <summary>
        /// Forces the <see cref="ChatBubble"/> to be destroyed immediately instead of waiting for it to expire.
        /// </summary>
        public void Destroy()
        {
            IsExpired = true;
        }

        /// <summary>
        /// Draws the <see cref="ChatBubble"/>.
        /// </summary>
        /// <param name="sb">The <see cref="SpriteBatch"/> to draw to.</param>
        public void Draw(SpriteBatch sb)
        {
            if (IsExpired)
                return;

            Vector2 size = Size;
            Vector2 drawPos = _manager.GetDrawOffset(Owner, size);

            // Draw the border
            Rectangle borderArea = new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)size.X, (int)size.Y);
            _manager.Border.Draw(sb, borderArea);

            // Draw the text
            Vector2 textPos = drawPos + new Vector2(_manager.Border.LeftWidth, _manager.Border.TopHeight);
            sb.DrawString(_manager.Font, Text, textPos.Round(), _manager.FontColor);
        }

        /// <summary>
        /// Updates the <see cref="ChatBubble"/>.
        /// </summary>
        /// <param name="currentTime">The current game time.</param>
        public void Update(int currentTime)
        {
            if (IsExpired)
                return;

            if (_deathTime <= currentTime)
            {
                Destroy();
                return;
            }
        }
    }
}