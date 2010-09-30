﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DemoGame.Client;
using NetGore;
using NetGore.EditorTools;
using NetGore.Graphics;
using NetGore.IO;
using NetGore.World;
using SFML.Graphics;

namespace DemoGame.Editor
{
    /// <summary>
    /// The <see cref="GraphicsDeviceControl"/> that provides all the actual displaying and interaction of a <see cref="Map"/>
    /// instance.
    /// </summary>
    public class MapScreenControl : GraphicsDeviceControl, IMapBoundControl, IGetTime
    {
        readonly ScreenGrid _grid;

        DrawingManager _drawingManager;
        Vector2 _cursorPos;
        Map _map;
        MouseButtons _mouseButton;

        /// <summary>
        /// Allows derived classes to handle when the <see cref="GraphicsDeviceControl.RenderWindow"/> is created or re-created.
        /// </summary>
        /// <param name="newRenderWindow">The current <see cref="GraphicsDeviceControl.RenderWindow"/>.</param>
        protected override void OnRenderWindowCreated(RenderWindow newRenderWindow)
        {
            base.OnRenderWindowCreated(newRenderWindow);

            // Update the DrawingManager
            if (_drawingManager == null || _drawingManager.IsDisposed)
                _drawingManager = new DrawingManager(newRenderWindow);
            else
                _drawingManager.RenderWindow = newRenderWindow;
        }

