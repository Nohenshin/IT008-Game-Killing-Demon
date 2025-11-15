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

        bool left, right, jump, attack;
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
            if (e.KeyCode == Keys.Left) left = true;
            if (e.KeyCode == Keys.Right) right = true;
            if (e.KeyCode == Keys.Up) jump = true;
            if (e.KeyCode == Keys.X) attack = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) left = false;
            if (e.KeyCode == Keys.Right) right = false;
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

            player.Update(left, right, jump, dt);
            enemy.Update(dt);

            // player tấn công enemy
            if (player.isAttacking && !enemy.IsDead && !enemy.IsRemoved)
            {
                if (player.AttackBox.IntersectsWith(enemy.HitBox))
                    enemy.TakeDamage(1);
            }
            else if (!player.isAttacking && !enemy.IsDead && !enemy.IsRemoved)
            {
                if (player.HitBox.IntersectsWith(enemy.HitBox))
                    player.OnCollisionWithEnemy();
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

    public class Sprite
    {
        public Bitmap Sheet;
        public int FrameW, FrameH;

        public int CurFrame = 0;
        public int TotalFrames;

        public int FrameInterval = 400;
        int timer = 0;

        public bool FlipX = false;
        public bool Loop = true; // true: quay vòng; false: dừng ở cuối (attack, die)

        public Sprite(string path, int frameW, int frameH, bool loop = true)
        {
            Sheet = new Bitmap(path);
            FrameW = frameW;
            FrameH = frameH;
            TotalFrames = Sheet.Width / frameW;
            Loop = loop;
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
                {
                    if (Loop)
                    {
                        CurFrame = 0;
                    }
                    else
                    {
                        CurFrame = TotalFrames - 1; // dừng ở cuối
                    }
                }
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


    public class Player
    {
        public float X, Y;
        public float vy = 0;
        public bool isDead = false;
        public bool isRemove = false;

        int facing = 1;

        public Sprite idle;
        public Sprite run;
        public Sprite attack;
        public Sprite die;

        public Sprite current;

        public bool isAttacking = false;

        const float GRAVITY = 0.04f;
        const float JUMP_FORCE = -0.75f;

        public int Width => current.FrameW;
        public int Height => current.FrameH;

        public Player()
        {
            idle = new Sprite("Assets/Player/idle.png", 64, 80, loop: true);
            run = new Sprite("Assets/Player/run.png", 80, 80, loop: true);
            attack = new Sprite("Assets/Player/attack.png", 96, 80, loop: false);
            die = new Sprite("Assets/Player/dead.png", 78, 68, loop: false);

            current = idle;

            X = 150;
            Y = GameForm.GROUND_Y - current.FrameH - 100;
        }

        public Rectangle HitBox =>
            new Rectangle((int)X, (int)Y, Width / 2, Height);

        public Rectangle AttackBox =>
            new Rectangle(
                facing == 1 ? (int)(X + Width - 20) : (int)(X - 20),
                (int)Y + Height / 3,
                20,
                Height / 3);

        public void StartAttack()
        {
            if (!isAttacking)
            {
                isAttacking = true;
                current = attack;
                current.Reset();
            }
        }

        public void OnCollisionWithEnemy()
        {
            isDead = true;
            current = die;
            
        }

        public void Update(bool left, bool right, bool jump, int dt)
        {
            float speed = 0.15f * dt;

            if (!isAttacking && !isDead)
            {
                if (left) { X -= speed; facing = -1; }
                if (right) { X += speed; facing = 1; }

                if (jump && IsOnGround())
                    vy = JUMP_FORCE * dt;
            }

            // gravity
            vy += GRAVITY * dt;
            Y += vy;

            // floor collision
            if (Y > GameForm.GROUND_Y - Height)
            {
                Y = GameForm.GROUND_Y - Height;
                vy = 0;
            }

            // animation logic
            if (isAttacking && !isDead)
            {
                // đảm bảo attack bắt đầu từ đầu
                current.Update(dt);

                if (current.CurFrame == current.TotalFrames - 1)
                {
                    isAttacking = false;
                    current = idle;
                    current.Reset();
                }
            }
            else if(!isAttacking && !isDead)
            {
                Sprite target = (left || right) ? run : idle;

                if (current != target)
                {
                    // giữ bottom anchor khi frame height khác nhau
                    float bottom = Y + current.FrameH;
                    current = target;
                    current.Reset();
                    Y = bottom - current.FrameH;
                }

                current.Update(dt);
            }
            else
            {
                current.Update(dt);

                if(current.CurFrame >= current.TotalFrames - 1)
                {
                    isRemove = true;
                }
            }

                current.FlipX = facing == -1;
        }

        private bool IsOnGround()
        {
            return Y >= GameForm.GROUND_Y - Height - 1;
        }

        public void Draw(Graphics g)
        {
            if(!isRemove)
                current.Draw(g, X, Y);
        }
    }

    public class Enemy
    {
        public float X, Y;
        int dir = -1;

        int hp = 3;

        public bool IsDead = false;
        public bool IsRemoved = false;

        public Sprite move;
        public Sprite die;
        public Sprite current;

        public Enemy()
        {
            move = new Sprite("Assets/Enemy/moveE.png", 137, 97, loop: true);
            die = new Sprite("Assets/Enemy/deadE.png", 143, 98, loop: false);

            current = move;

            X = 450;
            Y = GameForm.GROUND_Y - move.FrameH - 20;
        }

        public Rectangle HitBox =>
            new Rectangle((int)X, (int)Y, current.FrameW / 2, current.FrameH);

        public void TakeDamage(int d)
        {
            if (IsDead) return;

            hp -= d;

            if (hp <= 0)
            {
                IsDead = true;

                // CHỈ GÁN 1 LẦN DUY NHẤT
                current = die;
                current.Reset();

                // giữ đáy khi đổi animation frame-height khác
                float bottom = Y + move.FrameH;
                Y = bottom - die.FrameH;
            }
        }

        public void Update(int dt)
        {
            if (IsRemoved) return;

            if (!IsDead)
            {
                // Update chỉ move
                X += dir * 0.08f * dt;

                if (X > 550)
                    dir *= -1;

                move.FlipX = dir == 1;

                // CHỈ update sprite, KHÔNG gán lại current
                current = move;
                current.Update(dt);
            }
            else
            {
                die.FlipX = dir == -1;
                // Enemy chết → chỉ chạy die animation
                current.Update(dt);

                // khi frame cuối → remove
                if (current.CurFrame >= current.TotalFrames - 1)
                    IsRemoved = true;
            }
        }

        public void Draw(Graphics g)
        {
            if (!IsRemoved)
                current.Draw(g, X, Y);
        }
    }



}

//run 80x80
//idle 64x80
//attack 90x80
//die 75x64