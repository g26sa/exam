using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RabotaSIzobrazheniyami
{
    public class Form1 : Form
    {
        private PictureBox pictureBox;
        private Button btnOpen;
        private Button btnResize;
        private Button btnSave;

        private NumericUpDown nudWidth;
        private NumericUpDown nudHeight;
        private Label lblWidth;
        private Label lblHeight;

        private Image _currentImage;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Утилита для изображений";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(800, 500);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // PictureBox
            pictureBox = new PictureBox();
            pictureBox.Location = new Point(10, 10);
            pictureBox.Size = new Size(560, 480);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pictureBox);

            int panelX = 580;
            int startY = 10;
            int margin = 10;

            // Кнопка Open
            btnOpen = CreateButton("Открыть", panelX, startY, 200, 30);
            btnOpen.Click += BtnOpen_Click;

            // Подписи и поля ширины/высоты
            lblWidth = new Label();
            lblWidth.Text = "Ширина:";
            lblWidth.Location = new Point(panelX, startY + 50);
            lblWidth.AutoSize = true;
            this.Controls.Add(lblWidth);

            nudWidth = new NumericUpDown();
            nudWidth.Minimum = 10;
            nudWidth.Maximum = 5000;
            nudWidth.Value = 800;
            nudWidth.Location = new Point(panelX + 65, startY + 48);
            nudWidth.Size = new Size(80, 20);
            this.Controls.Add(nudWidth);

            lblHeight = new Label();
            lblHeight.Text = "Высота:";
            lblHeight.Location = new Point(panelX, startY + 80);
            lblHeight.AutoSize = true;
            this.Controls.Add(lblHeight);

            nudHeight = new NumericUpDown();
            nudHeight.Minimum = 10;
            nudHeight.Maximum = 5000;
            nudHeight.Value = 600;
            nudHeight.Location = new Point(panelX + 60, startY + 78);
            nudHeight.Size = new Size(80, 20);
            this.Controls.Add(nudHeight);

            // Кнопка Resize
            btnResize = CreateButton("Изменить", panelX, startY + 120, 200, 30);
            btnResize.Click += BtnResize_Click;

            // Кнопка Save
            btnSave = CreateButton("Сохранить", panelX, startY + 160, 200, 30);
            btnSave.Click += BtnSave_Click;
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

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Освобождаем предыдущую картинку, чтобы не держать файл
                        if (_currentImage != null)
                        {
                            _currentImage.Dispose();
                            _currentImage = null;
                        }

                        // Загружаем копию изображения из файла
                        using (var fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            _currentImage = Image.FromStream(fs);
                        }

                        pictureBox.Image = (Image)_currentImage.Clone();

                        nudWidth.Value = Math.Min((decimal)_currentImage.Width, nudWidth.Maximum);
                        nudHeight.Value = Math.Min((decimal)_currentImage.Height, nudHeight.Maximum);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при открытии изображения: " + ex.Message);
                    }
                }
            }
        }

        private void BtnResize_Click(object sender, EventArgs e)
        {
            if (_currentImage == null)
            {
                MessageBox.Show("Сначала откройте изображение.");
                return;
            }

            int newWidth = (int)nudWidth.Value;
            int newHeight = (int)nudHeight.Value;

            try
            {
                // Создаём новое изображение нужного размера
                var resized = new Bitmap(newWidth, newHeight);
                using (var g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(_currentImage, 0, 0, newWidth, newHeight);
                }

                // Освобождаем старое
                _currentImage.Dispose();
                _currentImage = resized;

                // Обновляем картинку в PictureBox
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                }
                pictureBox.Image = (Image)_currentImage.Clone();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при изменении размера: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_currentImage == null)
            {
                MessageBox.Show("Нет изображения для сохранения.");
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap Image|*.bmp";
                dialog.DefaultExt = "png";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var ext = Path.GetExtension(dialog.FileName).ToLowerInvariant();
                        System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;

                        if (ext == ".jpg" || ext == ".jpeg")
                            format = System.Drawing.Imaging.ImageFormat.Jpeg;
                        else if (ext == ".bmp")
                            format = System.Drawing.Imaging.ImageFormat.Bmp;

                        _currentImage.Save(dialog.FileName, format);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении изображения: " + ex.Message);
                    }
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (pictureBox.Image != null)
                pictureBox.Image.Dispose();

            if (_currentImage != null)
                _currentImage.Dispose();

            base.OnFormClosed(e);
        }
    }
}