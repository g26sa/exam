using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RabotaSVideo
{
    public class Form1 : Form
    {
        private TextBox txtPath;
        private Button btnBrowse;

        private Label lblSize;
        private Label lblCreated;
        private Label lblExtension;

        private Button btnOpenPlayer;
        private Button btnCopy;

        private string _currentFilePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Видео";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(600, 220);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            int margin = 10;

            // Label и TextBox для пути
            var lblPath = new Label();
            lblPath.Text = "Видеофайл:";
            lblPath.Location = new Point(10, 10);
            lblPath.AutoSize = true;
            this.Controls.Add(lblPath);

            txtPath = new TextBox();
            txtPath.Location = new Point(10, 30);
            txtPath.Size = new Size(460, 25);
            txtPath.ReadOnly = true;
            this.Controls.Add(txtPath);

            btnBrowse = CreateButton("Найти", 480, 30, 100, 25);
            btnBrowse.Click += BtnBrowse_Click;

            // Информация о файле
            lblSize = new Label();
            lblSize.Text = "Размер: -";
            lblSize.Location = new Point(10, 70);
            lblSize.AutoSize = true;
            this.Controls.Add(lblSize);

            lblCreated = new Label();
            lblCreated.Text = "Создано: -";
            lblCreated.Location = new Point(10, 95);
            lblCreated.AutoSize = true;
            this.Controls.Add(lblCreated);

            lblExtension = new Label();
            lblExtension.Text = "Формат: -";
            lblExtension.Location = new Point(10, 120);
            lblExtension.AutoSize = true;
            this.Controls.Add(lblExtension);

            // Кнопки действий
            btnOpenPlayer = CreateButton("Открыть в плеере", 10, 160, 170, 30);
            btnOpenPlayer.Click += BtnOpenPlayer_Click;

            btnCopy = CreateButton("Копировать в папку", 190, 160, 170, 30);
            btnCopy.Click += BtnCopy_Click;
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

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Video files|*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv|All files|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _currentFilePath = dialog.FileName;
                    txtPath.Text = _currentFilePath;
                    ShowFileInfo(_currentFilePath);
                }
            }
        }

        private void ShowFileInfo(string path)
        {
            try
            {
                var info = new FileInfo(path);

                double sizeMb = info.Length / (1024.0 * 1024.0);
                lblSize.Text = $"Размер: {sizeMb:F2} MB";

                lblCreated.Text = $"Создано: {info.CreationTime}";

                lblExtension.Text = $"Формат: {info.Extension}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении информации о файле: " + ex.Message);
            }
        }

        private void BtnOpenPlayer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentFilePath) || !File.Exists(_currentFilePath))
            {
                MessageBox.Show("Сначала выберите видеофайл.");
                return;
            }

            try
            {
                // Открываем файл в стандартном приложении Windows для этого типа
                Process.Start(new ProcessStartInfo
                {
                    FileName = _currentFilePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при запуске плеера: " + ex.Message);
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentFilePath) || !File.Exists(_currentFilePath))
            {
                MessageBox.Show("Сначала выберите видеофайл.");
                return;
            }

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку для резервной копии видеофайла";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string destDir = dialog.SelectedPath;
                        string fileName = Path.GetFileName(_currentFilePath);

                        string destPath = Path.Combine(destDir, fileName);

                        // Если файл уже есть, добавим суффикс
                        if (File.Exists(destPath))
                        {
                            string nameNoExt = Path.GetFileNameWithoutExtension(fileName);
                            string ext = Path.GetExtension(fileName);
                            destPath = Path.Combine(destDir, nameNoExt + "_copy" + ext);
                        }

                        File.Copy(_currentFilePath, destPath);
                        MessageBox.Show("Файл успешно скопирован:\n" + destPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при копировании файла: " + ex.Message);
                    }
                }
            }
        }
    }
}