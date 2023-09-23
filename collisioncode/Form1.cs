using Microsoft.VisualBasic.Devices;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography.Pkcs;
using System.Windows.Forms;
using static collisioncode.Form1;

namespace collisioncode
{
    public partial class Form1 : Form
    {
        //==========PUBLIC=VARIABLS=========
        #region 
        public class Circle
        {
            public float x { get; set; }
            public int health { get; set; }

            public float y { get; set; }
            public float z { get; set; }
            public float vx { get; set; }
            public float vy { get; set; }
            public float v { get; set; }
            public float radi { get; set; }
            public Color color { get; set; }
        }
        public class Block
        {
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
            public float sx { get; set; }
            public float sy { get; set; }
            public float sz { get; set; }
            public Color color { get; set; }
        }
        public class Triangle
        {
            public PointF p1 { get; set; }
            public PointF p2 { get; set; }
            public PointF p3 { get; set; }
            public Vector3 pa { get; set; }
            public Vector3 normal { get; set; }
            public float dis { get; set; }
            public Color color { get; set; }
        }
        public class Player
        {
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
            public float vx { get; set; }
            public float vy { get; set; }
            public float vz { get; set; }
            public float mx { get; set; }
            public float my { get; set; }
            public float mz { get; set; }
            public float sx { get; set; }
            public float sy { get; set; }
            public float sz { get; set; }
            public Color color { get; set; }
            public bool grounged { get; set; }
        }
        public Player player = new Player() {x = 0,y = 0,z = 0,sx = 100,sy = 250,sz = 100, color = Color.Yellow, grounged = false };
        public List<Block> block = new List<Block>();
       public List<Circle> circle = new List<Circle>();
       public List<Triangle> triangle = new List<Triangle>();
        Random rng = new Random();
        Bitmap bmp = new Bitmap(800, 800);
        float mouseX, mouseY;
        float mouseDX, mouseDY;
        bool mouseD;
        bool mouserD;
        bool gravity;
        bool keyfD;
        bool inpU, inpD, inpL, inpR;
        bool inpArrU, inpArrD, inpArrL, inpArrR;
        float temprX;
        float temprY;
        float screenscale;
        float screenscalex;
        float screenscaley;
        float screenzoom = 2000;
        float screenzoomV;
        float screenoffsetX = 1000;
        float screenoffsetY = 1000;
        Vector3 spaceOffset = new Vector3();
        float Rx = 0f;
        float Ry = 0f;
        float Rz = 0;
        float dis = 0;
        float distance = 4;
        Vector3 campos;

        Graphics g;
        #endregion

        //============START=AND=LOGIC========
        #region
        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += MouseScrole;
            Cursor.Hide();
           
            Block tempC = new Block();
            tempC.y = 900;
            tempC.x = 500;
            tempC.z = 0;
            tempC.sx = 1050;
            tempC.sy = 100;
            tempC.sz = 600;

            tempC.color = Color.Brown;

