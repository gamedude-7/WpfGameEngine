using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp2;
using GameEngine.Classes;
using GUI;

namespace GameEngine
{
    enum OBJECT4DV1_STATE
    {
        // states for objects
        ACTIVE = 0x0001,
        VISIBLE = 0x0002,
        CULLED = 0x0003
    }

    enum PARM_LINE
    {
        NO_INTERSECT,
        LINE_INTERSECT_IN_SEGMENT,
        LINE_INTERSECT_OUT_SEGMENT
    }

    enum PLX
    {

        // bit masks to simplify testing????
        _RGB_MASK = 0x8000,   // mask to extract RGB or indexed color
        _SHADE_MODE_MASK = 0x6000,   // mask to extract shading mode
        _2SIDED_MASK = 0x1000,   // mask for double sided
        _COLOR_MASK = 0x0fff,   // xxxxrrrrggggbbbb, 4-bits per channel RGB
                                // xxxxxxxxiiiiiiii, indexed mode 8-bit index

        // these are the comparision flags after masking
        // color mode of polygon
        _COLOR_MODE_RGB_FLAG = 0x8000,
        _COLOR_MODE_INDEXED_FLAG = 0x0000,

        // double/single sided flag
        _2SIDED_FLAG = 0x1000,
        _1SIDED_FLAG = 0x0000,

        // shading mode of polygon
        _SHADE_MODE_PURE_FLAG = 0x0000, // this poly is a constant color
        _SHADE_MODE_CONSTANT_FLAG = 0x0000,  // alias
        _SHADE_MODE_FLAT_FLAG = 0x2000,  // this poly uses flat shading
        _SHADE_MODE_GOURAUD_FLAG = 0x4000,  // this poly used gouraud shading
        _SHADE_MODE_PHONG_FLAG = 0x6000,  // this poly uses phong shading
        _SHADE_MODE_FASTPHONG_FLAG = 0x6000  // this poly uses phong shading (alias)                             
    }

    enum POLY4DV1_STATE
    {
        // states of polygons and faces
        _ACTIVE = 0x0001,
        _CLIPPED = 0x0002,
        _BACKFACE = 0x0004,
    }
    enum POLY4DV1_ATTR
    {


        _2SIDED = 0x0001,
        _TRANSPARENT = 0x0002,
        _8BITCOLOR = 0x0004,
        _RGB16 = 0x0008,
        _RGB24 = 0x0010,

        _SHADE_MODE_PURE = 0x0020,
        _SHADE_MODE_CONSTANT = 0x0020, // (alias)
        _SHADE_MODE_FLAT = 0x0040,
        _SHADE_MODE_GOURAUD = 0x0080,
        _SHADE_MODE_PHONG = 0x0100,
        _SHADE_MODE_FASTPHONG = 0x0100, // (alias)
        _SHADE_MODE_TEXTURE = 0x0200
    }


    // a polygon based on a vertex list
    public class POLY4DV1
    {
        public int state;    // state information
        public int attr;      // physical attributes of polygon
                                //public int color;    // color of polygon
        public Color color;

        //Point4D* vlist; // the vertex list itself                   
        public int poly_num_verts = 3; // number of vertices for current poly ( 3 by default)
        public unsafe void setVertexList(Point4D[] p)
        {
            fixed (Point4D* vlist = p)
            {

            }
        }

        public int[] vert = new int[3];      // the indices into the vertex list

    }

    public struct Point4D
    {
        float x, y, z, w;

        public Point4D(float x, float y, float z, float w)
        {
            this.x = x; this.y = y; this.z = z; this.w = w;
        }

        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public float Z { get { return z; } set { z = value; } }
        public float W { get { return w; } set { w = value; } }
    };

    // class for objects that are composed of a number of polygons
    public class OBJECT4DV1
    {

        private const int OBJECT4DV1_MAX_VERTICES = 40256;//64;
        private const int OBJECT4DV1_MAX_POLYS = 912;//128;

        public int id;                //numeric id of this object
        public string name; //ASCII name of object 
        public string type;
        public string scriptName;
        int state;           //state of object
        int attr;              // attributes of object

        float avg_radius; // average radius of object used for collision detection
        float max_radius;//maximum radius of object

