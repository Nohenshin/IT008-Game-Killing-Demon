using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameKillingDemonn
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {

        }
    }

    public class Sprite
    {
        public Bitmap Sheet;
        public int FrameW, FrameH;

        public int CurFrame = 0;
        public int TotalFrames;

        public int FrameInterval = 80;
        int timer = 0;

        public bool FlipX = false;

        public Sprite(string path, int frameW, int frameH)
        {
            Sheet = new Bitmap(path);
            FrameW = frameW;
            FrameH = frameH;
            TotalFrames = Sheet.Width / frameW;
        }

        public void Reset()
        {
            CurFrame = 0;
            timer = 0;
        }

        public void Update(int dt)
        {
            timer += dt;
            if (timer >= FrameInterval)
            {
                timer -= FrameInterval;
                CurFrame++;

                if (CurFrame >= TotalFrames)
                    CurFrame = TotalFrames - 1; // animation dead/attack đứng ở cuối
            }
        }

        public void Draw(Graphics g, float x, float y)
        {
            Rectangle src = new Rectangle(CurFrame * FrameW, 0, FrameW, FrameH);

            if (!FlipX)
            {
                g.DrawImage(Sheet, x, y, src, GraphicsUnit.Pixel);
            }
            else
            {
                g.TranslateTransform(x + FrameW, y);
                g.ScaleTransform(-1, 1);
                g.DrawImage(Sheet, 0, 0, src, GraphicsUnit.Pixel);
                g.ResetTransform();
            }
        }
    }

   
}
