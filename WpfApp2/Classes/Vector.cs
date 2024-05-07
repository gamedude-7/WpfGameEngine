using GameEngine.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Classes
{
    public class Vector4
    {
        public float x, y, z, w;
        //Vector vect;
        public Vector4()
        {
            //vect = new Vector();
        }

        public Vector4(float _x, float _y, float _z, float _w = 0)
        {
            x = _x; y = _y; z = _z; w = _w;
            //vect = new Vector(_x, _y, _z, _w);
        }

        public float X
        {
            get { 
                return x; 
            }
            set { 
                x = value; 
            }
        }

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public float Z
        {
            get
            {
                return z;
            }
            set
            {
                x = value;
            }
        }

        float getX() { return x; }
        float getY() { return y; }
        float getZ() { return z; }
        float getW() { return w; }

        void setX(float x_) { this.x = x_; }
        void setY(float y_) { this.y = y_; }
        void setZ(float z_) { this.z = z_; }
        void setW(float w_) { this.w = w_; }

        public Vector4 setVector(Vector4 vec)
        {
           
            Vector4 vect = new Vector4();
            vect.x = vec.x;
            vect.y = vec.y;
            vect.z = vec.z;
            vect.w = vec.w;
            
            return vect;
        }

        public static Vector4 operator+ (Vector4 vec, Vector4 vec2)
        {
            Vector4 vect = new Vector4();
            vect.x = vec.x + vec2.x;
            vect.y = vec.y + vec2.y;
            vect.z = vec.z + vec2.z;
            vect.w = vec.w + vec2.w;            
            return vect;
        }

        public static Vector4 operator -(Vector4 vec, Vector4 vec2)
        {
            Vector4 vect = new Vector4();
            vect.x = vec.x - vec2.x;
            vect.y = vec.y - vec2.y;
            vect.z = vec.z - vec2.z;
            vect.w = vec.w - vec2.w;            
            return vect;
        }

        public static Vector4 operator *(Vector4 vec, Vector4 vec2)
        {
            Vector4 vect = new Vector4();
            vect.x = vec.x * vec2.x;
            vect.y = vec.y * vec2.y;
            vect.z = vec.z * vec2.z;
            vect.w = vec.w * vec2.w;
            return vect;
        }

        public static Vector4 operator/(Vector4 vec, Vector4 vec2)
        {
            Vector4 vect = new Vector4();
            vect.x = vec.x / vec2.x;
            vect.y = vec.y / vec2.y;
            vect.z = vec.z / vec2.z;
            vect.w = vec.w / vec2.w;
            return vect;
        }

        public override bool Equals(object obj)
        {
            Vector4 vect = obj as Vector4;  
            return this == vect;
        }
        public static bool operator == (Vector4 lhs, Vector4 rhs)
        {
            bool status = false;
            if (lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.z == rhs.w) 
            {                         
                status = true;
            }
            return status;
        }

        public static bool operator !=(Vector4 lhs, Vector4 rhs)
        {
            bool status = false;
            if (lhs.x != rhs.x && lhs.y != rhs.y && lhs.z != rhs.z && lhs.w != rhs.w)
            {
                status = true;
            }
            return status;
        }

        public static bool operator <(Vector4 lhs, Vector4 rhs)
        {
            bool status = false;
            if (lhs.x < rhs.x && lhs.y < rhs.y && lhs.z < rhs.z && lhs.z < rhs.z && lhs.w < rhs.w)
            {
                status = true;
            }
            return status;
        }

        public static bool operator >(Vector4 lhs, Vector4 rhs)
        {
            bool status = false;
            if (lhs.x > rhs.x && lhs.y > rhs.y && lhs.z > rhs.z && lhs.z > rhs.z)
            {
                status = true;
            }
            return status;
        }

        public static bool operator <=(Vector4 lhs, Vector4 rhs)
        {
            bool status = false;
            if (lhs.x <= rhs.x && lhs.y <= rhs.y && lhs.z <= rhs.z && lhs.w <= rhs.w)
            {
                status = true;
            }
            return status;
        }

        public static bool operator >=(Vector4 lhs, Vector4 rhs)
        {
            bool status = false;
            if (lhs.x >= rhs.x && lhs.y >= rhs.y && lhs.z >= rhs.z && lhs.w >= rhs.w)
            {
                status = true;
            }
            return status;
        }
        //      Vector operator +(Vector &vec);
        //      Vector operator -(Vector &vec);
        //      Vector operator *(float scalar);
        //      Vector operator /(float scalar);

        //      Vector & operator+=(Vector &vec);
        //      Vector & operator-=(Vector &vec);
        //      Vector & operator*=(float scalar);
        //      Vector & operator/=(float scalar);
        //      void operator=(Vector &vec);
        //      float operator *(Vector &vec);
        //      bool operator ==(Vector &vec);
        //      bool operator !=(Vector &vec);
        //      float dot(Vector vec);
        //      Vector cross(Vector vec);

       /* Vector Perp()
        {
            return new Vector(y, -x);
        }
        float Distance(Vector &vec)
        {
            Vector dvec(vec.x - x, vec.y - y);
            return sqrt(dvec.x*dvec.x + dvec.y*dvec.y);
        }

        float DistanceSq(Vector &vec)
        {
            Vector dvec(vec.x - x, vec.y - y);
            return dvec.x*dvec.x + dvec.y*dvec.y;
        }*/
        float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);// sqrt(x * x + y * y + z * z);
        }
        float LengthSq()
        {
            return x * x + y * y + z *z;
        }
        float Det(Vector4 vec)
        {
            return x * vec.y - vec.x * y;
        }
        bool Equals(Vector4 vec)
        {
            return (x == vec.x && y == vec.y);
        }

        // normalize vector , doesn't return or save the length
        void Normalize()
        {
            float length = (float) Math.Sqrt(x * x + y * y + z * z);
            if (length != 0)
            {
                x /= length;
                y /= length;
                z /= length;
            }
        }

        //void Reverse()
        //{
        //    x = -x;
        //    y = -y;
        //}

        float Angle() // returns in degrees
        {
            float angle =(float)( (180.0 / 3.1415926) * Math.Atan2(-y, x) );
            if (angle < 0) angle += 360;
            return angle;
        }

        float radianAngle()
        {
            float angle = (float) ((3.1415926) * Math.Atan2(-y, x));
            if (angle < 0)
                angle += 2 * 3.14f;
            return angle;
        }
    } 
}
