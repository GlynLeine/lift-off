using System;
using GLXEngine.Core;

namespace GLXEngine
{
    /// <summary>
    /// The Sprite class holds 2D images that can be used as objects in your game.
    /// </summary>
    /// 
    public class Sprite : BoundsObject
    {
        protected Texture2D _texture;
        protected float[] _uvs;

        private uint _color = 0xFFFFFF;
        private float _alpha = 1.0f;

        protected bool _mirrorX = false;
        protected bool _mirrorY = false;

        public BlendMode blendMode = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLXEngine.Sprite"/> class.
        /// Specify a System.Drawing.Bitmap to use. Bitmaps will not be cached.
        /// </summary>
        /// <param name='bitmap'>
        /// Bitmap.
        /// </param>
        public Sprite(System.Drawing.Bitmap bitmap)
        {
            name = "BMP" + bitmap.Width + "x" + bitmap.Height;
            initializeFromTexture(new Texture2D(bitmap));
        }

        public Sprite(Texture2D a_texture2D)
        {
            name = a_texture2D.filename;
            initializeFromTexture(a_texture2D);
        }

        public Sprite(int width, int height)
        {
            name = "BMP" + width + "x" + height;
            initializeFromTexture(new Texture2D(width, height));
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														OnDestroy()
        //------------------------------------------------------------------------------------------------------------------------
        protected override void OnDestroy()
        {
            if (_texture != null) _texture.Dispose();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Sprite()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="GLXEngine.Sprite"/> class.
        /// Specify an image file to load. Please use a full filename. Initial path is the application folder.
        /// Images will be cached internally. That means once it is loaded, the same data will be used when
        /// you load the file again.
        /// </summary>
        /// <param name='filename'>
        /// The name of the file that should be loaded.
        /// </param>
        public Sprite(string filename, bool keepInCache = false, Scene a_scene = null)
        {
            name = filename;
            initializeFromTexture(Texture2D.GetInstance(filename, keepInCache));
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														initializeFromTexture()
        //------------------------------------------------------------------------------------------------------------------------
        protected void initializeFromTexture(Texture2D texture)
        {
            _texture = texture;
            SetBounds(_texture.width, _texture.height);
            setUVs();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														setUVs
        //------------------------------------------------------------------------------------------------------------------------
        protected virtual void setUVs()
        {
            float left = _mirrorX ? 1.0f : 0.0f;
            float right = _mirrorX ? 0.0f : 1.0f;
            float top = _mirrorY ? 1.0f : 0.0f;
            float bottom = _mirrorY ? 0.0f : 1.0f;
            _uvs = new float[8] { left, top, right, top, right, bottom, left, bottom };
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														texture
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the texture that is used to create this sprite.
        /// If no texture is used, null will be returned.
        /// Use this to retreive the original width/height or filename of the texture.
        /// </summary>
        public Texture2D texture
        {
            get { return _texture; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														width
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the sprite's width in pixels.
        /// </summary>
        override public float width
        {
            get
            {
                if (_texture != null) return Math.Abs(_texture.width * scaleX);
                return 0;
            }
            set
            {
                if (_texture != null && _texture.width != 0) scaleX = value / _texture.width;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														height
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the sprite's height in pixels.
        /// </summary>
        override public float height
        {
            get
            {
                if (_texture != null) return Math.Abs(_texture.height * scaleY);
                return 0;
            }
            set
            {
                if (_texture != null && _texture.height != 0) scaleY = value / _texture.height;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf()
        //------------------------------------------------------------------------------------------------------------------------
        override protected void RenderSelf(GLContext glContext)
        {
            if (game != null)
            {
                Vector2[] bounds = GetExtents();
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                for (int i = 0; i < 4; i++)
                {
                    if (bounds[i].x > maxX) maxX = bounds[i].x;
                    if (bounds[i].x < minX) minX = bounds[i].x;
                    if (bounds[i].y > maxY) maxY = bounds[i].y;
                    if (bounds[i].y < minY) minY = bounds[i].y;
                }
                bool test = (maxX < game.RenderRange.left) || (maxY < game.RenderRange.top) || (minX >= game.RenderRange.right) || (minY >= game.RenderRange.bottom);
                if (test == false)
                {
                    if (blendMode != null) blendMode.enable();
                    _texture.Bind();
                    glContext.SetColor((byte)((_color >> 16) & 0xFF),
                                       (byte)((_color >> 8) & 0xFF),
                                       (byte)(_color & 0xFF),
                                       (byte)(_alpha * 0xFF));
                    glContext.DrawQuad(GetArea(), _uvs);
                    glContext.SetColor(1, 1, 1, 1);
                    _texture.Unbind();
                    if (blendMode != null) BlendMode.NORMAL.enable();
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Mirror
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// This function can be used to enable mirroring for the sprite in either x or y direction.
        /// </summary>
        /// <param name='mirrorX'>
        /// If set to <c>true</c> to enable mirroring in x direction.
        /// </param>
        /// <param name='mirrorY'>
        /// If set to <c>true</c> to enable mirroring in y direction.
        /// </param>
        public void Mirror(bool mirrorX, bool mirrorY)
        {
            _mirrorX = mirrorX;
            _mirrorY = mirrorY;
            setUVs();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														color
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the color filter for this sprite.
        /// This can be any value between 0x000000 and 0xFFFFFF.
        /// </summary>
        public uint color
        {
            get { return _color; }
            set { _color = value & 0xFFFFFF; }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														color
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the color of the sprite.
        /// </summary>
        /// <param name='r'>
        /// The red component, range 0..1
        /// </param>
        /// <param name='g'>
        /// The green component, range 0..1
        /// </param>
        /// <param name='b'>
        /// The blue component, range 0..1
        /// </param>
        public void SetColor(float r, float g, float b)
        {
            r = Mathf.Clamp(r, 0, 1);
            g = Mathf.Clamp(g, 0, 1);
            b = Mathf.Clamp(b, 0, 1);
            byte rr = (byte)Math.Floor((r * 255));
            byte rg = (byte)Math.Floor((g * 255));
            byte rb = (byte)Math.Floor((b * 255));
            color = (uint)rb + (uint)(rg << 8) + (uint)(rr << 16);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														alpha
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the alpha value of the sprite. 
        /// Setting this value allows you to make the sprite (semi-)transparent.
        /// The alpha value should be in the range 0...1, where 0 is fully transparent and 1 is fully opaque.
        /// </summary>
        public float alpha
        {
            get { return _alpha; }
            set { _alpha = value; }
        }

    }
}

