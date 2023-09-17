using Microsoft.VisualBasic.Devices;
using System.Reflection;
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
            public float sy { get; set; }
            public Color color { get; set; }
            public bool grounged { get; set; }
        }
        public Player player = new Player() {x = 200,y = 200,sx = 40,sy = 120, color = Color.Yellow, grounged = false };
        public List<Block> block = new List<Block>();
       public List<Circle> circle = new List<Circle>();
        Random rng = new Random();
        Bitmap bmp;
        float mouseX, mouseY;
        float mouseDX, mouseDY;
        bool mouseD;
        bool mouserD;
        bool keyfD;
        bool inpU, inpD, inpL, inpR;
        float temprX;
        float temprY;
        float screenscale;
        float screenoffsetX = 0;
        float screenoffsetY = 0;
        #endregion

        //============START=AND=LOGIC========
        #region
        public Form1()
        {
            InitializeComponent();
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

            if (pictureBox1.Width > pictureBox1.Height)
            {
                bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                screenscale = bmp.Height / 1000f;

            }
            else
            {
                bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                screenscale = bmp.Width / 1000f;

            }
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.White);
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

                drawObj(g, block[i].x, block[i].y, block[i].sx, block[i].sy, block[i].color);

            }
            drawObj(g, player.x, player.y, player.sx, player.sy, player.color);


            if (mouserD || mouseD)
            {
                drawShp(g, new PointF[] { new PointF(player.x, player.y - (player.sy / 2f)), new PointF((player.x - temprX), (player.y - (player.sy / 2f) - temprY)) }, Color.Black, 3);

                drawCir(g, player.x - temprX, player.y - (player.sy / 2f) - temprY, 5, Color.BlueViolet);
            }
            if (keyfD)
            {
                PointF[] points = new PointF[] { new PointF { X = mouseX, Y = mouseY }, new PointF { X = mouseX, Y = mouseDY }, new PointF { X = mouseDX, Y = mouseDY }, new PointF { X = mouseDX, Y = mouseY } };
                drawShp(g, points, Color.Black, 3);

                drawCir(g, mouseDX, mouseDY, 5, Color.BlueViolet);
                drawCir(g, mouseX, mouseY, 5, Color.BlueViolet);
                drawCir(g, mouseX, mouseDY, 5, Color.BlueViolet);
                drawCir(g, mouseDX, mouseY, 5, Color.BlueViolet);
            }
            drawCir(g, mouseDX, mouseDY, 5, Color.BlueViolet);

            #endregion

            MovePlayer();
            drawShp(g, new PointF[] { new PointF(5, 5), new PointF(5, 995), new PointF(995, 995), new PointF(995, 5) }, Color.Black, 10);
            
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
            player.x += player.vx;
            player.y += player.vy;
            player.scrx = player.x + screenoffsetX;

            if(player.scrx < 1000 )
            {
               screenoffsetX += 0.05f * (1000 - player.scrx);
            }
            if(player.scrx > 1000)
            {
                screenoffsetX += 0.05f * (1000 - player.scrx);
            }
            if (player.grounged)
            {
                player.vx *= 0.95f;
                player.vy *= 0.99f;

            }
            else
            {
                player.vx *= 0.92f;
                player.vy *= 0.95f;

            }
            player.vy += 0.9f;


            label1.Text = player.scrx.ToString();
           // label2.Text = temprY.ToString();
            #region
            if (player.y > 1000 + (player.sy / 2f))
            {
                player.y = -(player.sy / 2) + 10;
                //player.vy *= -1 + bounce;
            }
            if (player.y < -(player.sy / 2f))
            {
                player.y = 1000 + (player.sy / 2f) - 10;
                // player.vy *= -1 + bounce;
            }
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


                if (((player.y + (player.sy / 2f) > block[t].y - (block[t].sy * 0.5f)) && ((player.y - (player.sy / 2f) < block[t].y + (block[t].sy * 0.5f)))) && ((player.x + (player.sx / 2f) > block[t].x - (block[t].sx * 0.5f)) && ((player.x - (player.sx / 2f) < block[t].x + (block[t].sx * 0.5f)))))
                {


                    if (((player.x > block[t].x - (block[t].sx * 0.5f)) && ((player.x < block[t].x + (block[t].sx * 0.5f)))))
                    {
                        if (player.y < block[t].y)
                        {
                            player.vy *= -1 + bounce;
                            player.y = block[t].y - (block[t].sy * 0.5f) - (player.sy / 2f);

                        }
                        else if (player.y > block[t].y)
                        {
                            player.vy *= -1 + bounce;
                            player.y = block[t].y + (block[t].sy * 0.5f) + (player.sy / 2f);
                        }


                    }
                    if (((player.y > block[t].y - (block[t].sy * 0.5f)) && ((player.y < block[t].y + (block[t].sy * 0.5f)))))
                    {
                        if (player.x < block[t].x)
                        {
                            player.vx *= -1 + bounce;
                            player.x = block[t].x - (block[t].sx * 0.5f) - (player.sx / 2f);

                        }
                        else if (player.x > block[t].x)
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

            if (circle[i].y > 1000 - circle[i].radi)
            {
                circle[i].health--;
                circle[i].y = 1000 - circle[i].radi;
                circle[i].vy *= -1 + bounce;
            }
            if (circle[i].y < circle[i].radi)
            {
                circle[i].health--;
                circle[i].y = circle[i].radi;
                circle[i].vy *= -1 + bounce;
            }
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




                    if ((circle[i].y > block[t].y - (block[t].sy * 0.5f)) && (circle[i].y < block[t].y + (block[t].sy * 0.5f)))
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


                    }
                    if ((circle[i].x > block[t].x - (block[t].sx * 0.5f)) && (circle[i].x < block[t].x + (block[t].sx * 0.5f)))
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
            g.FillEllipse(new SolidBrush(c),screenscale *( x - (r)) +(screenoffsetX * screenscale),screenscale *( y - (r)),screenscale *( r * 2f),screenscale *( r * 2f));
            g.DrawEllipse(new Pen(Color.Black, screenscale * 3),screenscale *( x - (r)) + (screenoffsetX * screenscale), screenscale * ( y - (r)),screenscale *( r * 2f),screenscale *( r * 2f));
        }
        void drawObj(Graphics g, float x, float y, float sx, float sy, Color color)
        {
            g.FillRectangle(new SolidBrush(color), screenscale * (x - (0.5f * sx)) + (screenoffsetX * screenscale), screenscale * (y - (0.5f * sy)), screenscale * sx, screenscale * sy);
            g.DrawRectangle(new Pen(Color.Black, screenscale * 3), screenscale * ( x - (0.5f * sx)) + (screenoffsetX * screenscale), screenscale * ( y - (0.5f * sy)), screenscale * sx, screenscale * sy);
        }
        void drawShp(Graphics g, PointF[] points1, Color color,int thick)
        {
            
            for (int i = 0; i < points1.Length; i++)
            {
                points1[i].X *= screenscale;
                points1[i].X += (screenoffsetX * screenscale);
                
                points1[i].Y *= screenscale;
            }
            if(thick == 0)
            {
                g.FillPolygon(new SolidBrush(color), points1);
            }
            {
                g.DrawPolygon(new Pen(color, screenscale * thick), points1);

            }
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

                tempC.vy = temprY / 10f;
                tempC.vx = temprX / 10f;
                tempC.color = Color.Aquamarine;
                tempC.health = 1;
                circle.Add(tempC);
                mouseD = false;

            }
            if (e.Button == MouseButtons.Right)
            {



                player.vy = -temprY / 6f;
                player.vx = -temprX / 6f;

                mouserD = false;
            }

        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            

        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Space || e.KeyCode == Keys.W) && player.grounged) { player.vy = -30; }
            if (e.KeyCode == Keys.E)
            {
                circle.Clear();
            }
            if (e.KeyCode == Keys.Q)
            {
                block.Clear();
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
           // if (e.KeyCode == Keys.W) { player.my = -1; }
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
                tempC.y =  (mouseY + mouseDY) / 2f ;
                tempC.x =  (mouseX + mouseDX) / 2f ;
                tempC.sx = MathF.Abs(temprX );
                if (tempC.sx < 10) { tempC.sx = 10; }
                tempC.sy = MathF.Abs( temprY );
                if (tempC.sy < 10) { tempC.sy = 10; }

                tempC.color = Color.Brown;

                block.Add(tempC);
                keyfD = false;
            }
            if (e.KeyCode == Keys.D) { inpR = false; }
            if (e.KeyCode == Keys.A) { inpL = false; }
         //   if (e.KeyCode == Keys.W) { player.my = 0; }
            if (e.KeyCode == Keys.S) { inpD = false; }
        }
        private void Inputs()
        {
            var relativePoint = PointToClient(Cursor.Position);
            
            if (!mouserD && !mouseD && !keyfD)
            {
                mouseX = (relativePoint.X - screenoffsetX) / screenscale;
                mouseY = (relativePoint.Y - screenoffsetY) / screenscale;
            }

            mouseDX = (relativePoint.X - screenoffsetX) / screenscale;
            mouseDY = (relativePoint.Y - screenoffsetY) / screenscale;


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
