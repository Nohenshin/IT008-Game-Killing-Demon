using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameKillingDemonn
{
    public partial class GameForm : Form
    {
        public const int GROUND_Y = 446; // mặt đất

        Player player;
        Enemy enemy;
        GameMap map;

        Bitmap backBuffer;
        Graphics backG;

        Stopwatch timer = new Stopwatch();
        long last;

        bool left, right, jump, attack, fastRight, fastLeft;
        public GameForm()
        {
            this.ClientSize = new Size(800, 480);
            this.DoubleBuffered = true;
            this.Text = "2D WinForms Game";

            // Load game objects
            map = new GameMap();
            player = new Player();
            enemy = new Enemy();

            // Double buffer
            backBuffer = new Bitmap(800, 480);
            backG = Graphics.FromImage(backBuffer);

            // Events
            this.Paint += OnPaint;
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;

            // Game loop
            timer.Start();
            last = timer.ElapsedMilliseconds;

            Timer loop = new Timer();
            loop.Interval = 1;
            loop.Tick += GameLoop;
            loop.Start();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                left = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                right = true;
            }
            if(e.KeyCode == Keys.S)
            {
                if (left) fastLeft = true;
                if (right) fastRight = true;
            }
            if (e.KeyCode == Keys.Up) jump = true;
            if (e.KeyCode == Keys.X) attack = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                left = false;
                fastLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                right = false;
                fastRight = false;
            }
            if (e.KeyCode == Keys.S) fastRight = fastLeft = false;
            if (e.KeyCode == Keys.Up) jump = false;
            if (e.KeyCode == Keys.X) attack = false;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            long now = timer.ElapsedMilliseconds;
            int dt = (int)(now - last);
            last = now;

            if (attack)
                player.StartAttack();

            player.Update(left, right, jump, dt, fastLeft, fastRight);
            enemy.Update(dt);

            // player tấn công enemy
            if (player.isAttacking && !enemy.IsDead && !enemy.IsRemoved)
            {
                if (player.AttackBox.IntersectsWith(enemy.HitBox))
                    enemy.TakeDamage(1);
            }
            else if (!player.isAttacking && !enemy.IsDead && !enemy.IsRemoved)
            {
                //if (player.HitBox.IntersectsWith(enemy.HitBox))
                    //player.OnCollisionWithEnemy();
            }

            if (enemy.getIsRemove())
            {
                for(int i = 0; i < 2; i++)
                    enemy = new Enemy();
            }

                Invalidate();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            backG.Clear(Color.Black);

            map.Draw(backG);
            enemy.Draw(backG);
            player.Draw(backG);

            e.Graphics.DrawImage(backBuffer, 0, 0);
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
        }

    }
}
