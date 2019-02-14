using System;

namespace GLXEngine.Core
{
	public struct Rectangle
	{
		public float x, y, width, height;
		
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

		//------------------------------------------------------------------------------------------------------------------------
		//														ToString()
		//------------------------------------------------------------------------------------------------------------------------
		override public string ToString() {
			return (x + "," + y + "," + width + "," + height);
		}
		
	}
}