        public Point4D world_pos;  // position of object in world
        public Vector4 dir = new Vector4();         // rotation angles of object in world
                                                    // cords or unit direction vector user defined ???

        Vector4 ux, uy, uz;  //local axes to track full orientation
                            //this is updated automatically during rotation calls

        public Vector4 scale = new Vector4(1, 1, 1, 1);

        public int num_vertices;    // number of verticesof this object

        public Point4D[] vlist_trans = new Point4D[OBJECT4DV1_MAX_VERTICES];    // array of transformed vertices
        public Point4D[] vlist_local = new Point4D[OBJECT4DV1_MAX_VERTICES];    // array of local vertices

        public int num_polys;   // number of polygons in object mesh
        public POLY4DV1[] plist = new POLY4DV1[OBJECT4DV1_MAX_POLYS];  // array of polygons       

        public float X
        {
            get
            {
                return world_pos.X;
            }
            set
            {
                world_pos.X = value;
            }
        }

        public float Y
        {
            get
            {
                return world_pos.Y;
            }
            set
            {
                world_pos.Y = value;
            }
        }

        public float Z
        {
            get
            {
                return world_pos.Z;
            }
            set
            {
                world_pos.Z = value;
            }
        }

        public float Pitch
        {
            get
            {
                return dir.X;
            }
            set
            {
                dir.X = value;
            }
        }

        public float Yaw
        {
            get
            {
                return dir.Y;
            }
            set
            {
                dir.Y = value;
            }
        }

