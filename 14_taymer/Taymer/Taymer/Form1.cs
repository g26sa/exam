using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Taymer
{
    public class Form1 : Form
    {
        private NumericUpDown numMinutes;
        private NumericUpDown numSeconds;
        private Label lblMinutes;
        private Label lblSeconds;
        private Label lblTimeDisplay;

        private Button btnStart;
        private Button btnPause;
        private Button btnReset;

        private Timer timer;
        private int _timeLeftSeconds;
        private bool _isRunning = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Таймер обратного отсчёта";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(400, 220);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Минуты
            lblMinutes = new Label();
            lblMinutes.Text = "Минуты:";
            lblMinutes.Location = new Point(10, 10);
            lblMinutes.AutoSize = true;
            this.Controls.Add(lblMinutes);

            numMinutes = new NumericUpDown();
            numMinutes.Location = new Point(80, 8);
            numMinutes.Minimum = 0;
            numMinutes.Maximum = 600;
            numMinutes.Value = 1;
            numMinutes.Width = 70;
            this.Controls.Add(numMinutes);

            // Секунды
            lblSeconds = new Label();
            lblSeconds.Text = "Секунды:";
            lblSeconds.Location = new Point(170, 10);
            lblSeconds.AutoSize = true;
            this.Controls.Add(lblSeconds);

            numSeconds = new NumericUpDown();
            numSeconds.Location = new Point(240, 8);
            numSeconds.Minimum = 0;
            numSeconds.Maximum = 59;
            numSeconds.Value = 0;
            numSeconds.Width = 70;
            this.Controls.Add(numSeconds);

            // Отображение времени
            lblTimeDisplay = new Label();
            lblTimeDisplay.Text = "00:00";
            lblTimeDisplay.Location = new Point(10, 50);
            lblTimeDisplay.Size = new Size(360, 60);
            lblTimeDisplay.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            lblTimeDisplay.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblTimeDisplay);

            // Кнопки
            btnStart = CreateButton("Старт", 10, 130, 110, 30);
            btnStart.Click += BtnStart_Click;

            btnPause = CreateButton("Пауза", 140, 130, 110, 30);
            btnPause.Click += BtnPause_Click;

            btnReset = CreateButton("Сброс", 270, 130, 110, 30);
            btnReset.Click += BtnReset_Click;

            // Таймер
            timer = new Timer();
            timer.Interval = 1000; // 1 секунда [web:160][web:163]
            timer.Tick += Timer_Tick;

            UpdateTimeDisplayFromInputs();
        }

        private Button CreateButton(string text, int x, int y, int width, int height)
        {
            var button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(width, height);
            button.BackColor = Color.FromArgb(47, 79, 111);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            this.Controls.Add(button);
            return button;
        }

        private void UpdateTimeDisplayFromInputs()
        {
            int minutes = (int)numMinutes.Value;
            int seconds = (int)numSeconds.Value;
            _timeLeftSeconds = minutes * 60 + seconds;
            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            int minutes = _timeLeftSeconds / 60;
            int seconds = _timeLeftSeconds % 60;
            lblTimeDisplay.Text = $"{minutes:00}:{seconds:00}";
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (!_isRunning)
            {
                if (_timeLeftSeconds <= 0)
                {
                    UpdateTimeDisplayFromInputs();
                    if (_timeLeftSeconds <= 0)
                    {
                        MessageBox.Show("Установите время больше нуля.");
                        return;
                    }
                }

                timer.Start();
                _isRunning = true;
            }
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                timer.Stop();
                _isRunning = false;
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            timer.Stop();
            _isRunning = false;
            UpdateTimeDisplayFromInputs();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_timeLeftSeconds > 0)
            {
                _timeLeftSeconds--;
                UpdateTimeDisplay();
            }
            else
            {
                timer.Stop();
                _isRunning = false;
                SystemSounds.Beep.Play();
                MessageBox.Show("Время вышло!");
            }
        }
    }
}