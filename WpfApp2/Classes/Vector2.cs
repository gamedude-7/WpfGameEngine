using GameEngine.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Classes
{
    internal class Vector2
    {
        public float x, y, z, w;
        //Vector vect;
        public Vector2()
        {
            //vect = new Vector();
        }

        public Vector2(float _x, float _y)
        {
            //vect = new Vector(_x, _y, _z, _w);
        }

        float getX() { return x; }
        float getY() { return y; }

        void setX(float x_) { this.x = x_; }
        void setY(float y_) { this.y = y_; }


        public Vector2 setVector2(Vector2 vec)
        {
           
            Vector2 vect = new Vector2(vec.x,vec.y);
            vect.x = vec.x;
            vect.y = vec.y;
            vect.z = vec.z;
            vect.w = vec.w;
            
            return vect;
        }

        public static Vector2 operator+ (Vector2 vec, Vector2 vec2)
        {
            Vector2 vect = new Vector2();
            vect.x = vec.x + vec2.x;
            vect.y = vec.y + vec2.y;
            vect.z = vec.z + vec2.z;
            vect.w = vec.w + vec2.w;            
            return vect;
        }

        public static Vector2 operator -(Vector2 vec, Vector2 vec2)
        {
            Vector2 vect = new Vector2();
            vect.x = vec.x - vec2.x;
            vect.y = vec.y - vec2.y;
            vect.z = vec.z - vec2.z;
            vect.w = vec.w - vec2.w;            
            return vect;
        }

        public static Vector2 operator *(Vector2 vec, Vector2 vec2)
        {
            Vector2 vect = new Vector2();
            vect.x = vec.x * vec2.x;
            vect.y = vec.y * vec2.y;
            vect.z = vec.z * vec2.z;
            vect.w = vec.w * vec2.w;
            return vect;
        }

        public static Vector2 operator/(Vector2 vec, Vector2 vec2)
        {
            Vector2 vect = new Vector2();
            vect.x = vec.x / vec2.x;
            vect.y = vec.y / vec2.y;
            vect.z = vec.z / vec2.z;
            vect.w = vec.w / vec2.w;
            return vect;
        }

        public static bool operator == (Vector2 lhs, Vector2 rhs)
        {
            bool status = false;
            if (lhs.x == rhs.x && lhs.y == rhs.y) 
            {                         
                status = true;
            }
            return status;
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            bool status = false;
            if (lhs.x != rhs.x && lhs.y != rhs.y )
            {
                status = true;
            }
            return status;
        }

        public static bool operator <(Vector2 lhs, Vector2 rhs)
        {
            bool status = false;
            if (lhs.x < rhs.x && lhs.y < rhs.y )
            {
                status = true;
            }
            return status;
        }

        public static bool operator >(Vector2 lhs, Vector2 rhs)
        {
            bool status = false;
            if (lhs.x > rhs.x && lhs.y > rhs.y)
            {
                status = true;
            }
            return status;
        }

        public static bool operator <=(Vector2 lhs, Vector2 rhs)
        {
            bool status = false;
            if (lhs.x <= rhs.x && lhs.y <= rhs.y )
            {
                status = true;
            }
            return status;
        }

        public static bool operator >=(Vector2 lhs, Vector2 rhs)
        {
            bool status = false;
            if (lhs.x >= rhs.x && lhs.y >= rhs.y )
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
            return (float)Math.Sqrt(x * x + y * y);// sqrt(x * x + y * y + z * z);
        }
        float LengthSq()
        {
            return x * x + y * y;
        }
        float Det(Vector2 vec)
        {
            return x * vec.y - vec.x * y;
        }
        bool Equals(Vector2 vec)
        {
            return (x == vec.x && y == vec.y);
        }

        // normalize vector , doesn't return or save the length
        //void Normalize()
        //{
        //    float length = sqrt(x * x + y * y + z * z);
        //    if (length != 0)
        //    {
        //        x /= length;
        //        y /= length;
        //        z /= length;
        //    }
        //}

        //void Reverse()
        //{
        //    x = -x;
        //    y = -y;
        //}

        //float Angle() // returns in degrees
        //{
        //    float angle = (180.0 / 3.1415926) * atan2(-y, x);
        //    if (angle < 0) angle += 360;
        //    return angle;
        //}

        //float radianAngle()
        //{
        //    float angle = (3.1415926) * atan2(-y, x);
        //    if (angle < 0) angle += 2 * 3.14;
        //    return angle;
        //}
    }
}
