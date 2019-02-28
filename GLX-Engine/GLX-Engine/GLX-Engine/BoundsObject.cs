using System;
using GLXEngine.Core;


namespace GLXEngine
{
    public class BoundsObject : GameObject
    {

        protected Rectangle m_bounds;

        public BoundsObject(Scene a_scene, float a_width = 0, float a_height = 0) : base(a_scene)
        {
            m_bounds = new Rectangle(0, 0, a_width, a_height);
        }

        protected BoundsObject()
        {
        }

        public void SetBounds(float a_width, float a_height)
        {
            if(m_bounds != null)
            {
                m_bounds.width = a_width;
                m_bounds.height = a_height;
            }
            else
                m_bounds = new Rectangle(0, 0, a_width, a_height);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														width
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the sprite's width in pixels.
        /// </summary>
        virtual public float width
        {
            get
            {
                if (m_bounds != null) return m_bounds.width * scaleX;
                return 0;
            }
            set
            {
                if (m_bounds != null && m_bounds.width != 0) scaleX = value / m_bounds.width;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														height
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the sprite's height in pixels.
        /// </summary>
        virtual public float height
        {
            get
            {
                if (m_bounds != null) return m_bounds.height * scaleY;
                return 0;
            }
            set
            {
                if (m_bounds != null && m_bounds.height != 0) scaleY = value / m_bounds.height;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetArea()
        //------------------------------------------------------------------------------------------------------------------------
        public Vector2[] GetHull()
        {
            return new Vector2[4] {
               new Vector2(m_bounds.left, m_bounds.top),
               new Vector2(m_bounds.right, m_bounds.top),
               new Vector2(m_bounds.right, m_bounds.bottom),
               new Vector2(m_bounds.left, m_bounds.bottom)
            };
        }
        public float[] GetArea()
        {
            return new float[8] {
               m_bounds.left, m_bounds.top,
               m_bounds.right, m_bounds.top,
               m_bounds.right, m_bounds.bottom,
               m_bounds.left, m_bounds.bottom
            };
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetExtents()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the four corners of this object as a set of 4 Vector2s.
        /// </summary>
        /// <returns>
        /// The extents.
        /// </returns>
        public Vector2[] GetExtents()
        {
            Vector2[] ret = new Vector2[4];
            ret[0] = TransformPoint(m_bounds.left, m_bounds.top);
            ret[1] = TransformPoint(m_bounds.right, m_bounds.top);
            ret[2] = TransformPoint(m_bounds.right, m_bounds.bottom);
            ret[3] = TransformPoint(m_bounds.left, m_bounds.bottom);
            return ret;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetOrigin()
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the origin, the pivot point of this Sprite in pixels.
        /// </summary>
        /// <param name='x'>
        /// The x coordinate.
        /// </param>
        /// <param name='y'>
        /// The y coordinate.
        /// </param>
        public void SetOrigin(float x, float y)
        {
            m_bounds.x = -x;
            m_bounds.y = -y;
        }

    }
}
