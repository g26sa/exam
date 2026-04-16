using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Risovalka
{
    public class Form1 : Form
    {
        private PictureBox canvas;
        private Button btnColor;
        private Button btnClear;
        private Button btnSave;
        private Label lblThickness;
        private NumericUpDown numThickness;

        private Bitmap _bitmap;
        private bool _isDrawing = false;
        private Point _lastPoint;
        private Pen _pen;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Простая рисовалка";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(800, 500);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Поле рисования
            canvas = new PictureBox();
            canvas.Location = new Point(10, 10);
            canvas.Size = new Size(600, 480);
            canvas.BackColor = Color.White;
            canvas.BorderStyle = BorderStyle.FixedSingle;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            this.Controls.Add(canvas);

            // Кнопка выбора цвета
            btnColor = CreateButton("Цвет...", 630, 20, 140, 30);
            btnColor.Click += BtnColor_Click;

            // Толщина
            lblThickness = new Label();
            lblThickness.Text = "Толщина:";
            lblThickness.Location = new Point(630, 70);
            lblThickness.AutoSize = true;
            this.Controls.Add(lblThickness);

            numThickness = new NumericUpDown();
            numThickness.Location = new Point(700, 68);
            numThickness.Minimum = 1;
            numThickness.Maximum = 20;
            numThickness.Value = 3;
            numThickness.Width = 70;
            numThickness.ValueChanged += NumThickness_ValueChanged;
            this.Controls.Add(numThickness);

            // Очистить
            btnClear = CreateButton("Очистить", 630, 110, 140, 30);
            btnClear.Click += BtnClear_Click;

            // Сохранить
            btnSave = CreateButton("Сохранить как...", 630, 150, 140, 30);
            btnSave.Click += BtnSave_Click;

            // Инициализация битмапа и пера
            _bitmap = new Bitmap(canvas.Width, canvas.Height);
            canvas.Image = _bitmap;

            _pen = new Pen(Color.Black, (float)numThickness.Value);
            _pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            _pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
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

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDrawing = true;
                _lastPoint = e.Location;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing)
                return;

            if (e.Button == MouseButtons.Left)
            {
                using (Graphics g = Graphics.FromImage(_bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.DrawLine(_pen, _lastPoint, e.Location);
                }
                _lastPoint = e.Location;
                canvas.Invalidate(); // перерисовать PictureBox [web:154]
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDrawing = false;
            }
        }

        private void BtnColor_Click(object sender, EventArgs e)
        {
            using (var dialog = new ColorDialog())
            {
                dialog.Color = _pen.Color;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _pen.Color = dialog.Color;
                }
            }
        }

        private void NumThickness_ValueChanged(object sender, EventArgs e)
        {
            _pen.Width = (float)numThickness.Value;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                g.Clear(Color.White);
            }
            canvas.Invalidate();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap Image|*.bmp";
                dialog.FileName = "risunok.png";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ImageFormat format = ImageFormat.Png;
                        string ext = System.IO.Path.GetExtension(dialog.FileName).ToLowerInvariant();
                        if (ext == ".jpg" || ext == ".jpeg")
                            format = ImageFormat.Jpeg;
                        else if (ext == ".bmp")
                            format = ImageFormat.Bmp;

                        _bitmap.Save(dialog.FileName, format);
                        MessageBox.Show("Изображение сохранено:\n" + dialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                    }
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _pen?.Dispose();
            _bitmap?.Dispose();
            base.OnFormClosed(e);
        }
    }
}