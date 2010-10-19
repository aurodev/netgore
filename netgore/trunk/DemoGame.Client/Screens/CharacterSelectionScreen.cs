using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using log4net;
using NetGore;
using NetGore.Graphics;
using NetGore.Graphics.GUI;
using NetGore.IO;
using NetGore.World;
using SFML.Graphics;
using SFML.Window;

namespace DemoGame.Client
{
    class CharacterSelectionScreen : GameMenuScreenBase
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public const string ScreenName = "character selection";
        const string _title = "Select Character";
        const string _unusedCharacterSlotText = "unused";

        CharacterSlotControl[] _charSlotControls;
        ClientSockets _sockets = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterSelectionScreen"/> class.
        /// </summary>
        /// <param name="screenManager">The <see cref="IScreenManager"/> to add this <see cref="GameScreen"/> to.</param>
        public CharacterSelectionScreen(IScreenManager screenManager) : base(screenManager, ScreenName, _title)
        {
        }

        /// <summary>
        /// Handles screen activation, which occurs every time the screen becomes the current
        /// active screen. Objects in here often will want to be destroyed on Deactivate().
        /// </summary>
        public override void Activate()
        {
            base.Activate();

            if (_sockets == null)
                _sockets = ClientSockets.Instance;

            _sockets.PacketHandler.AccountCharacterInfos.AccountCharactersLoaded += HandleCharInfosUpdated;
        }

        void ClickButton_CharacterSelection(object sender, MouseButtonEventArgs e)
        {
            var src = (CharacterSlotControl)sender;
            var slot = src.Slot;

            AccountCharacterInfo charInfo;
            if (!_sockets.PacketHandler.AccountCharacterInfos.TryGetInfo(slot, out charInfo))
                ScreenManager.SetScreen(CreateCharacterScreen.ScreenName);
            else
            {
                using (var pw = ClientPacket.SelectAccountCharacter(slot))
                {
                    _sockets.Send(pw, ClientMessageType.System);
                }
            }
        }

        void ClickButton_LogOut(object sender, MouseButtonEventArgs e)
        {
            // Change screens
            ScreenManager.SetScreen(LoginScreen.ScreenName);

            // Disconnect the socket so we actually "log out"
            if (_sockets != null)
            {
                try
                {
                    _sockets.Disconnect();
                }
                catch (Exception ex)
                {
                    // Ignore errors in disconnecting
                    Debug.Fail("Disconnect failed: " + ex);
                    if (log.IsErrorEnabled)
                        log.ErrorFormat("Failed to disconnect client socket ({0}). Exception: {1}", _sockets, ex);
                }
            }
        }

        void HandleCharInfosUpdated(AccountCharacterInfos sender)
        {
            for (var i = 0; i < _charSlotControls.Length; i++)
            {
                AccountCharacterInfo charInfo;
                if (_sockets.PacketHandler.AccountCharacterInfos.TryGetInfo((byte)i, out charInfo))
                    _charSlotControls[i].CharInfo = charInfo;
            }
        }

        /// <summary>
        /// Handles initialization of the GameScreen. This will be invoked after the GameScreen has been
        /// completely and successfully added to the ScreenManager. It is highly recommended that you
        /// use this instead of the constructor. This is invoked only once.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            var cScreen = new Panel(GUIManager, Vector2.Zero, ScreenManager.ScreenSize);

            // Create the menu buttons
            var menuButtons = GameScreenHelper.CreateMenuButtons(ScreenManager, cScreen, "Log out");
            menuButtons["Log out"].Clicked += ClickButton_LogOut;