        /// <summary>
        /// Gets the <see cref="IDrawingManager"/> used to display the map.
        /// </summary>
        public IDrawingManager DrawingManager
        {
            get { return _drawingManager; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapScreenControl"/> class.
        /// </summary>
        public MapScreenControl()
        {
            if (DesignMode)
                return;

            _grid = new ScreenGrid();
            _camera = new Camera2D(new Vector2(ClientSize.Width, ClientSize.Height));
        }

        public void ChangeMap(MapID mapID)
        {
            if (Map != null && Map.ID == mapID)
                return;

            Map = new Map(mapID, Camera, this);
            Map.Load(ContentPaths.Dev, false, MapEditorDynamicEntityFactory.Instance);
        }

        readonly ICamera2D _camera;

        /// <summary>
        /// Gets the camera used to view the map.
        /// </summary>
        [Browsable(false)]
        public ICamera2D Camera
        {
            get { return _camera; }
        }

        /// <summary>
        /// Gets or sets the current position of the cursor in the world.
        /// </summary>
        [Browsable(false)]
        public Vector2 CursorPos
        {
            get { return _cursorPos; }
            set { _cursorPos = value; }
        }

        /// <summary>
        /// Gets the <see cref="ScreenGrid"/> to display for the <see cref="Map"/>.
        /// </summary>
        [Browsable(false)]
        public ScreenGrid Grid
        {
            get { return _grid; }
        }

        /// <summary>
        /// Gets or sets the map being displayed on this <see cref="MapScreenControl"/>.
        /// </summary>
        [Browsable(false)]
        public Map Map
        {
            get { return _map; }
            set { _map = value; }
        }

        /// <summary>
        /// Gets the <see cref="MouseButtons"/> current pressed.
        /// </summary>
        [Browsable(false)]
        public MouseButtons MouseButton
        {
            get { return _mouseButton; }
        }

        TickCount _lastUpdateTime = TickCount.MinValue;

        /// <summary>
        /// When overridden in the derived class, draws the graphics to the control.
        /// </summary>
        /// <param name="currentTime">The current time.</param>
        protected override void HandleDraw(TickCount currentTime)
        {
            int deltaTime;
            if (_lastUpdateTime == TickCount.MinValue)
            {
                deltaTime = 30;
            }
            else
            {
                deltaTime = Math.Max(5, (int)(currentTime - _lastUpdateTime));
            }

            _lastUpdateTime = currentTime;

            DrawingManager.Update(currentTime);

            // Update
            UpdateMap(currentTime, deltaTime);

            // Draw the world
            var worldSB = DrawingManager.BeginDrawWorld(Camera);
            if (worldSB != null)
            {
                DrawMapWorld(worldSB);
                DrawingManager.EndDrawWorld();
            }

            // Draw the GUI
            var guiSB = DrawingManager.BeginDrawGUI();
            if (guiSB != null)
            {
                DrawMapGUI(guiSB);
                DrawingManager.EndDrawGUI();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Resize"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var clientSize = new Vector2(ClientSize.Width, ClientSize.Height);
            Camera.Size = clientSize * Camera.Scale;
        }

        protected virtual void UpdateMap(TickCount currentTime, int deltaTime)
        {
            Cursor = Cursors.Default;

            if (Map != null)
                Map.Update(deltaTime);
        }

        protected virtual void DrawMapWorld(ISpriteBatch sb)
        {
            // Check for a valid map
            if (Map == null)
                return;

            DrawingManager.LightManager.Ambient = Map.AmbientLight;

            Map.Draw(sb);

            // TODO: !! MapGrh bound walls
            /*
            if (chkDrawAutoWalls.Checked)
            {
                foreach (var mg in Map.MapGrhs)
                {
                    if (!_camera.InView(mg.Grh, mg.Position))
                        continue;

                    var boundWalls = _mapGrhWalls[mg.Grh.GrhData];
                    if (boundWalls == null)
                        continue;

                    foreach (var wall in boundWalls)
                    {
                        EntityDrawer.Draw(sb, Camera, wall, mg.Position);
                    }
                }
            }
            */

            // TODO: !! Border
            /*
            _mapBorderDrawer.Draw(sb, Map, _camera);
            */

            // TODO: !! Selection area
            /*
            CursorManager.DrawSelection(sb);
            */

            // Grid
            // TODO: !! if (chkDrawGrid.Checked)
                Grid.Draw(sb, Camera);

            // Light sources
            // TODO: !! if (chkLightSources.Checked)
            {
                var offset = AddLightCursor.LightSprite.Size / 2f;
                foreach (var light in DrawingManager.LightManager)
                {
                    AddLightCursor.LightSprite.Draw(sb, light.Position - offset);
                }
            }

            // TODO: !! Tool interface
            //CursorManager.DrawInterface(sb);

            // Focused selected object (don't draw it for lights, though)
            /*
            var som = GlobalState.Instance.Map.SelectedObjsManager;
            foreach (var selected in som.SelectedObjects.Where(x => !(x is ILight)))
            {
                if (selected == som.Focused)
                    _focusedSpatialDrawer.DrawFocused(selected as ISpatial, sb);
                else
                    FocusedSpatialDrawer.DrawNotFocused(selected as ISpatial, sb);
            }
            */
        }

        protected virtual void DrawMapGUI(ISpriteBatch sb)
        {
            // Cursor coordinates
            var font = GlobalState.Instance.DefaultRenderFont;

            var cursorPosText = CursorPos.ToString();
            var cursorPosTextPos = new Vector2(ClientSize.Width, ClientSize.Height) -
                                   font.MeasureString(cursorPosText) - new Vector2(4);

            sb.DrawStringShaded(font, cursorPosText, cursorPosTextPos, Color.White, Color.Black);
        }

        /// <summary>
        /// Derived classes override this to initialize their drawing code.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // We don't want to initialize any of this stuff in the design mode
            if (DesignMode)
                return;

            _drawingManager = new DrawingManager(RenderWindow);

            // Add an event hook to the tick timer so we can update ourself
            GlobalState.Instance.Tick -= InvokeDrawing;
            GlobalState.Instance.Tick += InvokeDrawing;
        }

        /// <summary>
        /// Handles MouseDown events.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            _mouseButton = e.Button;

            if (((IMapBoundControl)this).IMap != null)
                Focus();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _mouseButton = e.Button;

            if (Camera != null)
                _cursorPos = Camera.ToWorld(e.X, e.Y);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _mouseButton = e.Button;
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

        #region IMapBoundControl Members

        /// <summary>
        /// Gets or sets the current <see cref="IMapBoundControl.IMap"/>.
        /// </summary>
        IMap IMapBoundControl.IMap { get; set; }

        #endregion
    }
}