using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameKillingDemonn
{
    public class Enemy
    {
        public float X, Y;
        int dir = -1;
        public int hp;
        float dir1;

        public bool IsDead = false;
        public bool IsRemoved = false;

        public Sprite move;
        public Sprite die;
        public Sprite current;

        public Enemy(float X, int hp, float dir1)
        {
            move = new Sprite("Assets/Enemy/moveE.png", 48, 32, loop: true);
            die = new Sprite("Assets/Enemy/deadE.png", 48, 32, loop: false);

            current = move;

            this.hp = hp;
            this.X = X;
            this.dir1 = dir1;
            Y = GameForm.GROUND_Y - move.FrameH - 15;
        }

        public Rectangle HitBox =>
            new Rectangle((int)X, (int)Y, current.FrameW / 2, current.FrameH);

        public bool getIsDead()
        {
            return IsDead;
        }



        public bool getIsRemove()
        {
            return IsRemoved;
        }

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
                X += dir1 * 0.08f * dt;

                //if (X > 550)
                //    dir *= -1;

                move.FlipX = dir == 1;

                // CHỈ update sprite, KHÔNG gán lại current
                current = move;
                current.Update(dt);
            }
            else
            {
                die.FlipX = dir == 1;
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