            block.Add(tempC);
        }
        void Gooo()
        {
            //test
            
            screenscale = bmp.Height / screenzoom;

            screenscalex = bmp.Width  / 1000f;
            screenscaley = bmp.Height / 1000f;
           
            g = Graphics.FromImage(bmp);
            g.FillRectangle(new SolidBrush(Color.DarkGray), 0, 0, screenscalex * 1000, screenscaley * 1000);
            triangle.Clear();
            #region
            for (int i = 0; i < circle.Count; i++)
            {

                getCollisions(i);
                CirclePhysics(i);

            }
            circle.RemoveAll(j => j.health < 1);
            for (int i = 0; i < circle.Count; i++)
            {
               
                SetVectors( Makepoint(circle[i].x, circle[i].y, circle[i].z, circle[i].radi, circle[i].radi, circle[i].radi),Color.Blue);

            }


            for (int i = 0; i < block.Count; i++)
            {
                SetVectors( Makepoint(block[i].x, block[i].y, block[i].z, block[i].sx, block[i].sy, block[i].sz), block[i].color);
            }


            SetVectors( Makepoint(player.x, player.y,player.z, player.sx, player.sy, player.sx),player.color);


            if (mouserD || mouseD)
            {
                PointF[] points = new PointF[] { new PointF(player.x, player.y - (player.sy / 2f)), new PointF(player.x, player.y - (player.sy / 2f)), new PointF((player.x - temprX), (player.y - (player.sy / 2f) - temprY)), new PointF((player.x - temprX), (player.y - (player.sy / 2f) - temprY)) };
                SetVectors( Makepoint2(points, 5,player.z),Color.Gray);
            }
            if (keyfD)
            {
              PointF[] points = new PointF[] { new PointF { X = mouseX - screenoffsetX + spaceOffset.X, Y = mouseY - screenoffsetY + spaceOffset.Y }, new PointF { X = mouseX - screenoffsetX + spaceOffset.X, Y = mouseDY - screenoffsetY + spaceOffset.Y }, new PointF { X = mouseDX - screenoffsetX + spaceOffset.X, Y = mouseDY - screenoffsetY + spaceOffset.Y }, new PointF { X = mouseDX - screenoffsetX + spaceOffset.X , Y = mouseY - screenoffsetY + spaceOffset.Y } };
              SetVectors(Makepoint2(points, 600,player.z),Color.Gray);
            }
           // drawCir( mouseDX - screenoffsetX, mouseDY - screenoffsetY, 5, Color.BlueViolet);
            SetVectors(Makepoint(mouseDX - screenoffsetX + spaceOffset.X, mouseDY - screenoffsetY + spaceOffset.Y,player.z, 50, 50, 50), Color.Gray);

            #endregion

            MovePlayer();
          //  triangle.RemoveAll(j => Vector3.Dot(j.normal,Vector3.Normalize(campos)) 
          //  > 0.0f);
            triangle.RemoveAll(j => j.pa.Z <= campos.Z);
            triangle.Sort((t1, t2) => t1.dis.CompareTo(t2.dis));
            for(int i = 0; i < triangle.Count; i++)
            {
                drawShp(new PointF[] { triangle[i].p1, triangle[i].p2, triangle[i].p3 }, triangle[i].color,1);
            }
            pictureBox1.Image = bmp;
            
        }
        private void UpdateTic(object sender, EventArgs e)
        {
            campos = new Vector3(0, 0, -(distance * 1000));
            Inputs();
            Gooo();

        }
        void CirclePhysics(int i)
        {
            // float bounce = (6f*rng.NextSingle()/10f) -0.1f;

            // circle[i].vy += 0.5f;
            // circle[i].vx *= 0.99f;
            // circle[i].vy *= 0.99f;
            if (circle[i].vx < 0.1 && circle[i].vx > -0.1) { circle[i].vx = 0; }

            circle[i].vy = Math.Clamp(circle[i].vy, -40, 40);
            circle[i].vx = Math.Clamp(circle[i].vx, -40, 40);
            circle[i].v = Math.Clamp(MathF.Sqrt((circle[i].vy * circle[i].vy) + (circle[i].vx * circle[i].vx)), 0, 10) / 10f;

            circle[i].x += circle[i].vx;
            circle[i].y += circle[i].vy;
        }
        void MovePlayer()
        {
            
            player.vx += player.mx;
            player.vz += player.mz;
            player.vy += player.my * 2;
            player.vx = Math.Clamp(player.vx,-40, 40);
            player.vz = Math.Clamp(player.vz,-40, 40);
            player.vy = Math.Clamp(player.vy,-40, 40);
            player.x += player.vx;
            player.z += player.vz;
            player.y += player.vy;

            if(spaceOffset.X > ( player.x))
            {
                 spaceOffset.X += 0.03f * (( player.x)-spaceOffset.X);

            }
            if (spaceOffset.X < ( player.x))
               {
                   spaceOffset.X += 0.03f * ((player.x)- spaceOffset.X);
            }
            if(spaceOffset.Y > ( player.y))
            {
                 spaceOffset.Y += 0.03f * (( player.y)-spaceOffset.Y);

            }
            if (spaceOffset.Y < (player.y))
               {
                   spaceOffset.Y += 0.03f * ((player.y)- spaceOffset.Y);
            } 
            
            {
                player.vx *= 0.955f;
                player.vy *= 0.99f;
                player.vz *= 0.955f;

            }
            if(gravity)
            {
                player.vy += 1f;

            }

            player.grounged = false;
            for (int t = 0; t < block.Count; t++)
            {
                if (((player.y + (player.sy / 2f) + 3 > block[t].y - (block[t].sy * 0.5f)) && ((player.y - (player.sy / 2f) - 3 < block[t].y + (block[t].sy * 0.5f)))) && ((player.x + (player.sx / 2f) + 3 > block[t].x - (block[t].sx * 0.5f)) && ((player.x - (player.sx / 2f) - 3 < block[t].x + (block[t].sx * 0.5f)))) && ((player.z + (player.sz / 2f) + 3 > block[t].z - (block[t].sz * 0.5f)) && ((player.z - (player.sz / 2f) - 3 < block[t].z + (block[t].sz * 0.5f)))))
                {
                    if (player.y < block[t].y - (block[t].sy * 0.5f))
                    {
                        player.grounged = true;
                    }
                }
            }

            //============================================
            for (int t = 0; t < block.Count; t++)
            {
                float bounce = 0.8f;


                if (((player.y + (player.sy / 2f) >= block[t].y - (block[t].sy * 0.5f)) && ((player.y - (player.sy / 2f) <= block[t].y + (block[t].sy * 0.5f)))) && ((player.x + (player.sx / 2f) >= block[t].x - (block[t].sx * 0.5f)) && ((player.x - (player.sx / 2f) <= block[t].x + (block[t].sx * 0.5f)))) && ((player.z + (player.sz / 2f) > block[t].z - (block[t].sz * 0.5f)) && ((player.z - (player.sz / 2f) < block[t].z + (block[t].sz * 0.5f)))))
                {
                   
                    float ydif = ((0.5f*player.sy) + (0.5f * block[t].sy)) - MathF.Abs (player.y - block[t].y);
                    float xdif = ((0.5f * player.sx) + (0.5f * block[t].sx)) - MathF.Abs(player.x - block[t].x);
                    float zdif = ((0.5f * player.sz) + (0.5f * block[t].sz)) - MathF.Abs(player.z - block[t].z);
                    
                    
                    if (ydif < xdif && ydif < zdif)
                    {

                        if (player.y + (player.sy / 2f) < block[t].y)
                        {
                            player.vy *= -1 + bounce;
                            player.y = block[t].y - (block[t].sy * 0.5f) - (player.sy / 2f);
                            
                        }
                        else if (player.y - (player.sy / 2f) > block[t].y)
                        {
                            player.vy *= -1 + bounce;
                            player.y = block[t].y + (block[t].sy * 0.5f) + (player.sy / 2f);
                        }
                        
                        
                    }
                   else if (xdif < zdif)
                    {
                       if (player.x + (player.sx / 2f) < block[t].x)
                       {
                           player.vx *= -1 + bounce;
                           player.x = block[t].x - (block[t].sx * 0.5f) - (player.sx / 2f);
                 
                       }
                       else if (player.x - (player.sx / 2f) > block[t].x)
                       {
                           player.vx *= -1 + bounce;
                           player.x = block[t].x + (block[t].sx * 0.5f) + (player.sx / 2f);
                       }
                 
                 
                   }
                    else
                    {
                        if (player.z + (player.sz / 2f) < block[t].z)
                        {
                            player.vz *= -1 + bounce;
                            player.z = block[t].z - (block[t].sz * 0.5f) - (player.sz / 2f);

                        }
                        else if (player.z - (player.sz / 2f) > block[t].z)
                        {
                            player.vz *= -1 + bounce;
                            player.z = block[t].z + (block[t].sz * 0.5f) + (player.sz / 2f);
                        }
                    }



                }
            }

        }
        void getCollisions(int i)
        {

            for (int t = 0; t < circle.Count; t++)
            {

                if (i != t)
                {
                    float dis;
                    float overlap;
                    dis = MathF.Sqrt(((circle[i].y - circle[t].y) * (circle[i].y - circle[t].y)) + ((circle[i].x - circle[t].x) * (circle[i].x - circle[t].x)));
                    overlap = (dis - circle[i].radi - circle[t].radi) * 0.1f;
                    if (dis < circle[i].radi + circle[t].radi)
                    {
                        circle[i].vx -= (overlap * (circle[i].x - circle[t].x)) / dis;
                        circle[i].vy -= (overlap * (circle[i].y - circle[t].y)) / dis;

                    }
                }
            }
            float bounce = 0f;

            for (int t = 0; t < block.Count; t++)
            {

                if (((circle[i].y + circle[i].radi > block[t].y - (block[t].sy * 0.5f)) && ((circle[i].y - circle[i].radi < block[t].y + (block[t].sy * 0.5f)))) && ((circle[i].x + circle[i].radi > block[t].x - (block[t].sx * 0.5f)) && ((circle[i].x - circle[i].radi < block[t].x + (block[t].sx * 0.5f)))))
                {
                    float ydif = ((  circle[i].radi) + (0.5f * block[t].sy)) - MathF.Abs(circle[i].y - block[t].y);
                    float xdif = ((  circle[i].radi) + (0.5f * block[t].sx)) - MathF.Abs(circle[i].x - block[t].x);


                    if (ydif > xdif)
                      //  if ((circle[i].y > block[t].y - (block[t].sy * 0.5f)) && (circle[i].y < block[t].y + (block[t].sy * 0.5f)))
                    {
                        if (circle[i].x < block[t].x)
                        {
                            circle[i].vx *= -1 + bounce;
                            circle[i].x = block[t].x - (block[t].sx * 0.5f) - circle[i].radi;
                            circle[i].health--;
                        }
                        else if (circle[i].x > block[t].x)
                        {
                            circle[i].vx *= -1 + bounce;
                            circle[i].x = block[t].x + (block[t].sx * 0.5f) + circle[i].radi;
                            circle[i].health--;

                        }


                    }else
                    {
                        if (circle[i].y < block[t].y)
                        {
                            circle[i].vy *= -1 + bounce;
                            circle[i].y = block[t].y - (block[t].sy * 0.5f) - circle[i].radi;
                            circle[i].health--;

                        }
                        else if (circle[i].y > block[t].y)
                        {
                            circle[i].vy *= -1 + bounce;
                            circle[i].y = block[t].y + (block[t].sy * 0.5f) + circle[i].radi;
                            circle[i].health--;

                        }


                    }


                    //circle[i].vx *= -1 + bounce;

                }
            }


        }
        #endregion

