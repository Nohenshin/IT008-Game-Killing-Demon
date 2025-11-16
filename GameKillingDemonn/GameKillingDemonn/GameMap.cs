using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameKillingDemonn
{
    public class GameMap
    {
        public Bitmap Background;

        public GameMap()
        {
            Background = new Bitmap("Assets/Background/background.jpg");
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(Background, 0, 0, 800, 480);
        }
    }
}
