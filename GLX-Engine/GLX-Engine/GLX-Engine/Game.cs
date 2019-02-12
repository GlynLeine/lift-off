using GLXEngine.Core;
using GLXEngine.Managers;
using System;
using System.Collections.Generic;

namespace GLXEngine
{
    /// <summary>
    /// The Game class represents the Game window.
    /// Only a single instance of this class is allowed.
    /// </summary>
    public abstract class Game : Scene
    {
        public static Game main = null;

        private GLContext _glContext;

        public override event RenderDelegate OnAfterRender;

        public readonly bool PixelArt;

        //------------------------------------------------------------------------------------------------------------------------
        //														Game()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="GLXEngine.Game"/> class.
        /// This class represents a game window, containing an openGL view.
        /// </summary>
        /// <param name='width'>
        /// Width of the window in pixels.
        /// </param>
        /// <param name='height'>
        /// Height of the window in pixels.
        /// </param>
        /// <param name='fullScreen'>
        /// If set to <c>true</c> the application will run in fullscreen mode.
        /// </param>
        public Game(int pWidth, int pHeight, bool pFullScreen, bool pVSync = true, int pRealWidth = -1, int pRealHeight = -1, bool pPixelArt = false) : base()
        {
            if (pRealWidth <= 0)
            {
                pRealWidth = pWidth;
            }
            if (pRealHeight <= 0)
            {
                pRealHeight = pHeight;
            }
            PixelArt = pPixelArt;

            if (PixelArt)
            {
                // offset should be smaller than 1/(2 * "pixelsize"), but not zero:
                x = 0.01f;
                y = 0.01f;
            }

            if (main != null)
            {
                throw new Exception("Only a single instance of Game is allowed");
            }
            else
            {
                main = this;
                _glContext = new GLContext(this);
                _glContext.CreateWindow(pWidth, pHeight, pFullScreen, pVSync, pRealWidth, pRealHeight);

                _renderRange = new Rectangle(0, 0, pWidth, pHeight);

                //register ourselves for updates
                Add(this);

            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetViewPort()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the rendering output view port. All rendering will be done within the given rectangle.
        /// The default setting is {0, 0, game.width, game.height}.
        /// </summary>
        /// <param name='x'>
        /// The x coordinate.
        /// </param>
        /// <param name='y'>
        /// The y coordinate.
        /// </param>
        /// <param name='width'>
        /// The new width of the viewport.
        /// </param>
        /// <param name='height'>
        /// The new height of the viewport.
        /// </param>
        public void SetViewport(int x, int y, int width, int height)
        {
            // Translate from GLXEngine coordinates (origin top left) to OpenGL coordinates (origin bottom left):
            //Console.WriteLine ("Setting viewport to {0},{1},{2},{3}",x,y,width,height);
            _glContext.SetScissor(x, game.height - height - y, width, height);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ShowMouse()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Shows or hides the mouse cursor.
        /// </summary>
        /// <param name='enable'>
        /// Set this to 'true' to enable the cursor.
        /// Else, set this to 'false'.
        /// </param>
        public void ShowMouse(bool enable)
        {
            _glContext.ShowCursor(enable);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Start()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Start the game loop. Call this once at the start of your game.
        /// </summary>
        public override void Start()
        {
            base.Start();
            _glContext.Run();
        }

        bool recurse = true;

        public override void Render(GLContext glContext)
        {
            base.Render(glContext);
            if (OnAfterRender != null && recurse)
            {
                recurse = false;
                OnAfterRender(glContext);
                recurse = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf()
        //------------------------------------------------------------------------------------------------------------------------
        override protected void RenderSelf(GLContext glContext)
        {
            //empty
        }


        //------------------------------------------------------------------------------------------------------------------------
        //														width
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the width of the window.
        /// </summary>
        public override int width
        {
            get { return _glContext.width; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														height
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the height of the window.
        /// </summary>
        public override int height
        {
            get { return _glContext.height; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Destroy()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Destroys the game, and closes the game window.
        /// </summary>
        override protected void OnDestroy()
        {
            base.OnDestroy();
            _glContext.Close();
        }

        public int currentFps
        {
            get
            {
                return _glContext.currentFps;
            }
        }

        public int targetFps
        {
            get
            {
                return _glContext.targetFps;
            }
            set
            {
                _glContext.targetFps = value;
            }
        }

     }
}

