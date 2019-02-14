using System;
using static GLXEngine.Mathf;

namespace GLXEngine.Core
{
    public struct Vector2
    {
        public float x;
        public float y;

        //------------------------------------------------------------------------------------------------------------------------
        //														Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public Vector2(float a_angle)
        {
            x = Cos(a_angle * PI / 180.0f);
            y = Sin(a_angle * PI / 180.0f);
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(Vector2 source)
        {
            this.x = source.x;
            this.y = source.y;
        }
        public Vector2(Vector2i source)
        {
            this.x = source.x;
            this.y = source.y;
        }


        //------------------------------------------------------------------------------------------------------------------------
        //												    Randomised vectors
        //------------------------------------------------------------------------------------------------------------------------
        public static Vector2 Random(float a_minX, float a_maxX, float a_minY, float a_maxY)
        {   return new Vector2(Utils.Random(a_minX, a_maxX), Utils.Random(a_minY, a_maxY)); }

        public static Vector2 Random(float a_minValue, float a_maxValue)
        {   return new Vector2(Utils.Random(a_minValue, a_maxValue), Utils.Random(a_minValue, a_maxValue)); }

        public static Vector2 RandomWithLength(float a_minLength, float a_maxLength)
        {   return new Vector2(Utils.Random(0f, 1f), Utils.Random(0f, 1f)).SetMagnitude(Utils.Random(a_minLength, a_maxLength));    }

        public static Vector2 RandomFromAngle(float a_minAngle, float a_maxAngle)
        {   return new Vector2(Utils.Random(a_minAngle, a_maxAngle));   }
       

        //------------------------------------------------------------------------------------------------------------------------
        //												        ToString()
        //------------------------------------------------------------------------------------------------------------------------
        override public string ToString()    {   return "[Vector2 " + x + ", " + y + "]";    }


        //------------------------------------------------------------------------------------------------------------------------
        //												   Secundary accessors
        //------------------------------------------------------------------------------------------------------------------------
        public float magnitude
        {
            get {   return Sqrt(x * x + y * y); }
            set {   float curAngle  = angle * PI / 180f;    x = Cos(curAngle) * value;  y = Sin(curAngle) * value;  }
        }
        public float sqrMagnitude
        {
            get {   return x * x + y * y;   }
        }
        public float angle
        {
            get
            {
                if (magnitude != 0)
                {   float temp =  Acos(Dot(new Vector2(1, 0)) / magnitude) * 180f / PI; if(y < 0)   temp *= -1f;    return temp;  }
                else
                    return 0;
            }
            set {   float curMagnitude = magnitude; x = Cos(value * PI / 180.0f) * curMagnitude;   y = Sin(value * PI / 180.0f) * curMagnitude;   }
        }
        public Vector2 normal {   get { return x != 0 || y != 0 ? this / magnitude : new Vector2(); } }


        //------------------------------------------------------------------------------------------------------------------------
        //												   Vector functions
        //------------------------------------------------------------------------------------------------------------------------
        public float Dot(Vector2 b)                         {   return x * b.x + y * b.y;       }
        public static float Distance(Vector2 a, Vector2 b)  {   return Abs((a - b).magnitude);  }
        public Vector2 Normalize()                          {   return this /= magnitude;       }
        public Vector2 SetMagnitude(float a_newMag)         {   this.magnitude = a_newMag; return this; }
        public Vector2 SetAngle(float a_newAngle)           {   this.angle = a_newAngle; return this; }

        public override bool Equals(object obj)
        {
            return this == (Vector2)(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //												      Operators
        //------------------------------------------------------------------------------------------------------------------------
        public static implicit operator float[] (Vector2 a) {   return new float[] { a.x, a.y };    }
        public static implicit operator Vector2(float[] a)
        {   if (a.Length < 2) throw new Exception("Float array has fewer numbers than the array.");     else  return new Vector2(a[0], a[1]); }

        public static implicit operator Vector2i (Vector2 a) {   return new Vector2i(a);    }
        public static implicit operator Vector2 (Vector2i a) {   return new Vector2(a);     }

        public static Vector2 operator -(Vector2 a, Vector2 b)  {   return new Vector2(a.x - b.x, a.y - b.y);   }
        public static Vector2 operator +(Vector2 a, Vector2 b)  {   return new Vector2(a.x + b.x, a.y + b.y);   }
        public static Vector2 operator *(Vector2 a, Vector2 b)  {   return new Vector2(a.x * b.x, a.y * b.y);   }
        public static Vector2 operator /(Vector2 a, Vector2 b)  {   return new Vector2((b.x == 0 ? 0 : (a.x / b.x)), (b.y == 0 ? 0 : (a.y / b.y))); }

        public static Vector2 operator -(Vector2 a)  {   return new Vector2(-a.x , -a.y);   }

        public static Vector2 operator *(Vector2 a, float b)    {   return new Vector2(a.x * b, a.y * b);       }
        public static Vector2 operator *(float a, Vector2 b)    {   return new Vector2(b.x * a, b.y * a);       }
        public static Vector2 operator /(Vector2 a, float b)    {return new Vector2((b == 0 ? 0 : (a.x / b)), (b == 0 ? 0 : (a.y / b)));}

        public static bool operator ==(Vector2 a, Vector2 b)    {   return a.x <= b.x + 0.0001f && a.x >= b.x - 0.0001f && a.y <= b.y + 0.0001f && a.y >= b.y - 0.0001f; }
        public static bool operator !=(Vector2 a, Vector2 b)    {   return !(a == b); }


        //------------------------------------------------------------------------------------------------------------------------
        //												   Interpolation
        //------------------------------------------------------------------------------------------------------------------------
        public Vector2 Lerp(Vector2 a_target, float a_scalar)   {   return this += (a_target-this)*a_scalar;    }
        public static Vector2 Lerp(Vector2 a_source, Vector2 a_target, float a_scalar) {    return a_source + (a_target-a_source)*a_scalar; }
        public Vector2 Slerp(Vector2 a_target, float a_scalar)  {   this.angle += (a_target.angle - this.angle)*a_scalar;   return this;    }
        public static Vector2 Slerp(Vector2 a_source, Vector2 a_target, float a_scalar) {   Vector2 temp = new Vector2(a_source);   temp.angle += (a_target.angle - a_source.angle)*a_scalar;   return temp;}
    }




    //------------------------------------------------------------------------------------------------------------------------
    //														Vector2i
    //------------------------------------------------------------------------------------------------------------------------
    public struct Vector2i
    {
        public int x;
        public int y;

        public Vector2i(float a_angle)
        {
            x = Round(Cos(a_angle * PI / 180.0f));
            y = Round(Sin(a_angle * PI / 180.0f));
        }

        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2i(Vector2i source)
        {
            this.x = source.x;
            this.y = source.y;
        }

        public Vector2i(Vector2 source)
        {
            this.x = Round(source.x);
            this.y = Round(source.y);
        }
       
        override public string ToString()
        {
            return "[Vector2 " + x + ", " + y + "]";
        }

         public float magnitude
        {
            get {   return Sqrt(x * x + y * y); }
            set {   float curAngle  = angle * PI / 180f;    x = Round(Cos(curAngle) * value);  y = Round(Sin(curAngle) * value);  }
        }
        public int sqrMagnitude
        {
            get {   return x * x + y * y;   }
        }
        public float angle
        {
            get
            {
                if (magnitude != 0)
                {   float temp =  Acos(Dot(new Vector2(1, 0)) / magnitude) * 180f / PI; if(y < 0)   temp *= -1f;    return temp;  }
                else
                    return 0;
            }
            set {   float curMagnitude = magnitude; x = Round(Cos(value * PI / 180.0f) * curMagnitude);   y = Round(Sin(value * PI / 180.0f) * curMagnitude);   }
        }
        public Vector2i normal {   get { return x != 0 || y != 0 ? this / magnitude : new Vector2i(); } }


        public float Dot(Vector2 b)                         {   return x * b.x + y * b.y;       }
        public static float Distance(Vector2 a, Vector2 b)  {   return Abs((a - b).magnitude);  }
        public Vector2i Normalize()                          {   return this /= magnitude;       }
        public Vector2i SetMagnitude(float a_newMag)         {   this.magnitude = a_newMag; return this; }
        public Vector2i SetAngle(float a_newAngle)           {   this.angle = a_newAngle; return this; }

        public override bool Equals(object obj)
        {
            return this == (Vector2i)(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator int[] (Vector2i a) {   return new int[] { a.x, a.y };    }
        public static implicit operator Vector2i(int[] a)
        {   if (a.Length < 2) throw new Exception("Float array has fewer numbers than the array.");     else  return new Vector2i(a[0], a[1]); }
 
        public static implicit operator Vector2i (Vector2 a) {   return new Vector2i(a);    }
        public static implicit operator Vector2 (Vector2i a) {   return new Vector2(a);     }

        public static Vector2i operator -(Vector2i a, Vector2i b)  {   return new Vector2i(a.x - b.x, a.y - b.y);   }
        public static Vector2i operator +(Vector2i a, Vector2i b)  {   return new Vector2i(a.x + b.x, a.y + b.y);   }
        public static Vector2i operator *(Vector2i a, Vector2i b)  {   return new Vector2i(a.x * b.x, a.y * b.y);   }
        public static Vector2i operator /(Vector2i a, Vector2i b)  {   return new Vector2i(Round(b.x == 0 ? 0 : (a.x / b.x)), Round(b.y == 0 ? 0 : (a.y / b.y))); }

        public static Vector2i operator *(Vector2i a, float b)    {   return new Vector2i(Round(a.x * b), Round(a.y * b));       }
        public static Vector2i operator *(float a, Vector2i b)    {   return new Vector2i(Round(b.x * a), Round(b.y * a));       }
        public static Vector2i operator /(Vector2i a, float b)    {return new Vector2i(Round(b == 0 ? 0 : (a.x / b)), Round(b == 0 ? 0 : (a.y / b)));}

        public static bool operator ==(Vector2i a, Vector2i b)    {   return a.x == b.x&& a.y == b.y; }
        public static bool operator !=(Vector2i a, Vector2i b)    {   return !(a == b); }

        public Vector2i Lerp(Vector2i a_target, float a_scalar)   {   return this += (a_target-this)*a_scalar;    }
        public static Vector2i Lerp(Vector2i a_source, Vector2i a_target, float a_scalar) {    return a_source + (a_target-a_source)*a_scalar; }
        public Vector2i Slerp(Vector2i a_target, float a_scalar)  {   this.angle += (a_target.angle - this.angle)*a_scalar;   return this;    }
        public static Vector2i Slerp(Vector2i a_source, Vector2i a_target, float a_scalar) {   Vector2i temp = new Vector2i(a_source);   temp.angle += (a_target.angle - a_source.angle)*a_scalar;   return temp;}
    }
}

