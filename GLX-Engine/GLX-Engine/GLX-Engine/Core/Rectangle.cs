using System;

namespace GLXEngine.Core
{
	public class Rectangle : Shape
	{
		public float width, height;

		//------------------------------------------------------------------------------------------------------------------------
		//														Rectangle()
		//------------------------------------------------------------------------------------------------------------------------
		public Rectangle (float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Properties()
		//------------------------------------------------------------------------------------------------------------------------
		public float left { get { return x; } set { x = value; } }
		public float right { get { return x + width; } set { width = value - x; } }
		public float top { get { return y; } set { y = value; } }
		public float bottom { get { return y + height; } set { height = value - y; } }

        public override bool Contains(Vector2 a_point)
        {
            return a_point.x >= left && a_point.x <= right && a_point.y >= top && a_point.y <= bottom;
        }

        public override bool Overlaps(Rectangle a_other)
        {
            return !(a_other.left > right || a_other.right < left || a_other.bottom < top || a_other.top > bottom);
        }

		//------------------------------------------------------------------------------------------------------------------------
		//														ToString()
		//------------------------------------------------------------------------------------------------------------------------
		override public string ToString() {
			return (x + "," + y + "," + width + "," + height);
		}
		
	}
}

