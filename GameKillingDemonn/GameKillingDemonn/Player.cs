using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameKillingDemonn
{
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

        public void Update(bool left, bool right, bool jump, int dt, bool fast)
        {
            if (isRemove) return;
            float normalSpeed = 0.15f * dt;
            float fastSpeed = 0.35f * dt;

            if (!isAttacking && !isDead)
            {
                float speed = fast ? fastSpeed : normalSpeed;
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
            else if (!isAttacking && !isDead)
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

                if (current.CurFrame >= current.TotalFrames - 1)
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
            if (!isRemove)
                current.Draw(g, X, Y);
        }
    }
}
