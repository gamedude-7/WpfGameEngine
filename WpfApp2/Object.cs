using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp2
{
    class Poloygon
    {
        public int state;    // state information

        public int attr;      // physical attributes of polygon

        public Color color;

        public unsafe void setVertexList(Point4D[] p)
        {

            fixed (Point4D* vlist = p)
            {

            }

        }

        public int[] vert = new int[3];      // the indices into the vertex list
    }

    public class Triangle : Object

    {

        /* nothing in here.  This class is just to basically 

         state that this is a specific kind of polygon that can only have 3 sides/vertices.

             */

        public Triangle(Point4D v1, Point4D v2, Point4D v3)

        {

            setToTriangle(new Point4D[] { v1, v2, v3 });

        }

    }

    public class Object
    {

        List<Point4D> vlist_trans = new List<Point4D>();  // list of transformed vertices
        List<Point4D> vlist_local = new List<Point4D>();  // array of local vertices
        List<Poloygon> plist = new List<Poloygon>();

        Point4D world_pos;      // position of object in world
        public Vector3D dir;         // rotation angles of object in world

        public Vector3D scale = new Vector3D(1, 1, 1);

        public int num_polys;   // number of polygons in object mesh

        public int num_vertices;    // number of verticesof this object
                                    /**

                                  Updates this object to the properties of a triangle

                                  */

        public void setToTriangle(Point4D[] vertices)

        {

            vlist_trans.Add(vertices[0]);
            vlist_trans.Add(vertices[1]);
            vlist_trans.Add(vertices[2]);

            num_polys = 1;

            num_vertices = vertices.Length;

        }



        public virtual Triangle[] ConvertToTriangles()

        {

            Debug.Print("This should not be shown...");

            return new Triangle[] { };

        }


    }
}
