using System;
using GLXEngine.Core;

namespace GLXEngine
{
	/// <summary>
	/// The Transformable class contains all positional data of GameObjects.
	/// </summary>
	public class Transformable
	{
		public TransformComponent transform = new TransformComponent();

		//------------------------------------------------------------------------------------------------------------------------
		//														Transform()
		//------------------------------------------------------------------------------------------------------------------------
		public Transformable () {
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														GetMatrix()
		//------------------------------------------------------------------------------------------------------------------------		
		/// <summary>
		/// Returns the gameobject's 4x4 matrix.
		/// </summary>
		/// <value>
		/// The matrix.
		/// </value>
		public float[] matrix {
			get {
				float[] matrix = (float[])transform.m_matrix.Clone();
				matrix[0] *= transform.m_scale.x;
				matrix[1] *= transform.m_scale.x;
				matrix[4] *= transform.m_scale.y;
				matrix[5] *= transform.m_scale.y;
				return matrix;
			}
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														x
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the x position.
		/// </summary>
		/// <value>
		/// The x.
		/// </value>
		public float x {
			get { return transform.m_matrix[12]; }
			set { transform.m_matrix[12] = value; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														y
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the y position.
		/// </summary>
		/// <value>
		/// The y.
		/// </value>
		public float y {
			get { return transform.m_matrix[13]; }
			set { transform.m_matrix[13] = value; }
		}

        //------------------------------------------------------------------------------------------------------------------------
		//														position
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		/// <value>
		/// The position.
		/// </value>
		public Vector2 position {
			get { return new Vector2(transform.m_matrix[12], transform.m_matrix[13]);}
			set { transform.m_matrix[12] = value.x; transform.m_matrix[13] = value.y;}
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetXY
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the X and Y position.
		/// </summary>
		/// <param name='x'>
		/// The x coordinate.
		/// </param>
		/// <param name='y'>
		/// The y coordinate.
		/// </param>
		public void SetXY(float x, float y) {
			transform.m_matrix[12] = x;
			transform.m_matrix[13] = y;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//													InverseTransformPoint()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Transforms the point from the game's global space to this object's local space.
		/// </summary>
		/// <returns>
		/// The point.
		/// </returns>
		/// <param name='x'>
		/// The x coordinate.
		/// </param>
		/// <param name='y'>
		/// The y coordinate.
		/// </param>
		public virtual Vector2 InverseTransformPoint (float x, float y)
		{
			Vector2 ret = new Vector2 ();
			x -= transform.m_matrix [12];
			y -= transform.m_matrix [13];
			if (transform.m_scale.x != 0) ret.x = ((x * transform.m_matrix[0] + y * transform.m_matrix[1]) / transform.m_scale.x); else ret.x = 0;
			if (transform.m_scale.y != 0) ret.y = ((x * transform.m_matrix[4] + y * transform.m_matrix[5]) / transform.m_scale.y); else ret.y = 0;
			return ret;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														DistanceTo()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Returns the distance to another Transformable
		/// </summary>
		public float DistanceTo (Transformable other)
		{
			float dx = other.x - x;
			float dy = other.y - y;
			return Mathf.Sqrt(dx * dx + dy * dy);
		}

				
		//------------------------------------------------------------------------------------------------------------------------
		//														TransformPoint()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Transforms the point from this object's local space to the game's global space.
		/// </summary>
		/// <returns>
		/// The point.
		/// </returns>
		/// <param name='x'>
		/// The x coordinate.
		/// </param>
		/// <param name='y'>
		/// The y coordinate.
		/// </param>
		public virtual Vector2 TransformPoint(float x, float y) {
			Vector2 ret = new Vector2();
			ret.x = (transform.m_matrix[0] * x * transform.m_scale.x + transform.m_matrix[4] * y * transform.m_scale.y + transform.m_matrix[12]);
			ret.y = (transform.m_matrix[1] * x * transform.m_scale.x + transform.m_matrix[5] * y * transform.m_scale.y + transform.m_matrix[13]);
			return ret;
		}



		//------------------------------------------------------------------------------------------------------------------------
		//														Rotation
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the object's rotation in degrees.
		/// </summary>
		/// <value>
		/// The rotation.
		/// </value>
		public float rotation {
			get { return transform.rotation; }
			set {
				transform.rotation = value;
			}
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Turn()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Turn the specified object with a certain angle in degrees.
		/// </summary>
		/// <param name='angle'>
		/// Angle.
		/// </param>
		public void Turn (float angle) {
			rotation = transform.rotation + angle;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Move()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Move the object, based on its current rotation.
		/// </summary>
		/// <param name='stepX'>
		/// Step x.
		/// </param>
		/// <param name='stepY'>
		/// Step y.
		/// </param>
		public void Move (float stepX, float stepY) {
			float r = transform.rotation * Mathf.PI / 180.0f;
			float cs = Mathf.Cos (r);
			float sn = Mathf.Sin (r);
			transform.m_matrix[12] = (transform.m_matrix[12] + cs * stepX - sn * stepY);
			transform.m_matrix[13] = (transform.m_matrix[13] + sn * stepX + cs * stepY);
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														Translate()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Move the object, in world space. (Object rotation is ignored)
		/// </summary>
		/// <param name='stepX'>
		/// Step x.
		/// </param>
		/// <param name='stepY'>
		/// Step y.
		/// </param>
		public void Translate (float stepX, float stepY) {
			transform.m_matrix[12] += stepX;
			transform.m_matrix[13] += stepY;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														SetScaleXY()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's scaling
		/// </summary>
		/// <param name='scaleX'>
		/// Scale x.
		/// </param>
		/// <param name='scaleY'>
		/// Scale y.
		/// </param>
		public void SetScaleXY(float scaleX, float scaleY) {
			transform.m_scale.x = scaleX;
			transform.m_scale.y = scaleY;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														SetScale()
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's scaling
		/// </summary>
		/// <param name='scale'>
		/// Scale x and y.
		/// </param>
		public void SetScaleXY(float scale) {
			transform.m_scale.x = scale;
			transform.m_scale.y = scale;
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														scaleX
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's x-axis scale
		/// </summary>
		/// <value>
		/// The scale x.
		/// </value>
		public float scaleX {
			get { return transform.m_scale.x; }
			set { transform.m_scale.x = value; }
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														scaleY
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's y-axis scale
		/// </summary>
		/// <value>
		/// The scale y.
		/// </value>
		public float scaleY {
			get { return transform.m_scale.y; }
			set { transform.m_scale.y = value; }
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														scale
		//------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Sets the object's x-axis and y-axis scale
		/// Note: This getter/setter is included for convenience only
		/// Reading this value will return scaleX, not scaleY!!
		/// </summary>
		/// <value>
		/// The scale.
		/// </value>
		public float scale {
			get { 
				return transform.m_scale.x; 
			}
			set { 
				transform.m_scale.x = value;
				transform.m_scale.y = value; 
			}
		}

		/// <summary>
		/// Returns the inverse matrix transformation, if it exists.
		/// (Use this e.g. for cameras used by sub windows)
		/// </summary>
		public Transformable Inverse() {
			Transformable inv=new Transformable();
			if (scaleX == 0 || scaleY == 0)
				throw new Exception ("Cannot invert a transform with scale 0");
			float cs = transform.m_matrix [0];
			float sn = transform.m_matrix [1];
			inv.transform.m_matrix [0] = cs / scaleX;
			inv.transform.m_matrix [1] = -sn / scaleY;
			inv.transform.m_matrix [4] = sn / scaleX;
			inv.transform.m_matrix [5] = cs / scaleY;
			inv.x = (-x * cs - y * sn) / scaleX;
			inv.y = (x * sn - y * cs) / scaleY;
			return inv;
		}
	}
}


