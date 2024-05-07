using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp2
{
   

    class Camera
    {
        float fov;      // field of view for both horizontal and vertical axes
        public Point4D pos;     // world position of camera used by both camera models
        public Vector3D dir;     // euler agles or look at direction of camera for UVN

        // remember screen and viewport are synonomous

        float viewport_width;   // size of screen/viewport
        float viewport_height;

        public Camera(Point4D cam_pos, Vector3D cam_dir, float fov, float viewport_width, float viewport_height)
        {
            this.pos = cam_pos;
            this.dir = cam_dir;
            this.fov = fov;
            this.viewport_width = viewport_width;
            this.viewport_height = viewport_height;
        }
    }


}
