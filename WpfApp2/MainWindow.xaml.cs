using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameEngine;
using GUI;
using GameEngine.Classes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point startPt, endPt;
        Line line;
        List<Line> list;
        Matrix matrix;

        private const int DEBUG = 0;
        private int x, y;
        private float z;
        //private Graphics g;
        private Pen p;
        private bool isMouseDown;
        private CAM4DV1 camFront = new CAM4DV1(),
                                   camLeft = new CAM4DV1(),
                                   camTop = new CAM4DV1(),
                                   camMain = new CAM4DV1();
        private Cube cube;
        private int numOfObjects;
        private Process game;
        private OBJECT4DV1 objectInstance;
        LinkedList<OBJECT4DV1> listObjects;
        
        bool sendingMessage = false;
        public MainWindow()
        {
            InitializeComponent();

            list = new List<Line>();
            listObjects = new LinkedList<OBJECT4DV1>();

            numOfObjects = listObjects.Count;
            //p = new Pen(Color.Black);
            isMouseDown = false;
            float fov = 90; // field of view
            #region init cameras
            // front cam
            Point4D cam_pos = new Point4D(0, 0, -100, 0);
            Vector4 cam_dir = new Vector4(0, 0, 0, 0);
            camFront.Init_CAM4DV1(cam_pos, cam_dir, fov,
                                                 (float)LeftCanvasColumn.Width.Value,
                                                (float)TopCanvasRow.Height.Value);
            //// left cam
            //cam_pos.X = -100;
            //cam_pos.Z = 0;
            //cam_dir.Y = (float)Math.PI / 2;
            //camLeft.Init_CAM4DV1(cam_pos, cam_dir, fov,
            //                                    splitContainer1.Panel2.Width,
            //                                    splitContainer1.Panel2.Height);
            //// top cam
            //cam_pos.X = 0; cam_pos.Y = 100; cam_pos.Z = 0;
            //cam_dir.X = (float)Math.PI / 2; cam_dir.Y = 0;
            //camTop.Init_CAM4DV1(cam_pos, cam_dir, fov,
            //                                    splitContainer2.Panel1.Width,
            //                                    splitContainer2.Panel1.Height);

            //// main cams
            //cam_pos.X = 0; cam_pos.Y = 0; cam_pos.Z = -100;
            //cam_dir.X = 0; cam_dir.Y = 0; cam_dir.Z = 0;
            //camMain.Init_CAM4DV1(cam_pos, cam_dir, fov,
            //                                    splitContainer2.Panel2.Width,
            //                                    splitContainer2.Panel2.Height);

            #endregion
        }



        private void TopLeftCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Define parameters used to create the BitmapSource.
            PixelFormat pf = PixelFormats.Bgr32;
            int width = 400;
            int height = 400;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];

            // Initialize the image with data.
            //Random value = new Random();
            //value.NextBytes(rawImage);

            line = new Line();
            startPt = e.GetPosition(TopLeftCanvas);
            line.X1 = startPt.X;
            line.Y1 = startPt.Y;
            endPt = e.GetPosition(TopLeftCanvas);
            line.X1 = endPt.X;
            line.Y1 = endPt.Y;
            line.Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            list.Add(line);

            //x = (int) startPt.X;
            //y = (int) startPt.Y;
            //z = camFront.pos.Z;

            byte color = 255;
            int x0 = Convert.ToInt32(startPt.X) * rawStride / 400;
            int y0 = Convert.ToInt32(startPt.Y);
            int x1 = Convert.ToInt32(endPt.X) * rawStride / 400;
            int y1 = Convert.ToInt32(endPt.Y);
            rawImage = GUI.GUI.Draw_Line(x0, y0, x1, y1, color, rawImage, rawStride);

            // Create a BitmapSource.
            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);

            // Create an image element;
            //Image myImage = new Image();
            //myImage.Width = 400;
            //// Set image source.
            //myImage.Source = bitmap;

            //TopLeftCanvas.Children.Add(line);
            //BitmapImage bitmapImage = new BitmapImage();
            //bitmapImage.BeginInit();
            //string src = TopLeftCanvas.Source.ToString();
            //bitmapImage.UriSource = new Uri(src);
            //bitmapImage.EndInit();
            //BitmapSource bitmapSource = TopLeftCanvas.Source;
            TopLeftCanvas.Source = bitmap;// bitmapImage;

            // if create cube button is down and left mouse button is pressed
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point pt = e.GetPosition(TopLeftCanvas);
                x = Convert.ToInt32(pt.X);
                y = Convert.ToInt32(pt.Y);
                z = camFront.pos.Z;
                cube = new Cube();
            }
        }

        private void repaint(byte[] rawImage)
        {
            TopLeftCanvas_Paint(rawImage);
        }

        private void TopLeftCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (line == null)
            //    return;
            //endPt = e.GetPosition(TopLeftCanvas);

            //line.X2 = endPt.X;
            //line.Y2 = endPt.Y;

            //// Define parameters used to create the BitmapSource.
            //PixelFormat pf = PixelFormats.Bgr32;
            //int width = 500;
            //int height = 400;
            //int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            //byte[] rawImage = new byte[rawStride * height];

            //byte color = 255;
            //int x0 = Convert.ToInt32(startPt.X) * rawStride / 500;
            //int y0 = Convert.ToInt32(startPt.Y);
            //int x1 = Convert.ToInt32(endPt.X) * rawStride / 500;
            //int y1 = Convert.ToInt32(endPt.Y);
            //rawImage = GUI.GUI.Draw_Line(x0, y0, x1, y1, color, rawImage, rawStride);
            //// Create a BitmapSource.
            //BitmapSource bitmap = BitmapSource.Create(width, height,
            //    96, 96, pf, null,
            //    rawImage, rawStride);
            //TopLeftCanvas.Source = bitmap;
        }

        private void MoveButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TurnOffAllButtons();
            MoveBorder.BorderBrush = Brushes.SlateBlue;
            MoveBorder.BorderThickness = new Thickness(1);
        }

        private void RotateButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TurnOffAllButtons();
            RotateBorder.BorderBrush = Brushes.SlateBlue;
            RotateBorder.BorderThickness = new Thickness(1);
        }

        private void ScaleButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TurnOffAllButtons();
            ScaleBorder.BorderBrush = Brushes.SlateBlue;
            ScaleBorder.BorderThickness = new Thickness(1);
        }

        private void ArrowButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TurnOffAllButtons();
            ArrowBorder.BorderBrush = Brushes.SlateBlue;
            ArrowBorder.BorderThickness = new Thickness(1);
        }

        private void CameraButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TurnOffAllButtons();
            CameraBorder.BorderBrush = Brushes.SlateBlue;
            CameraBorder.BorderThickness = new Thickness(1);
        }

        private void PlayButton_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TopLeftCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int x1, x2, y1, y2;
            float xper, yper, d;

            PixelFormat pf = PixelFormats.Bgr32;
            int width = 400;
            int height = 400;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];           
            repaint(rawImage);
        }

        private void TurnOffAllButtons()
        {
            CameraBorder.BorderBrush = null;
            CameraBorder.BorderThickness = new Thickness(0);
            CubeBorder.BorderBrush = null;
            CubeBorder.BorderThickness = new Thickness(0);
            ScaleBorder.BorderBrush = null;
            ScaleBorder.BorderThickness = new Thickness(0);
            RotateBorder.BorderBrush = null;
            RotateBorder.BorderThickness = new Thickness(0);
            MoveBorder.BorderBrush = null;
            MoveBorder.BorderThickness = new Thickness(0);
            ArrowBorder.BorderBrush = null;
            ArrowBorder.BorderThickness = new Thickness(0);
        }

        private void CubeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TurnOffAllButtons();
            CubeBorder.BorderBrush = Brushes.SlateBlue;
            CubeBorder.BorderThickness = new Thickness(1);
        }

        private void TopLeftCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            endPt = e.GetPosition(TopLeftCanvas);

            PositionLabel.Content = endPt.ToString();
            int xcoord = Convert.ToInt32(endPt.X);// - 400 / 2;// Convert.ToInt32(LeftCanvasColumn.Width.Value) / 2;
            int ycoord = Convert.ToInt32(endPt.Y);// (Convert.ToInt32(TopCanvasRow.Height.Value) - Convert.ToInt32(endPt.Y))
                //- Convert.ToInt32(TopCanvasRow.Height.Value) / 2;

            if (line == null)
                return;

            //float alpha = 0.5f * (float)LeftCanvasColumn.Width.Value - 0.5f;
            //float beta = 0.5f * (float)TopCanvasRow.Height.Value - 0.5f;
            int x1, x2, y1, y2;
            float xper, yper, d;
            if (e.LeftButton == MouseButtonState.Pressed && CubeBorder.BorderBrush != null)
            {                

                if (listObjects.Count > numOfObjects)
                {
                    listObjects.RemoveLast();
                }
                #region cube
                d = 1;// (float)LeftCanvasColumn.Width.Value / 2f * (float)Math.Tan(Math.PI/2);
                float columnWidth = 400;// Convert.ToSingle(LeftCanvasColumn.Width.Value);
                xper = (x - columnWidth / 2f) / (columnWidth / 2f);
                float x1c = (xper * (-camFront.pos.Z));
                x1 = Convert.ToInt32(x1c);
                xper = (xcoord - columnWidth / 2f) / (columnWidth / 2f);
                float x2c = (xper * (-camFront.pos.Z));
                x2 = Convert.ToInt32(x2c);


                if (x2 > x1)
                    cube.width = (x2 - x1);
                else
                    cube.width = (x1 - x2);               

                float rowHeight = Convert.ToSingle(TopCanvasRow.Height.Value);
                float startY = rowHeight - y; // invert
                ycoord = (int)rowHeight - ycoord;

                yper = (startY - rowHeight / 2f) / (rowHeight / 2f);
                y1 = (int)(yper * (-camFront.pos.Z));
                
                yper = (ycoord - rowHeight/ 2f) / (rowHeight / 2f);
                y2 = (int)(yper * (-camFront.pos.Z));

                if (y2 > y1)
                    cube.height = Math.Abs(y2 - y1);
                else
                    cube.height = Math.Abs(y1 - y2);

                cube.world_pos.X = ((x1 + x2) / 2);
                cube.world_pos.Y = -((y1 + y2) / 2);
                cube.world_pos.Z = (cube.length / 2);

                #endregion

                #region local coordinates
                cube.vlist_local[0].X = -cube.width / 2;
                cube.vlist_local[0].Y = cube.height / 2;
                cube.vlist_local[0].Z = -cube.length / 2;
                cube.vlist_local[1].X = cube.width / 2;
                cube.vlist_local[1].Y = cube.height / 2;
                cube.vlist_local[1].Z = -cube.length / 2;
                cube.vlist_local[2].X = cube.width / 2;
                cube.vlist_local[2].Y = -cube.height / 2;
                cube.vlist_local[2].Z = -cube.length / 2;
                cube.vlist_local[3].X = -cube.width / 2;
                cube.vlist_local[3].Y = -cube.height / 2;
                cube.vlist_local[3].Z = -cube.length / 2;
                cube.vlist_local[4].X = -cube.width / 2;
                cube.vlist_local[4].Y = cube.height / 2;
                cube.vlist_local[4].Z = cube.length / 2;
                cube.vlist_local[5].X = cube.width / 2;
                cube.vlist_local[5].Y = cube.height / 2;
                cube.vlist_local[5].Z = cube.length / 2;
                cube.vlist_local[6].X = cube.width / 2;
                cube.vlist_local[6].Y = -cube.height / 2;
                cube.vlist_local[6].Z = cube.length / 2;
                cube.vlist_local[7].X = -cube.width / 2;
                cube.vlist_local[7].Y = -cube.height / 2;
                cube.vlist_local[7].Z = cube.length / 2;
                #endregion

                cube.num_polys = 6;
                cube.plist[0] = new POLY4DV1();
                cube.plist[0].poly_num_verts = 4;
                cube.plist[0].vert = new int[4];
                cube.plist[0].vert[0] = 0;
                cube.plist[0].vert[1] = 1;
                cube.plist[0].vert[2] = 2;
                cube.plist[0].vert[3] = 3;

                cube.plist[1] = new POLY4DV1();
                cube.plist[1].poly_num_verts = 4;
                cube.plist[1].vert = new int[4];
                cube.plist[1].vert[0] = 4;
                cube.plist[1].vert[1] = 5;
                cube.plist[1].vert[2] = 6;
                cube.plist[1].vert[3] = 7;

                cube.plist[2] = new POLY4DV1();
                cube.plist[2].poly_num_verts = 4;
                cube.plist[2].vert = new int[4];
                cube.plist[2].vert[0] = 0;
                cube.plist[2].vert[1] = 3;
                cube.plist[2].vert[2] = 7;
                cube.plist[2].vert[3] = 4;

                cube.plist[3] = new POLY4DV1();
                cube.plist[3].poly_num_verts = 4;
                cube.plist[3].vert = new int[4];
                cube.plist[3].vert[0] = 1;
                cube.plist[3].vert[1] = 2;
                cube.plist[3].vert[2] = 6;
                cube.plist[3].vert[3] = 5;

                cube.plist[4] = new POLY4DV1();
                cube.plist[4].poly_num_verts = 4;
                cube.plist[4].vert = new int[4];
                cube.plist[4].vert[0] = 0;
                cube.plist[4].vert[1] = 1;
                cube.plist[4].vert[2] = 5;
                cube.plist[4].vert[3] = 4;

                cube.plist[5] = new POLY4DV1();
                cube.plist[5].poly_num_verts = 4;
                cube.plist[5].vert = new int[4];
                cube.plist[5].vert[0] = 2;
                cube.plist[5].vert[1] = 3;
                cube.plist[5].vert[2] = 7;
                cube.plist[5].vert[3] = 6;

                listObjects.AddLast(cube);
                

                //line.X2 = endPt.X;
                //line.Y2 = endPt.Y;

                //// Define parameters used to create the BitmapSource.
                PixelFormat pf = PixelFormats.Bgr32;
                int width = 400;
                int height = 400;
                int rawStride = (width * pf.BitsPerPixel + 7) / 8;
                byte[] rawImage = new byte[rawStride * height];

                byte color = 255;
                //x1 = Convert.ToInt32(startPt.X) * rawStride / width;// (int)LeftCanvasColumn.Width.Value;
                //y1 = Convert.ToInt32(startPt.Y);
                //x2 = Convert.ToInt32(endPt.X) * rawStride / width;// (int)LeftCanvasColumn.Width.Value;
                //y2 = Convert.ToInt32(endPt.Y);
                //rawImage = GUI.GUI.Draw_Line(x1, y1, x2, y2, color, rawImage, rawStride);
                //// Create a BitmapSource.
                //BitmapSource bitmap = BitmapSource.Create(width, height,
                //    96, 96, pf, null,
                //    rawImage, rawStride);
                //TopLeftCanvas.Source = bitmap;
                repaint(rawImage);
            }
        }

        private void BottomRightCanvas_Paint()
        {
           
        }
        private void TopLeftCanvas_Paint(byte[] rawImage)
        {
            int x1, y1, x2, y2;
            int h = (int)TopCanvasRow.Height.Value;
            int w = (int)LeftCanvasColumn.Width.Value;
            foreach (OBJECT4DV1 obj in listObjects)
            {
                //p.Color = Color.Black;
                //if (treeView1.SelectedNodes.Count > 0)
                //{
                //    for (int i = 0; i < treeView1.SelectedNodes.Count; i++)
                //    {
                //        TreeNode node = treeView1.SelectedNodes[i] as TreeNode;
                //        if (node.Index == obj.id)
                //            p.Color = Color.Red;
                //    }
                //}
                #region convert from world to camera coordinates
                CortexMatrix Tcam_inv = CortexMatrix.Identity, Mtemp1, Mtemp2;
                Tcam_inv.M41 = -camFront.pos.X;
                Tcam_inv.M42 = -camFront.pos.Y;
                Tcam_inv.M43 = -camFront.pos.Z;
                Tcam_inv.M44 = 1;

                Mtemp1 = CortexMatrix.Multiply(Tcam_inv, CortexMatrix.RotationY(camFront.dir.Y));
                Mtemp2 = CortexMatrix.Multiply(CortexMatrix.RotationX(camFront.dir.X),
                                                      CortexMatrix.RotationZ(camFront.dir.Z));
                Tcam_inv = CortexMatrix.Multiply(Mtemp1, Mtemp2);
                CortexMatrix[] cube_camera = new CortexMatrix[obj.num_vertices];
                CortexMatrix[] cube_world = new CortexMatrix[obj.num_vertices];
                CortexMatrix[] cube_rotate = new CortexMatrix[obj.num_vertices];
                CortexMatrix[] cube_rotation = new CortexMatrix[obj.num_vertices];

                CortexMatrix Mrotation;
                Mrotation = CortexMatrix.RotationYawPitchRoll(obj.dir.Y, obj.dir.X, obj.dir.Z);

                for (int i = 0; i < obj.num_vertices; i++)
                {
                    cube_rotate[i] = CortexMatrix.Zero;
                    cube_rotate[i].M11 = obj.vlist_local[i].X * obj.scale.X;
                    cube_rotate[i].M12 = obj.vlist_local[i].Y * obj.scale.Y;
                    cube_rotate[i].M13 = obj.vlist_local[i].Z * obj.scale.Z;
                    cube_rotate[i].M14 = obj.vlist_local[i].W;
                    cube_rotation[i] = CortexMatrix.Multiply(cube_rotate[i], Mrotation);
                }

                for (int i = 0; i < obj.num_vertices; i++)
                {
                    cube_world[i] = CortexMatrix.Zero;
                    cube_world[i].M11 = obj.world_pos.X + cube_rotation[i].M11;
                    cube_world[i].M12 = obj.world_pos.Y + cube_rotation[i].M12;
                    cube_world[i].M13 = obj.world_pos.Z + cube_rotation[i].M13;
                    cube_world[i].M14 = 1;
                    cube_camera[i] = CortexMatrix.Multiply(cube_world[i], Tcam_inv);
                }
                #endregion

                #region perspective
                int d = 1;// splitContainer1.Panel1.Width / 2;
                //float ar =(float) splitContainer1.Panel1.Width / (float)splitContainer1.Panel1.Height;
                Point4D[] cube_per = new Point4D[obj.num_vertices];
                Single z_nearest = Single.MaxValue, z_farthest = Single.MinValue;
                for (int vertex = 0; vertex < obj.num_vertices; vertex++)
                {
                    cube_per[vertex].X = d * cube_camera[vertex].M11 / cube_camera[vertex].M13;
                    cube_per[vertex].Y = d * cube_camera[vertex].M12 / cube_camera[vertex].M13;
                    if (cube_camera[vertex].M13 < z_nearest)
                        z_nearest = cube_camera[vertex].M13;
                    if (cube_camera[vertex].M13 > z_farthest)
                        z_farthest = cube_camera[vertex].M13;
                }
                #endregion

                #region image space clipping

                if (z_nearest < camFront.pos.Z + 50 || z_farthest > Single.MaxValue)
                    continue;
                #endregion

                #region screen
                float alpha = 0.5f * 400- 0.5f; //(float)LeftCanvasColumn.Width.Value 
                float beta = 0.5f * (float)TopCanvasRow.Height.Value - 0.5f;

                Point[] pt = new Point[obj.num_vertices];
                for (int vertex = 0; vertex < obj.num_vertices; vertex++)
                {
                    pt[vertex] = new Point();
                    pt[vertex].X = (int)(alpha + cube_per[vertex].X * alpha);
                    pt[vertex].Y = (int)(beta + cube_per[vertex].Y * beta);
                }

                // Define parameters used to create the BitmapSource.
                PixelFormat pf = PixelFormats.Bgr32;
                int width = 400;
                int height = 400;
                int rawStride = (width * pf.BitsPerPixel + 7) / 8;
                //byte[] rawImage = new byte[rawStride * height];
                Position2Label.Text = "";
                for (int poly = 0; poly < obj.num_polys; poly++)
                {
                    for (int i = 0; i < obj.plist[poly].vert.Length; i++)
                    {
                        if (i == obj.plist[poly].vert.Length - 1)
                        {
                            x1 = (int)pt[obj.plist[poly].vert[i]].X;
                            y1 = (int)pt[obj.plist[poly].vert[i]].Y;
                            x2 = (int)pt[obj.plist[poly].vert[0]].X;
                            y2 = (int)pt[obj.plist[poly].vert[0]].Y;

                            if (x1 < 0)
                                x1 = 0;
                            else if (x1 > width)// LeftCanvasColumn.Width.Value)
                                x1 = width;// (int)LeftCanvasColumn.Width.Value;
                            if (x2 < 0)
                                x2 = 0;
                            else if (x2 > width) // LeftCanvasColumn.Width.Value)
                                x2 = width;// (int)LeftCanvasColumn.Width.Value;

                            if (y1 < 0)
                                y1 = 0;
                            else if (y1 > TopCanvasRow.Height.Value)
                                y1 = (int)TopCanvasRow.Height.Value;
                            if (y2 < 0)
                                y2 = 0;
                            else if (y2 > TopCanvasRow.Height.Value)
                                y2 = (int)TopCanvasRow.Height.Value;                            

                            byte color = 255;
                            //if (x1 > x2)
                                x1 = (int)(x1 * rawStride / width);// (int)LeftCanvasColumn.Width.Value;
                            //else
                            //    x1 = (int)(x1 * rawStride / width);
                            //if (x2 > x1)
                                x2 = (int)(x2 * rawStride / width);// (int)LeftCanvasColumn.Width.Value ;
                            //else
                            //    x2 = (int)(x2 * rawStride / width);
                            // y2 = Convert.ToInt32(endPt.Y);
                            rawImage = GUI.GUI.Draw_Line(x1, y1, x2, y2, color, rawImage, rawStride);
                            Position2Label.Text += "Poly: " + poly + " Vertex " + i + ": " + x1 + " Vertex " + (i + 1) + ": " + x2 + "\n";
                        }
                        else
                        {
                            x1 = (int)pt[obj.plist[poly].vert[i]].X;
                            y1 = (int)pt[obj.plist[poly].vert[i]].Y;
                            x2 = (int)pt[obj.plist[poly].vert[i + 1]].X;
                            y2 = (int)pt[obj.plist[poly].vert[i + 1]].Y;
                         
                            byte color = 255;
                            
                            if (x1 < 0)
                                x1 = 0;
                            else if (x1 > width)//LeftCanvasColumn.Width.Value)
                                x1 = width;// (int)LeftCanvasColumn.Width.Value;
                            if (x2 < 0)
                                x2 = 0;
                            else if (x2 > width)// LeftCanvasColumn.Width.Value)
                                x2 = width;// (int)LeftCanvasColumn.Width.Value;

                            if (y1 < 0)
                                y1 = 0;
                            else if (y1 > TopCanvasRow.Height.Value)
                                y1 = (int)TopCanvasRow.Height.Value;
                            if (y2 < 0)
                                y2 = 0;
                            else if (y2 > TopCanvasRow.Height.Value)
                                y2 = (int)TopCanvasRow.Height.Value;

                            //if (x1 > x2)
                            x1 = (int)(x1 * rawStride / width);// ;// (int)LeftCanvasColumn.Width.Value;
                                                                               //else
                                                                               //    x1 = (int)(x1 * rawStride / width);
                                                                               //if (x2 > x1)
                            x2 = (int)(x2 * rawStride / width);// ;// (int)LeftCanvasColumn.Width.Value ;
                            //else
                            //    x2 = (int)(x2 * rawStride / width );
                            //y2 = Convert.ToInt32(endPt.Y);

                            Position2Label.Text += "Poly: " + poly + " Vertex " + i + ": " + x1 + " Vertex " + (i+1) + ": " + x2 + "\n";
                            rawImage = GUI.GUI.Draw_Line(x1, y1, x2, y2, color, rawImage, rawStride);
                        }
                    }
                }
               
                // Create a BitmapSource.
                BitmapSource bitmap = BitmapSource.Create(width, height,
                    96, 96, pf, null,
                    rawImage, rawStride);
                TopLeftCanvas.Source = bitmap;
                #endregion

            }
        }
    }
}
//// Define parameters used to create the BitmapSource.
//PixelFormat pf = PixelFormats.Bgr32;
//int width = 500;
//int height = 400;
//int rawStride = (width * pf.BitsPerPixel + 7) / 8;
//byte[] rawImage = new byte[rawStride * height];

//byte color = 255;
//x1 = Convert.ToInt32(startPt.X) * rawStride / (int)LeftCanvasColumn.Width.Value;
//y1 = Convert.ToInt32(startPt.Y);
//x2 = Convert.ToInt32(endPt.X) * rawStride/ (int)LeftCanvasColumn.Width.Value;
//y2 = Convert.ToInt32(endPt.Y);
//rawImage = GUI.GUI.Draw_Line(x1, y1, x2, y2, color, rawImage, rawStride);
//// Create a BitmapSource.
//BitmapSource bitmap = BitmapSource.Create(width, height,
//    96, 96, pf, null,
//    rawImage, rawStride);
//TopLeftCanvas.Source = bitmap;