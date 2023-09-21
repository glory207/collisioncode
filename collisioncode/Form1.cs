using Microsoft.VisualBasic.Devices;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography.Pkcs;
using System.Windows.Forms;

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
            public float sx { get; set; }
            public float sy { get; set; }
            public Color color { get; set; }
        }
        public class Player
        {
            public float x { get; set; }
            public float y { get; set; }
            public float vx { get; set; }
            public float vy { get; set; }
            public float mx { get; set; }
            public float my { get; set; }
            public float sx { get; set; }
            public float scrx { get; set; }
            public float scry { get; set; }
            public float sy { get; set; }
            public Color color { get; set; }
            public bool grounged { get; set; }
        }
        public Player player = new Player() {x = 500,y = 200,sx = 100,sy = 250, color = Color.Yellow, grounged = false };
        public List<Block> block = new List<Block>();
       public List<Circle> circle = new List<Circle>();
        Random rng = new Random();
        Bitmap bmp = new Bitmap(800, 800);
        float mouseX, mouseY;
        float mouseDX, mouseDY;
        bool mouseD;
        bool mouserD;
        bool keyfD;
        bool inpU, inpD, inpL, inpR;
        float temprX;
        float temprY;
        float screenscale;
        float screenscalex;
        float screenscaley;
        float screenzoom = 2500f;
        float screenzoomV;
        float screenoffsetX = 0;
        float screenoffsetY = 0;
        
        float Rx = 0.2f;
        float Ry = 0;
        float Rz = 0;
        float dis = 0;
        #endregion

        //============START=AND=LOGIC========
        #region
        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += MouseScrole;
         //   Cursor.Hide();
           
            Block tempC = new Block();
            tempC.y = 900;
            tempC.x = 500;
            tempC.sx = 1050;
            tempC.sy = 100;

            tempC.color = Color.Brown;

            block.Add(tempC);
        }
        void Gooo()
        {
            //test
            
            screenscale = bmp.Height / screenzoom;

            screenscalex = bmp.Width  / 1000f;
            screenscaley = bmp.Height / 1000f;
           
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(new SolidBrush(Color.Gray), 0, 0, screenscalex * 1000, screenscaley * 1000);
            #region
            for (int i = 0; i < circle.Count; i++)
            {

                getCollisions(i);
                CirclePhysics(i);

            }
            circle.RemoveAll(j => j.health < 1);
            for (int i = 0; i < circle.Count; i++)
            {
                #region
                drawCir(g, circle[i].x, circle[i].y, circle[i].radi, circle[i].color);
                // float lnx = Math.Clamp((circle[i].vx * circle[i].radi * 0.1f), - circle[i].radi, circle[i].radi)*3f;
                // float lny = Math.Clamp((circle[i].vy * circle[i].radi * 0.1f), - circle[i].radi, circle[i].radi)*3f;
                // drawShp(g,new PointF[] {new PointF(circle[i].x, circle[i].y), new PointF( circle[i].x - lnx, circle[i].y - lny) }, Color.Black,10);
                #endregion
            }


            for (int i = 0; i < block.Count; i++)
            {

               // drawObj(g, block[i].x, block[i].y, block[i].sx, block[i].sy, block[i].color);
                SetVectors(g, Makepoint(block[i].x, block[i].y, block[i].sx, block[i].sy, 250f));
            }
            // drawObj(g, player.x, player.y, player.sx, player.sy, player.color);
            SetVectors(g, Makepoint(player.x, player.y, player.sx, player.sy, 100f));


            if (mouserD || mouseD)
            {
                drawShp(g, new PointF[] { new PointF(player.x, player.y - (player.sy / 2f)), new PointF((player.x - temprX), (player.y - (player.sy / 2f) - temprY)) }, Color.Black, 3);

                drawCir(g, player.x - temprX, player.y - (player.sy / 2f) - temprY, 5, Color.BlueViolet);
            }
            if (keyfD)
            {
                PointF[] points = new PointF[] { new PointF { X = mouseX - screenoffsetX, Y = mouseY - screenoffsetY }, new PointF { X = mouseX - screenoffsetX, Y = mouseDY - screenoffsetY }, new PointF { X = mouseDX - screenoffsetX, Y = mouseDY - screenoffsetY }, new PointF { X = mouseDX - screenoffsetX, Y = mouseY - screenoffsetY } };
                drawShp(g, points, Color.Black, 3);

                drawCir(g, mouseDX - screenoffsetX, mouseDY - screenoffsetY, 5, Color.BlueViolet);
                drawCir(g, mouseX - screenoffsetX, mouseY - screenoffsetY, 5, Color.BlueViolet);
                drawCir(g, mouseX - screenoffsetX, mouseDY - screenoffsetY, 5, Color.BlueViolet);
                drawCir(g, mouseDX - screenoffsetX, mouseY - screenoffsetY, 5, Color.BlueViolet);
            }
            drawCir(g, mouseDX - screenoffsetX, mouseDY - screenoffsetY, 5, Color.BlueViolet);

            #endregion

            MovePlayer();
            pictureBox1.Image = bmp;
            
        }
        private void UpdateTic(object sender, EventArgs e)
        {
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
            player.vy += player.my * 2;
            player.vx = Math.Clamp(player.vx,-40, 40);
            player.vy = Math.Clamp(player.vy,-40, 40);
            player.x += player.vx;
            player.y += player.vy;
            player.scrx = player.x + screenoffsetX;
            player.scry = player.y + screenoffsetY;
            //  screenoffsetX = (screenscalex -screenscaley) * 1000;
            
          if(player.scrx < (screenzoom*0.5f))
          {
             screenoffsetX += 0.05f * ((screenzoom * 0.5f) - player.scrx);
          }
          if(player.scrx > (screenzoom * 0.5f))
          {
              screenoffsetX += 0.05f * ((screenzoom * 0.5f) - player.scrx);
          } 
          if(player.scry < (screenzoom * 0.5f))
          {
              screenoffsetY += 0.05f * ((screenzoom * 0.5f) - player.scry);
          }
          if(player.scry > (screenzoom * 0.5f))
          {
              screenoffsetY += 0.05f * ((screenzoom * 0.5f) - player.scry);
          }
            // if (player.grounged)
            {
                player.vx *= 0.955f;
                player.vy *= 0.99f;

            }
            player.vy += 1f;


            label1.Text = screenscalex.ToString();
            label2.Text = screenscaley.ToString();
            label3.Text = (screenscalex- screenscaley).ToString();
           // label2.Text = temprY.ToString();
            #region
       //    if (player.y > 1000 + (player.sy / 2f))
            {
        //        player.y = -(player.sy / 2) + 10;
                //player.vy *= -1 + bounce;
            }
       //    if (player.y < -(player.sy / 2f))
       //    {
       //        player.y = 1000 + (player.sy / 2f) - 10;
       //        // player.vy *= -1 + bounce;
       //    }
          //  if (player.x > 1000 + (player.sx / 2f))
          //  {
          //      player.x = -(player.sx / 2f) + 10;
          //      //  player.vx *= -1 + bounce;
          //  }
          //  if (player.x < -(player.sx / 2f))
          //  {
          //      player.x = 1000 + (player.sx / 2f) - 10;
          //      // player.vx *= -1 + bounce;
          //  }
            #endregion

            player.grounged = false;
            for (int t = 0; t < block.Count; t++)
            {
                if (((player.y + (player.sy / 2f) + 3 > block[t].y - (block[t].sy * 0.5f)) && ((player.y - (player.sy / 2f) - 3 < block[t].y + (block[t].sy * 0.5f)))) && ((player.x + (player.sx / 2f) + 3 > block[t].x - (block[t].sx * 0.5f)) && ((player.x - (player.sx / 2f) - 3 < block[t].x + (block[t].sx * 0.5f)))))
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


                if (((player.y + (player.sy / 2f) >= block[t].y - (block[t].sy * 0.5f)) && ((player.y - (player.sy / 2f) <= block[t].y + (block[t].sy * 0.5f)))) && ((player.x + (player.sx / 2f) >= block[t].x - (block[t].sx * 0.5f)) && ((player.x - (player.sx / 2f) <= block[t].x + (block[t].sx * 0.5f)))))
                {
                   
                    float ydif = ((0.5f*player.sy) + (0.5f * block[t].sy)) - MathF.Abs (player.y - block[t].y);
                    float xdif = ((0.5f * player.sx) + (0.5f * block[t].sx)) - MathF.Abs(player.x - block[t].x);
                    
                    
                    if (ydif < xdif)
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
                   else{
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
            #region

        //   if (circle[i].y > 1000 - circle[i].radi)
        //   {
        //       circle[i].health--;
        //       circle[i].y = 1000 - circle[i].radi;
        //       circle[i].vy *= -1 + bounce;
        //   }
        //   if (circle[i].y < circle[i].radi)
        //   {
        //       circle[i].health--;
        //       circle[i].y = circle[i].radi;
        //       circle[i].vy *= -1 + bounce;
        //   }
          //  if (circle[i].x > 1000 - circle[i].radi)
          //  {
          //      circle[i].health--;
          //      circle[i].x = 1000 - circle[i].radi;
          //      circle[i].vx *= -1 + bounce;
          //  }
          //  if (circle[i].x < 0 + circle[i].radi)
          //  {
          //      circle[i].health--;
          //      circle[i].x = 0 + circle[i].radi;
          //      circle[i].vx *= -1 + bounce;
          //  }
            #endregion

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
        void drawCir(Graphics g,float x,float y,float r, Color c)
        {
            g.FillEllipse(new SolidBrush(c),screenscale *( x - (r)) +(screenoffsetX * screenscale),screenscale *( y - (r)) + (screenoffsetY * screenscale), screenscale *( r * 2f) , screenscale *( r * 2f));
            g.DrawEllipse(new Pen(Color.Black, screenscale * 3),screenscale *( x - (r)) + (screenoffsetX * screenscale),( screenscale * ( y - (r))) + (screenoffsetY * screenscale), screenscale *( r * 2f),screenscale *( r * 2f));
        }
        void drawObj(Graphics g, float x, float y, float sx, float sy, Color color)
        {
            g.FillRectangle(new SolidBrush(color), screenscale * (x - (0.5f * sx)) + (screenoffsetX * screenscale), (screenscale * (y - (0.5f * sy))) + (screenoffsetY * screenscale), screenscale * sx, screenscale * sy);
            g.DrawRectangle(new Pen(Color.Black, screenscale * 3), screenscale * ( x - (0.5f * sx)) + (screenoffsetX * screenscale),( screenscale * ( y - (0.5f * sy))) + (screenoffsetY * screenscale), screenscale * sx, screenscale * sy);
        }
        void drawShp(Graphics g, PointF[] points1, Color color,int thick)
        {
            
            for (int i = 0; i < points1.Length; i++)
            {
                points1[i].X *= screenscale;
                points1[i].X += (screenoffsetX * screenscale);
                
                points1[i].Y *= screenscale;
                points1[i].Y += (screenoffsetY * screenscale);

            }
            if (thick == 0)
            {
                g.FillPolygon(new SolidBrush(color), points1);
            }
            {
                g.DrawPolygon(new Pen(color, screenscale * thick), points1);

            }
        }
        #endregion
        //===========3D================
        #region

        Vector3[] Makepoint(float px, float py, float pw, float ph, float depth)
        {
            px *= 0.001f;
            py *= 0.001f;
            pw *= 0.0005f;
            ph *= 0.0005f;
         depth *= 0.0005f;

            Vector3[] madePointes = new Vector3[8]
            {
                new Vector3(px - pw, py - ph,dis -depth),
                new Vector3(px + pw, py - ph,dis -depth),
                new Vector3(px + pw, py + ph,dis -depth),
                new Vector3(px - pw, py + ph,dis -depth),

                new Vector3(px - pw, py - ph,dis + depth),
                new Vector3(px + pw, py - ph,dis + depth),
                new Vector3(px + pw, py + ph,dis + depth),
                new Vector3(px - pw, py + ph,dis + depth)

            };

            Vector3[] points = new Vector3[8] {
         new Vector3(-1.5f, -0.5f, -0.5f),
         new Vector3(0.5f, -0.5f, -0.5f),
         new Vector3(0.5f, 0.5f, -0.5f),
         new Vector3(-1.5f, 0.5f, -0.5f),

         new Vector3(-1.5f, -0.5f, 0.5f),
         new Vector3(0.5f, -0.5f, 0.5f),
         new Vector3(0.5f, 0.5f, 0.5f),
         new Vector3(-1.5f, 0.5f, 0.5f) };
            return madePointes;
        }
        void SetVectors(Graphics g, Vector3[] points)
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

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 rotated = matmul(rotationY, points[i]);
                rotated = matmul(rotationX, rotated);
                rotated = matmul(rotationZ, rotated);
                float distance = 2;
                float z = 1 / (distance - rotated.Z);
                float[,] projection = new float[,] {
                  { z, 0, 0},
                  {0, z, 0},
                  {0, 0, 0},
                };
                Vector3 projected2d = matmul(projection, rotated);

                projected2d *= (2000);
                projected[i] = projected2d;
                //point(projected2d.x, projected2d.y);
            }

          //  for (int i = 0; i < projected.Length; i++)
          //  {
          //      //   stroke(255);
          //      //   strokeWeight(16);
          //      //   noFill();
          //      Vector3 v = projected[i];
          //      // point(v.x, v.y);
          //      g.FillEllipse(new SolidBrush(Color.RebeccaPurple), screenscale * (v.X + 500f), screenscale * (v.Y + 500f), 5, 5);
          //  }

            // Connecting
            for (int i = 0; i < 4; i++)
            {
                connect(i, (i + 1) % 4, projected, g);
                connect(i + 4, ((i + 1) % 4) + 4, projected, g);
                connect(i, i + 4, projected, g);
            }

            // angle += 0.03f;
        }
        Vector3 matmul(float[,] matrix, Vector3 point)
        {
            Vector3 newPoint;
            newPoint.X = (point.X * matrix[0, 0]) + (point.Y * matrix[1, 0]) + (point.Z * matrix[2, 0]);
            newPoint.Y = (point.X * matrix[0, 1]) + (point.Y * matrix[1, 1]) + (point.Z * matrix[2, 1]);
            newPoint.Z = (point.X * matrix[0, 2]) + (point.Y * matrix[1, 2]) + (point.Z * matrix[2, 2]);
            return newPoint;
        }
        void connect(int i, int j, Vector3[] points, Graphics g)
        {
            Vector3 a = points[i];
            Vector3 b = points[j];
            // strokeWeight(1);
            // stroke(255);
            //g.DrawLine(new Pen(Color.Black, 1), screenscale * (a.X + 500f), screenscale * (a.Y + 500f), screenscale * (b.X + 500f), screenscale * (b.Y + 500f));
            drawShp(g,new PointF[] {new PointF(a.X,a.Y), new PointF(b.X, b.Y) },Color.Black,3);
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
            if ((e.KeyCode == Keys.Space || e.KeyCode == Keys.W) && player.grounged) { player.vy = -40; }
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
          //  if (e.KeyCode == Keys.W) { inpU = true; }
            if (e.KeyCode == Keys.S) { inpD = true; }
            if (e.KeyCode == Keys.F)
            { 
            keyfD = true;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F) 
            {
                Block tempC = new Block();
                tempC.y =  ((mouseY + mouseDY) / 2f ) - screenoffsetY;
                tempC.x =  ((mouseX + mouseDX) / 2f ) - screenoffsetX;
                tempC.sx = MathF.Abs(temprX );
                if (tempC.sx < 100) { tempC.sx = 100; }
                tempC.sy = MathF.Abs( temprY );
                if (tempC.sy < 100) { tempC.sy = 100; }

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
          //  if (e.KeyCode == Keys.W) { inpU = false; }
            if (e.KeyCode == Keys.S) { inpD = false; }
        }
        private void Inputs()
        {
            var relativePoint = PointToClient(Cursor.Position);
            screenzoomV *= 0.9f;
            screenzoom += screenzoomV;
            screenzoom = Math.Clamp(screenzoom, 1000, 5000);

            // screenzoom = Math.Clamp(screenzoom, 1500, 2000);

            if (!mouserD && !mouseD && !keyfD)
            {
               // mouseX = (relativePoint.X - screenoffsetX) / screenscale;
               // mouseY = (relativePoint.Y - screenoffsetY) / screenscale;
                mouseX = (relativePoint.X ) / screenscale;
                mouseY = (relativePoint.Y ) / screenscale;
            }

           // mouseDX = (relativePoint.X - screenoffsetX) / screenscale;
           // mouseDY = (relativePoint.Y - screenoffsetY) / screenscale;
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
                player.my = 0;
            }
            else if (inpU)
            {
                player.my = -1;
            }
            else if (inpD)
            {
                player.my = 1;
            }
        }
        #endregion
    }
}
