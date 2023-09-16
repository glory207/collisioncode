using Microsoft.VisualBasic.Devices;
using System.Reflection;

namespace collisioncode
{
    public partial class Form1 : Form
    {
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
            public float sy { get; set; }
            public Color color { get; set; }
            public bool grounged { get; set; }
        }
        public Player player = new Player() {x = 200,y = 200,sx = 20,sy = 60, color = Color.Yellow, grounged = false };
        public List<Block> block = new List<Block>();
       public List<Circle> circle = new List<Circle>();
        Random rng = new Random();
        Bitmap bmp;
        float mouseX, mouseY;
        float mouseDX, mouseDY;
        #endregion
        public Form1()
        {
            InitializeComponent();
            
        }
        private void UpdateTic(object sender, EventArgs e)
        {
            
            Gooo();

        }
        void CirclePhysics(int i)
        {
            // float bounce = (6f*rng.NextSingle()/10f) -0.1f;
            
            circle[i].vy += 0.9f;
           // circle[i].vx *= 0.99f;
           // circle[i].vy *= 0.99f;
            if (circle[i].vx < 0.1 && circle[i].vx > -0.1) { circle[i].vx = 0; }

            circle[i].vy = Math.Clamp(circle[i].vy,-40,40);
            circle[i].vx = Math.Clamp(circle[i].vx,-40,40);
            circle[i].v = Math.Clamp(MathF.Sqrt((circle[i].vy * circle[i].vy) + (circle[i].vx * circle[i].vx)),0,10)/10f;

            circle[i].x += circle[i].vx;
            circle[i].y += circle[i].vy;
        }
        void drawCir(Graphics g,float x,float y,float r, Color c)
        {
            g.FillEllipse(new SolidBrush(c), x - (r), y - (r), r * 2f, r * 2f);
            g.DrawEllipse(new Pen(Color.Black,3), x - (r), y - (r), r * 2f, r * 2f);
        }
        void drawObj(Graphics g, float x, float y, float sx, float sy, Color color)
        {
            g.FillRectangle(new SolidBrush(color), x - (0.5f * sx), y - (0.5f * sy), sx, sy);
            g.DrawRectangle(new Pen(Color.Black, 3), x - (0.5f * sx), y - (0.5f * sy), sx, sy);
        }
        void Gooo()
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.DrawRectangle(new Pen(Color.Black, 3), 0, 0, bmp.Width, bmp.Height);
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
                float lnx = Math.Clamp((circle[i].vx * circle[i].radi * 0.1f), - circle[i].radi, circle[i].radi);
                float lny = Math.Clamp((circle[i].vy * circle[i].radi * 0.1f), - circle[i].radi, circle[i].radi);
                g.DrawLine(new Pen(Color.Black, 3), circle[i].x, circle[i].y, circle[i].x - lnx, circle[i].y - lny);
                #endregion
            }
            

            drawCir(g, mouseX, mouseY, 5, Color.BlueViolet);
            for (int i = 0; i < block.Count; i++)
            {
                
                drawObj(g, block[i].x, block[i].y, block[i].sx, block[i].sy, block[i].color);
                
            }
            g.DrawLine(new Pen(Color.Black,3),mouseX,mouseY,mouseDX,mouseDY);
            #endregion

            MovePlayer();
            drawObj(g, player.x, player.y, player.sx, player.sy, player.color);

            pictureBox1.Image = bmp;
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
              
            if (circle[i].y > bmp.Height - circle[i].radi)
            {
                circle[i].health--;
                circle[i].y = bmp.Height - circle[i].radi;
                circle[i].vy *= -1 + bounce;
            }
            if (circle[i].y < circle[i].radi)
            {
                circle[i].health--;
                circle[i].y = circle[i].radi;
                circle[i].vy *= -1 + bounce;
            }
            if (circle[i].x > bmp.Width - circle[i].radi)
            {
                circle[i].health--;
                circle[i].x = bmp.Width - circle[i].radi;
                circle[i].vx *= -1 + bounce;
            }
            if (circle[i].x < 0 + circle[i].radi)
            {
                circle[i].health--;
                circle[i].x = 0 + circle[i].radi;
                circle[i].vx *= -1 + bounce;
            }
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
        void MakeCir()
        {
            for (int i = 0; i < circle.Count; i++)
            {
                circle[i].y = rng.Next(0, pictureBox1.Height);
                circle[i].x = rng.Next(0, pictureBox1.Width);
                circle[i].radi = 10;
               // circle[i].radi = rng.Next(20, 60);
                circle[i].vy = rng.Next(-10, 0);
                circle[i].vx = rng.Next(-20, 20);
                circle[i].color = Color.Aquamarine;
                circle[i].health = 3;
                //circle[i].color = Color.FromArgb(rng.Next(1, 225), rng.Next(1, 225), rng.Next(1, 225));
            }
        }
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
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && player.grounged) { player.vy = -20; }
            if (e.KeyCode == Keys.Back) { MakeCir(); }
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
            mouserD = true;
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Circle tempC = new Circle();
                tempC.y = mouseY;
                tempC.x = mouseX;
                tempC.radi = 10;

                tempC.vy = (mouseY - mouseDY)/5f;
                tempC.vx = (mouseX - mouseDX)/5f;
                tempC.color = Color.Aquamarine;
                tempC.health = 5;
                circle.Add(tempC);
                mouseD = false;

            }
            if (e.Button == MouseButtons.Right)
            {
                
                float power = 500;
                float tempX = (mouseX - mouseDX);
                tempX = Math.Clamp(tempX, -power, power) ;
                float tempY = (mouseY - mouseDY);
                tempY = Math.Clamp(tempY, -power, power) ;
                player.vy = tempY / 10f;
                player.vx = tempX / 10f;

                mouserD = false;
            }

        }
        bool mouseD;
        bool mouserD;
        bool inpU, inpD, inpL, inpR;
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F) 
            {
                Block tempC = new Block();
                tempC.y = (mouseY + mouseDY) / 2f;
                tempC.x = (mouseX + mouseDX) / 2f;
                tempC.sx = MathF.Abs(mouseX - mouseDX);
                if (tempC.sx < 10) { tempC.sx = 10; }
                tempC.sy = MathF.Abs(mouseY - mouseDY);
                if (tempC.sy < 10) { tempC.sy = 10; }

                tempC.color = Color.Brown;

                block.Add(tempC);
                mouserD = false;
            }
            if (e.KeyCode == Keys.D) { inpR = false; }
            if (e.KeyCode == Keys.A) { inpL = false; }
         //   if (e.KeyCode == Keys.W) { player.my = 0; }
            if (e.KeyCode == Keys.S) { inpD = false; }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouserD && !mouseD ) 
            {
                mouseX = e.X;
                mouseY = e.Y;
            }
            
            mouseDX = e.X;
            mouseDY = e.Y;
            
            
        }
        void MovePlayer()
        {
            if((inpL && inpR) || ((!inpL && !inpR)))
            {
                player.mx = 0;
            }
            else if(inpL)
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
            player.vx += player.mx;
            player.vy += player.my *2;
            player.x += player.vx;
            player.y += player.vy;
            if (player.grounged)
            {
            player.vx *= 0.9f;
            player.vy *= 0.9f;

            }
            else
            {
                player.vx *= 0.95f;
                player.vy *= 0.95f;

            }
            player.vy += 0.7f;

            if (mouseD )
            {
                mouseX = player.x;
                mouseY = player.y - (player.sy/2f);
            }
            

            #region
            if (player.y > bmp.Height + (player.sy/2f))
            {
                player.y = -(player.sy /2) +10;
                //player.vy *= -1 + bounce;
            }
            if (player.y < -(player.sy / 2f))
            {
                 player.y = bmp.Height + (player.sy/2f) - 10;
               // player.vy *= -1 + bounce;
            }
            if (player.x > bmp.Width + (player.sx / 2f))
            {
                player.x = -(player.sx / 2f) + 10;
              //  player.vx *= -1 + bounce;
            }
            if (player.x < - (player.sx / 2f))
            {
                 player.x = bmp.Width + (player.sx / 2f) - 10;
               // player.vx *= -1 + bounce;
            }
            #endregion
            
            player.grounged = false;
            for (int t = 0; t < block.Count; t++)
            {
                if (((player.y + (player.sy / 2f) +3 > block[t].y - (block[t].sy * 0.5f)) && ((player.y - (player.sy / 2f) -3 < block[t].y + (block[t].sy * 0.5f)))) && ((player.x + (player.sx / 2f) + 3 > block[t].x - (block[t].sx * 0.5f)) && ((player.x - (player.sx / 2f) - 3 < block[t].x + (block[t].sx * 0.5f)))))
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

    }
}