        //===========DRAWING================
        #region
        void drawShp(PointF[] points1, Color color,int thick)
        {
            for (int i = 0; i < points1.Length; i++)
            {
                points1[i].X *= screenscale;
                points1[i].X += (screenoffsetX * screenscale);
                
                points1[i].Y *= screenscale;
                points1[i].Y += (screenoffsetY * screenscale);
            }
            
           
            {
                 
         //    g.FillPolygon(new SolidBrush(color),new PointF[] { points1[0], points1[1], points1[2] });
             g.DrawLines(new Pen(color, 1),new PointF[] { points1[0], points1[1], points1[2] });
               
            }
            
        }
        #endregion
        //===========3D================
        #region

        Vector3[] Makepoint(float px, float py,float pz, float pw, float ph, float depth)
        {
            px *= 0.001f;
            py *= 0.001f;
            pz *= 0.001f;
            pw *= 0.0005f;
            ph *= 0.0005f;
         depth *= 0.0005f;

            Vector3[] madePointes = new Vector3[8]
            {
                new Vector3(px - pw, py - ph,pz+depth),
                new Vector3(px + pw, py - ph,pz+depth),
                new Vector3(px + pw, py + ph,pz+depth),
                new Vector3(px - pw, py + ph,pz+depth),

                new Vector3(px - pw, py - ph,pz - depth),
                new Vector3(px + pw, py - ph,pz - depth),
                new Vector3(px + pw, py + ph,pz - depth),
                new Vector3(px - pw, py + ph,pz - depth)

            };

            return madePointes;
        }
        
