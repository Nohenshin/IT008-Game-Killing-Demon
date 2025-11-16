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
    public partial class GameOverForm : Form
    {

        private Label labelTitle;
        private Label labelScore;
        private Button btnRetry;
        private Button btnExit;

        public bool Retry { get; private set; } = false;

        public GameOverForm(int score)
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.Opacity = 0.95;
            this.Size = new Size(360, 220);
            this.DoubleBuffered = true;

            // Title label
            labelTitle = new Label();
            labelTitle.Text = "GAME OVER";
            labelTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            labelTitle.ForeColor = Color.White;
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;
            labelTitle.AutoSize = false;
            labelTitle.Bounds = new Rectangle(20, 15, this.ClientSize.Width - 40, 50);
            this.Controls.Add(labelTitle);

            // Score label
            labelScore = new Label();
            labelScore.Text = "Score: " + score;
            labelScore.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
            labelScore.ForeColor = Color.White;
            labelScore.TextAlign = ContentAlignment.MiddleCenter;
            labelScore.AutoSize = false;
            labelScore.Bounds = new Rectangle(20, 70, this.ClientSize.Width - 40, 30);
            this.Controls.Add(labelScore);

            // Retry button
            btnRetry = new Button();
            btnRetry.Text = "Play Again";
            btnRetry.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btnRetry.Size = new Size(120, 36);
            btnRetry.Location = new Point(this.ClientSize.Width / 2 - btnRetry.Width - 10, 120);
            btnRetry.FlatStyle = FlatStyle.Flat;
            btnRetry.FlatAppearance.BorderSize = 0;
            btnRetry.BackColor = Color.FromArgb(50, 150, 50);
            btnRetry.ForeColor = Color.White;
            btnRetry.Click += BtnRetry_Click;
            this.Controls.Add(btnRetry);

            // Exit button
            btnExit = new Button();
            btnExit.Text = "Exit";
            btnExit.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            btnExit.Size = new Size(120, 36);
            btnExit.Location = new Point(this.ClientSize.Width / 2 + 10, 120);
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.BackColor = Color.FromArgb(180, 50, 50);
            btnExit.ForeColor = Color.White;
            btnExit.Click += BtnExit_Click;
            this.Controls.Add(btnExit);

            // Optional: rounded corners / border — keep simple for now

            // Make ESC close as Exit
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    Retry = false;
                    this.Close();
                }
            };
        }

        private void BtnRetry_Click(object sender, EventArgs e)
        {
            Retry = true;
            this.Close();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Retry = false;
            this.Close();
        }
    }
}