            // Create the character slots
            _charSlotControls = new CharacterSlotControl[GameData.MaxCharactersPerAccount];
            for (var i = 0; i < GameData.MaxCharactersPerAccount; i++)
            {
                var c = new CharacterSlotControl(cScreen, (byte)i);
                c.Clicked += ClickButton_CharacterSelection;
                _charSlotControls[i] = c;
            }
        }

        /// <summary>
        /// A <see cref="Control"/> that displays a single character slot.
        /// </summary>
        class CharacterSlotControl : Panel
        {
            /// <summary>
            /// The name of this <see cref="Control"/> for when looking up the skin information.
            /// </summary>
            const string _controlSkinName = "Character Slot";

            /// <summary>
            /// The number of slot controls per row.
            /// </summary>
            const int _slotsPerRow = 3;

            /// <summary>
            /// The y-axis offset from the top of the screen (so we don't cover the title).
            /// </summary>
            const float _yOffset = 120f;

            /// <summary>
            /// The color of the slot number text.
            /// </summary>
            static readonly Color _slotNumberTextColor = Color.LimeGreen;

            /// <summary>
            /// The size of each individual slot.
            /// </summary>
            static readonly Vector2 _slotSize = new Vector2(320, 180);

            /// <summary>
            /// The padding between the slots and between the slots.
            /// </summary>
            static readonly Vector2 slotPadding = new Vector2(10, 10);

            readonly Label _charNameControl;
            readonly PreviewCharacter _character = new PreviewCharacter();

            readonly byte _slot;
            readonly Label _slotNumberControl;

            AccountCharacterInfo _charInfo;
            ControlBorder _defaultBorder;
            ControlBorder _mouseOverBorder;

            /// <summary>
            /// Initializes a new instance of the <see cref="CharacterSlotControl"/> class.
            /// </summary>
            /// <param name="parent">Parent <see cref="Control"/> of this <see cref="Control"/>.</param>
            /// <param name="slot">The 0-base slot number.</param>
            /// <exception cref="NullReferenceException"><paramref name="parent"/> is null.</exception>
            public CharacterSlotControl(Control parent, byte slot) : base(parent, Vector2.Zero, _slotSize)
            {
                _slot = slot;

                Size = _slotSize;
                UpdatePositioning();

                // Create child controls

                // Slot number
                _slotNumberControl = CreateChildLabel(new Vector2(2, 2), (Slot + 1) + ". ");
                _slotNumberControl.ForeColor = _slotNumberTextColor;

                // Character name
                var charNameControlPos = _slotNumberControl.Position + new Vector2(_slotNumberControl.Size.X + 2, 0f);
                _charNameControl = CreateChildLabel(charNameControlPos, _unusedCharacterSlotText);
            }

            /// <summary>
            /// Gets or sets the <see cref="AccountCharacterInfo"/> for the character in this slot.
            /// </summary>
            public AccountCharacterInfo CharInfo
            {
                get { return _charInfo; }
                set
                {
                    _charInfo = value;

                    if (CharInfo != null)
                        _charNameControl.Text = CharInfo.Name;
                    else
                        _charNameControl.Text = _unusedCharacterSlotText;

                    if (_character != null && _charInfo != null)
                    {
                        _character.Body = _charInfo.BodyID;
                        _character.SetPaperDollLayers(_charInfo.EquippedBodies);
                        _character.Position = GetCharacterPosition();
                    }
                }
            }

            /// <summary>
            /// Gets the 0-based character slot number that this control is for.
            /// </summary>
            public byte Slot
            {
                get { return _slot; }
            }

            Label CreateChildLabel(Vector2 position, string text)
            {
                var ret = GameScreenHelper.CreateMenuLabel(this, position, text);
                ret.IsEnabled = false;
                ret.CanFocus = false;
                ret.IsBoundToParentArea = false;
                return ret;
            }

            /// <summary>
            /// Draws the <see cref="Control"/>.
            /// </summary>
            /// <param name="spriteBatch">The <see cref="ISpriteBatch"/> to draw to.</param>
            protected override void DrawControl(ISpriteBatch spriteBatch)
            {
                base.DrawControl(spriteBatch);

                if (_character != null)
                {
                    _character.Draw(spriteBatch);
                }
            }

            /// <summary>
            /// Gets the position to draw the <see cref="PreviewCharacter"/> at.
            /// </summary>
            /// <returns>The position to draw the <see cref="PreviewCharacter"/> at.</returns>
            Vector2 GetCharacterPosition()
            {
                var sp = ScreenPosition;
                var cs = ClientSize;
                var max = sp + cs;

                var center = sp + (cs / 2f);
                var offset = _character.Size / 2f;
                var pos = center - offset;

                pos.Y = max.Y - _character.Size.Y - 5;

                return pos;
            }

            /// <summary>
            /// When overridden in the derived class, loads the skinning information for the <see cref="Control"/>
            /// from the given <paramref name="skinManager"/>.
            /// </summary>
            /// <param name="skinManager">The <see cref="ISkinManager"/> to load the skinning information from.</param>
            public override void LoadSkin(ISkinManager skinManager)
            {
                _defaultBorder = skinManager.GetBorder(_controlSkinName);
                _mouseOverBorder = skinManager.GetBorder(_controlSkinName, "MouseOver");

                if (IsMouseEntered)
                    Border = _mouseOverBorder;
                else
                    Border = _defaultBorder;
            }

            /// <summary>
            /// Handles when the mouse has entered the area of the <see cref="Control"/>.
            /// This is called immediately before <see cref="Control.OnMouseEnter"/>.
            /// Override this method instead of using an event hook on <see cref="Control.MouseEnter"/> when possible.
            /// </summary>
            /// <param name="e">The event args.</param>
            protected override void OnMouseEnter(MouseMoveEventArgs e)
            {
                base.OnMouseEnter(e);

                Border = _mouseOverBorder;
            }

            /// <summary>
            /// Handles when the mouse has left the area of the <see cref="Control"/>.
            /// This is called immediately before <see cref="Control.OnMouseLeave"/>.
            /// Override this method instead of using an event hook on <see cref="Control.MouseLeave"/> when possible.
            /// </summary>
            /// <param name="e">The event args.</param>
            protected override void OnMouseLeave(MouseMoveEventArgs e)
            {
                base.OnMouseLeave(e);

                Border = _defaultBorder;
            }

            /// <summary>
            /// Updates the <see cref="Control"/>. This is called for every <see cref="Control"/>, even if it is disabled or
            /// not visible.
            /// </summary>
            /// <param name="currentTime">The current time in milliseconds.</param>
            protected override void UpdateControl(TickCount currentTime)
            {
                base.UpdateControl(currentTime);

                if (_character != null)
                    _character.Update();
            }

            /// <summary>
            /// Sets the default values for the <see cref="Control"/>. This should always begin with a call to the
            /// base class's method to ensure that changes to settings are hierchical.
            /// </summary>
            protected override void SetDefaultValues()
            {
                base.SetDefaultValues();

                ResizeToChildren = false;
            }

            /// <summary>
            /// Updates the position of the slot.
            /// </summary>
            void UpdatePositioning()
            {
                // Find the x-axis offset to use to center the slots
                var xOffset = _slotSize.X * _slotsPerRow;
                xOffset += (_slotsPerRow - 1) * slotPadding.X;
                xOffset = Parent.ClientSize.X - xOffset;
                xOffset /= 2;

                var offset = new Vector2(xOffset, _yOffset);

                // Find the position
                var row = Slot % _slotsPerRow;
                var column = Slot / _slotsPerRow;
                var pos = offset + (new Vector2(row, column) * (slotPadding + _slotSize));

                Position = pos;
            }
        }

        class PreviewCharacter : Entity, IGetTime
        {
            static readonly SkeletonManager _skeletonManager = SkeletonManager.Create(ContentPaths.Build);

            readonly ICharacterSprite _characterSprite;

            BodyID _body;

            /// <summary>
            /// Initializes a new instance of the <see cref="PreviewCharacter"/> class.
            /// </summary>
            public PreviewCharacter()
            {
                _characterSprite = Character.CreateCharacterSprite(this, this, _skeletonManager);
            }

            public void SetPaperDollLayers(IEnumerable<string> values)
            {
                _characterSprite.SetPaperDollLayers(values);
            }

            public BodyID Body
            {
                get { return _body; }
                set
                {
                    if (_body == value)
                        return;

                    _body = value;

                    var bodyInfo = BodyInfoManager.Instance.GetBody(Body);

                    if (bodyInfo != null)
                    {
                        _characterSprite.SetSet(bodyInfo.Walk, bodyInfo.Size);
                        _characterSprite.SetBody(bodyInfo.Body);
                        _characterSprite.SetPaperDollLayers(null);

                        Size = bodyInfo.Size;
                    }
                    else
                    {
                        _characterSprite.SetSet(null, Vector2.Zero);
                        _characterSprite.SetBody(null);
                        _characterSprite.SetPaperDollLayers(null);

                        Size = Vector2.One;
                    }
                }
            }

            /// <summary>
            /// When overridden in the derived class, gets if this <see cref="Entity"/> will collide against
            /// walls. If false, this <see cref="Entity"/> will pass through walls and completely ignore them.
            /// </summary>
            public override bool CollidesAgainstWalls
            {
                get { return false; }
            }

            public void Draw(ISpriteBatch sb)
            {
                _characterSprite.Draw(sb, Position, Direction.East, Color.White);
            }

            /// <summary>
            /// Perform pre-collision velocity and position updating.
            /// </summary>
            /// <param name="map">The map.</param>
            /// <param name="deltaTime">The amount of that that has elapsed time since last update.</param>
            public override void UpdateVelocity(IMap map, int deltaTime)
            {
            }

            public void Update()
            {
                if (_characterSprite != null)
                    _characterSprite.Update(GetTime());
            }

            #region IGetTime Members

            /// <summary>
            /// Gets the current time in milliseconds.
            /// </summary>
            /// <returns>The current time in milliseconds.</returns>
            public TickCount GetTime()
            {
                return TickCount.Now;
            }

            #endregion
        }
    }
}