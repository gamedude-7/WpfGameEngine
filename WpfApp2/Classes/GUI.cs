using GameEngine;
using System.Drawing;
using System.Diagnostics;

namespace GUI
{
    public class GUI
    {
        public static byte[] Draw_Line(int x0, int y0, int x1, int y1, byte color, byte[] image, int pitch)
        {
            int dx, dy, dx2, dy2, x_inc, y_inc, error, index = 0;

            index = 0;

            // precompute first pixel address in video buffer
            index = index + x0 + y0 * pitch;

            // delta values
            dx = x1 - x0;
            dy = y1 - y0;

            // test x component of slope
            if (dx >= 0)
            {
                x_inc = 1;
            }
            else
            {
                x_inc = -1;
                dx = -dx; // need absolute value
            }

            // test y component of slope
            if (dy >= 0)
            {
                y_inc = pitch;
            }
            else
            {
                y_inc = -pitch;
                dy = -dy; // need absolute value
            }

            // can't set first pixel position to 0.5 so we scale by 2
            // compute (dx,dy) * 2            
            dx2 = dx << 1;
            dy2 = dy << 1;

            // based on which delta is greater we can draw the line
            if (dx > dy)
            {
                // initialize the error term
                error = dy2 - dx;

                for (int i = 0; i <= dx; i++)
                {
                    image[index] = color;

                    // test if error has overflowed
                    if (error >= 0)
                    {
                        error -= dx2;

                        // move to next line
                        index += y_inc;
                    }

                    // adjust error term
                    error += dy2;

                    // move to next pixel
                    index += x_inc;
                }
            }
            else
            {
                // initialize the error term
                error = dx2 - dy;

                for (int i = 0; i <= dy; i++)
                {
                    image[index] = color;

                    // test if error has overflowed
                    if (error >= 0)
                    {
                        error -= dy2;

                        // move to next line
                        index += x_inc;
                    }

                    // adjust error term
                    error += dx2;

                    // move to next pixel
                    index += y_inc;
                }
            }
            return image;
        }
    }   
  
}