        Vector3[] Makepoint2(PointF[] Pp, float depth, float depth2)
        {
            for (int i = 0; i < Pp.Length; i++)
            {
                Pp[i].X *= 0.001f;
                Pp[i].Y *= 0.001f;
            }
            depth *= 0.0005f;
            depth2 *= 0.001f;
            int tmp = Pp.Length * 2;
            List<Vector3> madePointes = new List<Vector3>();
           
            for (int i = 0; i < Pp.Length; i++)
            {
                madePointes.Add(new Vector3( Pp[i].X , Pp[i].Y, depth2 - depth ));

            }
            for (int i = 0; i < Pp.Length; i++)
            {
                madePointes.Add(new Vector3( Pp[i].X , Pp[i].Y, depth2 + depth ));
            }
            return madePointes.ToArray();
        }
        void SetVectors( Vector3[] points,Color objCol)
        {
            float[,] rotationZ = new float[,] {
             {MathF.Cos(Rz), -MathF.Sin(Rz), 0},
              {MathF.Sin(Rz), MathF.Cos(Rz), 0},
              {0, 0, 1},
            };

            float[,] rotationX = new float[,] {
              {1, 0, 0},
              {0, MathF.Cos(Rx), -MathF.Sin(Rx)},
              {0, MathF.Sin(Rx), MathF.Cos(Rx)},
            };

            float[,] rotationY = new float[,] {
              { MathF.Cos(Ry), 0, MathF.Sin(Ry)},
              {0, 1, 0},
              {-MathF.Sin(Ry), 0, MathF.Cos(Ry)},
            };

            Vector3[] projected = new Vector3[points.Length];
            Vector3[] fullyrotated = new Vector3[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 rotated = matadd(rotationY, points[i]);
                rotated = matmul(rotationY, rotated);
                rotated = matmul(rotationX, rotated);
                rotated = matmul(rotationZ, rotated);
                fullyrotated[i] = rotated;
              //  float distance = 5;
                float z = 1 / (distance - rotated.Z);
                float[,] projection = new float[,] {
                  { z, 0, 0},
                  {0, z, 0},
                  {0, 0, 0},
                };
                Vector3 projected2d = matmul(projection, rotated);

                projected2d *= (2000);
                projected[i] = projected2d;
               
            }
            
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 0, 1, 2));
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 2, 3, 0));
          
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 5, 4, 7));
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 7, 6, 5));
          
           triangle.Add(MakeTri(projected, fullyrotated, objCol,  1, 0,4));
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 4, 5, 1));
          
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 3, 2, 6));
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 6, 7, 3));
          
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 1, 5, 6));
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 6, 2,1));
          
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 4, 0, 3));
           triangle.Add(MakeTri(projected, fullyrotated, objCol, 3, 7,4));

            
        }
        Triangle MakeTri(Vector3[] projected, Vector3[] fullyrotated,Color objCol,int a, int b, int c)
        {
            Triangle tri = new Triangle();
            tri.p1 = new PointF(projected[a].X, projected[a].Y);
            tri.p2 = new PointF(projected[b].X, projected[b].Y);
            tri.p3 = new PointF(projected[c].X, projected[c].Y);
            tri.pa = (fullyrotated[a] * fullyrotated[b] * fullyrotated[c]) / 3f;
            tri.dis = ((tri.pa.X- campos.X) *(tri.pa.X - campos.X))+ ((tri.pa.Y - campos.Y) *(tri.pa.Y - campos.Y)) + ((tri.pa.Z - campos.Z) *(tri.pa.Z - campos.Z));
            tri.color = objCol;
            tri.normal = Vector3.Cross(fullyrotated[a] - fullyrotated[c], fullyrotated[b] - fullyrotated[c]);
            tri.normal = Vector3.Normalize(tri.normal);

            return tri;

        }
        Vector3 matmul(float[,] matrix, Vector3 point)
        {
            Vector3 newPoint;
            newPoint.X = (point.X * matrix[0, 0]) + (point.Y * matrix[1, 0]) + (point.Z * matrix[2, 0]);
            newPoint.Y = (point.X * matrix[0, 1]) + (point.Y * matrix[1, 1]) + (point.Z * matrix[2, 1]);
            newPoint.Z = (point.X * matrix[0, 2]) + (point.Y * matrix[1, 2]) + (point.Z * matrix[2, 2]);
            return newPoint;
        }

        Vector3 matadd(float[,] matrix, Vector3 point)
        {
            Vector3 newPoint;
            newPoint.X = point.X - (spaceOffset.X/1000f);
            newPoint.Y = point.Y - (spaceOffset.Y/1000f);
            newPoint.Z = point.Z - (spaceOffset.Z/1000f);
            return newPoint;
        }
        void connect(int i, int j, Vector3[] points)
        {
       //    Vector3 a = points[i];
       //    Vector3 b = points[j];
       //    drawShp(new PointF[] {new PointF(a.X,a.Y), new PointF(b.X, b.Y) },Color.Black,3);
        }

        #endregion

        //==========INPUTS==================
        #region
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                mouseD = true;

            }
            if(e.Button == MouseButtons.Right)
            {
               mouserD = true;

            }
            
            
               

        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Circle tempC = new Circle();
                tempC.y = player.y - (player.sy / 2f);
                tempC.x = player.x;
                tempC.z = player.z;
                tempC.radi = 25;

                tempC.vy = temprY / 5f;
                tempC.vx = temprX / 5f;
                tempC.color = Color.Aquamarine;
                tempC.health = 10;
                circle.Add(tempC);
                mouseD = false;

            }
            if (e.Button == MouseButtons.Right)
            {



                player.vy = -temprY / 3f;
                player.vx = -temprX / 3f;

                mouserD = false;
            }

        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            

        }
        private void MouseScrole(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) { screenzoomV -= 10; }
            if (e.Delta < 0) { screenzoomV += 10; }
            
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Space) && player.grounged) { player.vy = -40; }
            if (e.KeyCode == Keys.E)
            {
                circle.Clear();
            }
            if (e.KeyCode == Keys.Q)
            {
                block.Clear();
            }
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (UpdateTimer.Enabled)
                { UpdateTimer.Stop(); }
                else
                {
                    UpdateTimer.Start();
                }
            }

            if (e.KeyCode == Keys.D) { inpR = true; }
            if (e.KeyCode == Keys.A) { inpL = true; }
            if (e.KeyCode == Keys.W) { inpU = true; }
            if (e.KeyCode == Keys.S) { inpD = true; }

            if (e.KeyCode == Keys.Right) { inpArrR = true; }
            if (e.KeyCode == Keys.Left) { inpArrL = true; }
            if (e.KeyCode == Keys.Up) { inpArrU = true; }
            if (e.KeyCode == Keys.Down) { inpArrD = true; }
            if (e.KeyCode == Keys.F){ keyfD = true;}
            if (e.KeyCode == Keys.G && gravity) { gravity = false;}
            else if (e.KeyCode == Keys.G && !gravity) { gravity = true; }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F) 
            {
                Block tempC = new Block();
                tempC.y =  ((mouseY + mouseDY) / 2f ) - screenoffsetY + spaceOffset.Y;
                tempC.x =  ((mouseX + mouseDX) / 2f ) - screenoffsetX + spaceOffset.X;
                tempC.z = player.z;
                tempC.sx = MathF.Abs(temprX );
                if (tempC.sx < 100) { tempC.sx = 100; }
                tempC.sy = MathF.Abs( temprY );
                if (tempC.sy < 100) { tempC.sy = 100; }
                tempC.sz = 600;

                tempC.color = Color.Brown;
                #region

                if (!(((player.y + (player.sy / 2f) >= tempC.y - (tempC.sy * 0.5f)) && ((player.y - (player.sy / 2f) <= tempC.y + (tempC.sy * 0.5f)))) && ((player.x + (player.sx / 2f) >= tempC.x - (tempC.sx * 0.5f)) && ((player.x - (player.sx / 2f) <= tempC.x + (tempC.sx * 0.5f))))))
                {
                    block.Add(tempC);

                }

                #endregion


                keyfD = false;
            }
            if (e.KeyCode == Keys.D) { inpR = false; }
            if (e.KeyCode == Keys.A) { inpL = false; }
            if (e.KeyCode == Keys.W) { inpU = false; }
            if (e.KeyCode == Keys.S) { inpD = false; }

            if (e.KeyCode == Keys.Right){ inpArrR = false; }
            if (e.KeyCode == Keys.Left) { inpArrL = false; }
            if (e.KeyCode == Keys.Up)   { inpArrU = false; }
            if (e.KeyCode == Keys.Down) { inpArrD = false; }
        }
        private void Inputs()
        {
            var relativePoint = PointToClient(Cursor.Position);
            screenzoomV *= 0.9f;
            distance += screenzoomV/100f;
            //distance = Math.Clamp(screenzoom, 3, 6);

           
            if (!mouserD && !mouseD && !keyfD)
            {
                mouseX = (relativePoint.X ) / screenscale;
                mouseY = (relativePoint.Y ) / screenscale;
            }

            mouseDX = (relativePoint.X ) / screenscale;
            mouseDY = (relativePoint.Y ) / screenscale;


            if (mouserD || mouseD)
            {

                float power = 200;
                temprX = (mouseX - mouseDX) * screenscale;
                temprY = (mouseY - mouseDY) * screenscale;

                temprY = Math.Clamp(temprY, -power, power);
                temprX = Math.Clamp(temprX, -power, power);

            }
            else if (keyfD)
            {

                temprX = (mouseX - mouseDX);
                temprY = (mouseY - mouseDY);

               

            }
            else
            {
                temprX = 0;
                temprY = 0;
            }

            if ((inpL && inpR) || ((!inpL && !inpR)))
            {
                player.mx = 0;
            }
            else if (inpL)
            {
                player.mx = -1;
            }
            else if (inpR)
            {
                player.mx = 1;
            }

            if ((inpU && inpD) || ((!inpU && !inpD)))
            {
                player.mz = 0;
            }
            else if (inpU)
            {
                player.mz = -1;
            }
            else if (inpD)
            {
                player.mz = 1;
            }


            if ((inpArrL && inpArrR) || ((!inpArrL && !inpArrR)))
            {
                Ry += 0;
            }
            else if (inpArrL)
            {
                Ry += 0.01f;
            }
            else if (inpArrR)
            {
                Ry -= 0.01f;
            }
            if ((inpArrU && inpArrD) || ((!inpArrU && !inpArrD)))
            {
                Rx += 0;
            }
            else if (inpArrU)
            {
                Rx -= 0.01f;
            }
            else if (inpArrD)
            {
                Rx += 0.01f;
            }
           // Rx =  Math.Clamp(Rx,-MathF.PI/2, MathF.PI / 2);
           // Ry =  Math.Clamp(Ry,-MathF.PI/2, MathF.PI / 2);
            label1.Text = distance.ToString();
            label2.Text = screenzoomV.ToString();
            label3.Text = player.z.ToString();
        }
        #endregion
    }
}