        public float Roll
        {
            get
            {
                return dir.Z;
            }
            set
            {
                dir.Z = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string Script
        {
            get
            {
                return scriptName;
            }
            set
            {
                scriptName = value;
            }
        }
        public void SET_BIT(int word, int bit_flag)
        {
            word = word | bit_flag;
        }

        //USHORT (*RGB16Bit)(int r, int g, int b) = NULL;
        // public delegate ushort RGB16Bit(int r, int g, int b);

        /*public ushort RGB16Bit565(int r, int g, int b)
        {
            // this function simply builds a 5.6.5 format 16 bit pixel
            // assumes input is RGB 0-255 each channel
            r >>= 3; g >>= 2; b >>= 3;
            return (_RGB16BIT565((r), (g), (b)));

        } // end RGB16Bit565

        /**
            Updates this object to the properties of a triangle
            */

        public OBJECT4DV1()
        {
            scale = new Vector4(1, 1, 1, 1);
        }

        public void setToTriangle(Point4D[] vertices)
        {
            vlist_trans = vertices;
            num_polys = 1;
            num_vertices = vertices.Length;
        }

        public virtual Triangle[] ConvertToTriangles()
        {
            Debug.Print("This should not be shown...");
            return new Triangle[] { };
        }

        string Get_Line_PLG(ref string buffer, int maxlength, ref StreamReader rdr)
        {
            // this little helper function simply read past comments 
            // and blank lines in a PLG file and always returns full 
            // lines with something on them on NULL if the file is empty

            //StreamReader rdr= new StreamReader(fs);

            int index = 0;  // general index
            int length = 0; // general length

            // enter into parsing loop
            while (true)
            {
                // read the next line
                buffer = rdr.ReadLine();
                if (buffer == null)
                    return null;

                // kill the whitespace
                for (index = 0; index < buffer.Length && buffer[index] == ' '; index++) ;

                // test if this was a blank line 
                length = buffer.Length;
                if (index >= length)
                    continue;

                // test if this was a comment
                if (buffer[index] == '#')
                    continue;


                // at this point we have a good line

                return buffer.Substring(index);
            } // end while

        } // end Get_Line_PLG

        public float Compute_Radius()
        {
            // this function computes the average and maximum radius for 
            // sent object and opdates the object data

            // reset incase there's any residue
            avg_radius = 0;
            max_radius = 0;

            // loop thru and compute radius
            for (int vertex = 0; vertex < num_vertices; vertex++)
            {
                // update the average and maximum radius
                float dist_to_vertex =
                        (float)Math.Sqrt((double)(vlist_local[vertex].X * vlist_local[vertex].X +
                            vlist_local[vertex].Y * vlist_local[vertex].Y +
                            vlist_local[vertex].Z * vlist_local[vertex].Z));

                // accumulate total radius
                avg_radius += dist_to_vertex;

                // update maximum radius   
                if (dist_to_vertex > max_radius)
                    max_radius = dist_to_vertex;

            } // end for vertex

            // finallize average radius computation
            avg_radius /= num_vertices;

            // return max radius
            return (max_radius);

        } // end Compute_OBJECT4DV1_Radius

        public int Load_PLY(ref string filename, Vector scale, Vector pos, Vector rot)
        {
            // this function loads a ply object in off disk, additionally it allows the caller to scale, position, and rotate the object to save extra calls later for 
            // non-dynamic objects

            FileStream fs; // file stream
            string buffer = ""; // working buffer
            string[] token;
            string token_string; // pointer to actual token text, ready for parsing
            StreamReader rdr;

            if ((fs = File.Open(filename, FileMode.Open, FileAccess.Read)) == null)
            {
                Debug.WriteLine("Couldn't open PLY file " + filename);
                return (0);
            } // end if

            Header header = new Header();
            PlyReader plyReader = new PlyReader(fs);
            header = plyReader.ReadHeader();

            // Access the elements (e.g., vertices and faces)

            List<Vertex> vertices = (List<Vertex>)header.Elements["vertex"];
            //List<Face> faces = (List<Face>)header.Elements["face"];

            num_vertices = header.vertexCount;
            // Read and process vertices
            for (int i = 0; i < header.vertexCount; i++)
            {
                Vertex vertex = plyReader.ReadVertices(i);

                //Console.WriteLine($"Vertex {i + 1}: X={vertex.X}, Y={vertex.Y}, Z={vertex.Z}");
                vertices.Add(vertex);
                vlist_local[i] = new Point4D();
                vlist_local[i].X = vertex.X;
                vlist_local[i].Y = vertex.Y;
                vlist_local[i].Z = vertex.Z;
            }

            int poly_num_verts = 0; // number of vertices for current poly (always 3)
                                    // Read and process faces
            num_polys = header.faceCount;

            for (int poly = 0; poly < num_polys; poly++)
            {
                Face face = plyReader.ReadFaces(poly);
                poly_num_verts = face.VertexCount;
                plist[poly] = new POLY4DV1();
                plist[poly].poly_num_verts = poly_num_verts;
                plist[poly].vert = new int[poly_num_verts];
                for (int i = 0; i < face.VertexCount; i++)
                {
                    plist[poly].vert[i] = Convert.ToInt32(face.VertexIndices[i]);
                }
            }
            fs.Close();
            // return success
            return (1);
        }

        public int Load_PLG(ref string filename, Vector4 scale, Vector4 pos, Vector4 rot)
        {
            // this function loads a plg object in off disk, additionally it allows the caller to scale, position, and rotate the object to save extra calls later for 
            // non-dynamic objects

            FileStream fs; // file stream
            string buffer = ""; // working buffer
            string[] token;
            string token_string; // pointer to actual token text, ready for parsing
            StreamReader rdr;

            // file format review, note types at end of each description
            // # this is a comment

            // # object descriptor
            // object name_string num_verts num_polys_int

            // # vertex list
            // x0_float y0_float z0_float
            // x1_float y1_float z1_float
            // x2_float y2_float z2_float
            // .
            // .
            // xn_float yn_float zn_float
            //
            // # polygon list
            // surface_description_ushort num_verts_int v0_index_int v1_index_int .
            // vn_index_int
            // .
            // .
            // surface_description_ushort num_verts_int v0_index_int v1_index_int .
            // vn_index_int

            // lets keep it simple and assume one element per line
            // hence we have to find the object descriptor, read it in, then the 
            // vertex list and read it in, and finally the polygon list

            // Step 1: clear out the active and visible
            // set state of object to active and visible
            state = (int)(OBJECT4DV1_STATE.ACTIVE | OBJECT4DV1_STATE.VISIBLE);

            // set position of object
            world_pos.X = pos.x;
            world_pos.Y = pos.y;
            world_pos.Z = pos.z;
            world_pos.W = pos.w;

            // Step 2: open the file for reading

            if ((fs = File.Open(filename, FileMode.Open, FileAccess.Read)) == null)
            {
                Debug.WriteLine("Couldn't open PLG file " + filename);
                return (0);
            } // end if

            rdr = new StreamReader(fs);
            // Step 3: get the first token string which should be the object descriptor
            if ((token_string = Get_Line_PLG(ref buffer, 255, ref rdr)) == null)
            {
                Debug.WriteLine("PLG file error with file " + filename + "(object descriptor invalid).");
                return (0);
            } // end if

            Debug.WriteLine("Object Descriptor:  " + token_string);

            // parse out the info object               
            token = token_string.Split(new Char[] { ' ' });
            name = token[0];
            num_vertices = int.Parse(token[1]);
            num_polys = int.Parse(token[2]);

            // Step 4: load the vertex list
            for (int vertex = 0; vertex < this.num_vertices; vertex++)
            {
                // get the next vertex
                if ((token_string = Get_Line_PLG(ref buffer, 255, ref rdr)) == null)
                {
                    Debug.WriteLine("PLG file error with file " + filename + " (vertex list invalid).");
                    return (0);
                } // end if

                // parse out vertex
                token = token_string.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                vlist_local[vertex].X = float.Parse(token[0]);
                vlist_local[vertex].Y = float.Parse(token[1]);
                vlist_local[vertex].Z = float.Parse(token[2]);
                vlist_local[vertex].W = 1;

                // scale vertices
                vlist_local[vertex].X *= scale.x;
                vlist_local[vertex].Y *= scale.y;
                vlist_local[vertex].Z *= scale.z;

                Debug.WriteLine("\nVertex " + vertex + "= " +
                                                    vlist_local[vertex].X + " " +
                                                    vlist_local[vertex].Y + " " +
                                                    vlist_local[vertex].Z + " " +
                                                    vlist_local[vertex].W);


            } // end for vertex

            // compute average and max radius
            Compute_Radius();

            Debug.WriteLine("\nObject average radius = " + avg_radius + ", max radius = " + max_radius);


            int poly_surface_desc = 0; // PLG/PLX surface descriptor
            int poly_num_verts = 0; // number of vertices for current poly (always 3)
            string tmp_string;        // temp string to hold surface descriptor in and
                                        // test if it need to be converted from hex

            // Step 5: load the polygon list
            for (int poly = 0; poly < num_polys; poly++)
            {
                // get the next polygon descriptor
                if ((token_string = Get_Line_PLG(ref buffer, 255, ref rdr)) == null)
                {
                    Debug.WriteLine("PLG file error with file " + filename + " (polygon descriptor invalid).");
                    return (0);
                } // end if

                Debug.WriteLine("\nPolygon " + poly + ":");

                // each vertex list MUST have 3 vertices since we made this a rule that all models
                // must be constructed of triangles
                // read in surface descriptor, number of vertices, and vertex list
                token = token_string.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                tmp_string = token[0]; // surface descriptor

                plist[poly] = new POLY4DV1();
                plist[poly].poly_num_verts = int.Parse(token[1]);
                plist[poly].vert[0] = int.Parse(token[2]);
                plist[poly].vert[1] = int.Parse(token[3]);
                plist[poly].vert[2] = int.Parse(token[4]);
                /*sscanf(token_string, "%s %d %d %d %d", tmp_string,
                                                        &poly_num_verts, // should always be 3 
                                                        &plist[poly].vert[0],
                                                        &plist[poly].vert[1],
                                                        &plist[poly].vert[2]);
                */

                // since we are allowing the surface descriptor to be in hex format
                // with a leading "0x" we need to test for it
                if (tmp_string[0] == '0' && Char.ToUpper(tmp_string[1]) == 'X')
                    poly_surface_desc = Convert.ToInt32(tmp_string, 16);//sscanf(tmp_string,"%x", &poly_surface_desc);                
                else
                    poly_surface_desc = int.Parse(tmp_string);

                // point polygon vertex list to object's vertex list
                // note that this is redundant since the polylist is contained
                // within the object in this case and its up to the user to select
                // whether the local or transformed vertex list is used when building up
                // polygon geometry, might be a better idea to set to NULL in the context
                // of polygons that are part of an object

                unsafe
                {
                    plist[poly].setVertexList(vlist_local);
                    //plist[poly].vlist = vlist_local;
                }

                Debug.WriteLine("Surface Desc = 0x" + poly_surface_desc + ", num_verts = " + poly_num_verts +
                                        ", vert_indices [" + plist[poly].vert[0] + ", " + plist[poly].vert[1] + ", " + plist[poly].vert[2] + "]");




                // now we that we have the vertex list and we have entered the polygon
                // vertex index data into the polygon itself, now let's analyze the surface
                // descriptor and set the fields for the polygon based on the description

                // extract out each field of data from the surface descriptor
                // first let's get the single/double sided stuff out of the way
                if ((poly_surface_desc & Convert.ToInt32(PLX._2SIDED_FLAG)) > 0)
                {
                    // double sided
                    SET_BIT(plist[poly].attr, Convert.ToInt32(POLY4DV1_ATTR._2SIDED));
                    Debug.WriteLine("2 sided");
                } // end if
                else
                {
                    // one sided
                    Debug.WriteLine("1 sided");
                } // end else

                // now let's set the color type and color
                if ((poly_surface_desc & Convert.ToInt32(PLX._COLOR_MODE_RGB_FLAG)) > 0)
                {
                    // this is an RGB 4.4.4 surface
                    SET_BIT(plist[poly].attr, Convert.ToInt32(POLY4DV1_ATTR._RGB16));

                    // now extract color and copy into polygon color
                    // field in proper 16-bit format 
                    // 0x0RGB is the format, 4 bits per pixel 
                    int red = ((poly_surface_desc & 0x0f00) >> 8);
                    int green = ((poly_surface_desc & 0x00f0) >> 4);
                    int blue = (poly_surface_desc & 0x000f);

                    // although the data is always in 4.4.4 format, the graphics card
                    // is either 5.5.5 or 5.6.5, but our virtual color system translates
                    // 8.8.8 into 5.5.5 or 5.6.5 for us, but we have to first scale all
                    // these 4.4.4 values into 8.8.8
                    plist[poly].color = Colors.Black;//RGB16Bit(red*16, green*16, blue*16);

                    Debug.WriteLine("RGB color = [" + red + ", " + green + ", " + blue + "]");
                } // end if
                else
                {
                    // this is an 8-bit color indexed surface
                    SET_BIT(plist[poly].attr, Convert.ToInt32(POLY4DV1_ATTR._8BITCOLOR));

                    // and simple extract the last 8 bits and that's the color index
                    plist[poly].color = Colors.Black;// (poly_surface_desc & 0x00ff);

                    Debug.WriteLine("8-bit color index = " + plist[poly].color);

                } // end else

                // handle shading mode
                int shade_mode = (poly_surface_desc & Convert.ToInt32(PLX._SHADE_MODE_MASK));

                // set polygon shading mode
                switch (shade_mode)
                {
                    case 0x0000:
                        {//PLX._SHADE_MODE_PURE_FLAG
                            SET_BIT(plist[poly].attr, Convert.ToInt32(POLY4DV1_ATTR._SHADE_MODE_PURE));
                            Debug.WriteLine("Shade mode = pure");
                        }
                        break;

                    case 0x2000:
                        {// PLX._SHADE_MODE_FLAT_FLAG
                            SET_BIT(plist[poly].attr, Convert.ToInt32(POLY4DV1_ATTR._SHADE_MODE_FLAT));
                            Debug.WriteLine("Shade mode = flat");

                        }
                        break;

                    case 0x4000:
                        { //PLX._SHADE_MODE_GOURAUD_FLAG
                            SET_BIT(plist[poly].attr, Convert.ToInt32(POLY4DV1_ATTR._SHADE_MODE_GOURAUD));
                            Debug.WriteLine("Shade mode = gouraud");
                        }
                        break;

                    case 0x8000:
                        {//PLX_SHADE_MODE_PHONG_FLAG
                            SET_BIT(plist[poly].attr, Convert.ToInt32(POLY4DV1_ATTR._SHADE_MODE_PHONG));
                            Debug.WriteLine("Shade mode = phong");
                        }
                        break;

                    default: break;
                } // end switch

                // finally set the polygon to active
                plist[poly].state = Convert.ToInt32(POLY4DV1_STATE._ACTIVE);

            } // end for poly

            // close the file            
            fs.Close();

            // return success
            return (1);


        }
    }

    public class Triangle : OBJECT4DV1
    {
        /* nothing in here.  This class is just to basically 
         state that this is a specific kind of polygon that can only have 3 sides/vertices.
             */
        public Triangle(Point4D v1, Point4D v2, Point4D v3)
        {
            setToTriangle(new Point4D[] { v1, v2, v3 });
        }
    }

    class Cube : OBJECT4DV1
    {
        public float width, height, length;

        public Cube() : base()
        {
            base.num_vertices = 8;
            //  base.num_polys = 6;
        }

        public override Triangle[] ConvertToTriangles()
        {
            Debug.Print("cube convert to triangles caled... YAY!!");

            int[] offsets = new int[]
                 {0,1,2,3, // front
                 0,3,7,4, // left side
                 0,4,5,1, // top
                 4,5,6,7, // back
                 3,2,6,7, // bottom
                 1,2,6,5 // right side
                 };

            Point4D[] vertices = new Point4D[8];
            for (int i = 0; i < 8; i++)
                vertices[i] = vlist_trans[i];

            Triangle[] result = new Triangle[6 * 2];

            // loop through the square sides of the cube
            for (int side = 0; side < 6; side++)
            {
                // loop through triangles on each side
                for (int v = 2; v < 4; v++)
                {
                    int offset = side * 4;
                    result[side * 2 + (v - 2)] =
                        new Triangle(
                        vertices[offsets[offset]], vertices[offsets[offset + v - 1]], vertices[offsets[offset + v]]);
                }
            }
            return result;
        }

    }

    class CAM4DV1
    {
        int state;   // state of camera
        int attr;      // camera attributes

        public Point4D pos;     // world position of camera used by both camera models

        public Vector4 dir;   // euler agles or look at direction of camera for UVN

        Vector4 u;     // extra vectors to track the camera orientation
        Vector4 v;      // for more complex UVN camera model
        Vector4 n;
        Point4D target;  // target or "look at" position for UVN model

        float view_dist_h;    // horizontal and vertical viewing distances
        float view_dist_v;

        float fov;      // field of view for both horizontal and vertical axes

        // 3d clipping planes
        // if view volume is NOT 90 degree then general 3d clipping
        // must be employed
        float near_clip_z;        // near z=constant clipping plane
        float far_clip_z;          // far z=constant clipping plane

        //UnsafeNativeMethods.Plane rt_clip_plane;   // the right clipping plane
        //UnsafeNativeMethods.Plane lt_clip_plane;    // the left clipping plane
        //UnsafeNativeMethods.Plane tp_clip_plane;   // top clipping plane
        //UnsafeNativeMethods.Plane bt_clip_plane;   // the bottom clipping plane

        float viewplane_width;     // width and height of view plane to project onto
        float viewplane_height;    // usually 2x2 for normalized projection or the exact
                                   // same size as the viewport or screen window

        // remember screen and viewport are synonomous
        float viewport_width;   // size of screen/viewport
        float viewport_height;
        float viewport_center_x;  // center of view port (final image destination)
        float viewport_center_y;

        // aspect ratio
        float aspect_ratio;   // screen import width/height

        // these matrices are not necessarily needed based on the method of 
        // transformation, for example, a manual perspective or screen transform
        // and or a concatenated perspective/screen, however, having these
        // matrices give us more flexibility

        CortexMatrix mcam;    // storage for the world to camera transform matrix
        CortexMatrix mper;      // storage for the camera to perspective transform matrix
        CortexMatrix mscr;      // storage for the perspective to screen transform matrix

        public void Init_CAM4DV1(Point4D cam_pos,   // initial camera position
                                            Vector4 cam_dir,       // initial camera angles
                                            float fov,                    // field of view
                                            float viewport_width,   // size of final screen viewport
                                            float viewport_height)
        {
            pos = cam_pos;
            dir = cam_dir;
            this.fov = fov;
            this.viewport_width = viewport_width;
            this.viewport_height = viewport_height;
        }
    }
}
