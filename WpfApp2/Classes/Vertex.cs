using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.Classes
{
    public class Vertex
    {
        // private double x;
        //[ElementProperty(0)]
        public float X { get; set; }

        public Vertex(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        /* public double getX()
         { return x; }
        public double X
         {
             get { return x; }
             set { x = value; }
         }*/

        //[ElementProperty(1)]
        public float Y { get; set; }

        //[ElementProperty(2)]
        public float Z { get; set; }
    }  
}
