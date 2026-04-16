using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace RabotaSAudio
{
    public class Form1 : Form
    {
        private TextBox txtPath;
        private Button btnBrowse;

        private Label lblSize;
        private Label lblExtension;

        private Button btnPlay;
        private Button btnStop;
        private Button btnCopy;

        private string _currentFilePath;
        private SoundPlayer _player;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Утилита для аудиофайлов";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(600, 230);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Путь к файлу
            var lblPath = new Label();
            lblPath.Text = "Аудиофайл:";
            lblPath.Location = new Point(10, 10);
            lblPath.AutoSize = true;
            this.Controls.Add(lblPath);

            txtPath = new TextBox();
            txtPath.Location = new Point(10, 30);
            txtPath.Size = new Size(460, 25);
            txtPath.ReadOnly = true;
            this.Controls.Add(txtPath);

            btnBrowse = CreateButton("Обзор", 480, 30, 100, 25);
            btnBrowse.Click += BtnBrowse_Click;

            // Информация о файле
            lblSize = new Label();
            lblSize.Text = "Размер: -";
            lblSize.Location = new Point(10, 70);
            lblSize.AutoSize = true;
            this.Controls.Add(lblSize);

            lblExtension = new Label();
            lblExtension.Text = "Расширение: -";
            lblExtension.Location = new Point(10, 95);
            lblExtension.AutoSize = true;
            this.Controls.Add(lblExtension);

            // Кнопки управления
            btnPlay = CreateButton("Воспроизвести", 10, 130, 170, 30);
            btnPlay.Click += BtnPlay_Click;

            btnStop = CreateButton("Остановить", 190, 130, 170, 30);
            btnStop.Click += BtnStop_Click;

            btnCopy = CreateButton("Скопировать в папку", 370, 130, 210, 30);
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
                dialog.Filter = "Audio files|*.wav;*.mp3;*.wma;*.aac;*.flac|All files|*.*";
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
                lblExtension.Text = $"Расширение: {info.Extension}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении информации о файле: " + ex.Message);
            }
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentFilePath) || !File.Exists(_currentFilePath))
            {
                MessageBox.Show("Сначала выберите аудиофайл.");
                return;
            }

            // SoundPlayer поддерживает только .wav [web:129][web:137]
            if (!string.Equals(Path.GetExtension(_currentFilePath), ".wav", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Для воспроизведения через эту утилиту используйте файлы .wav.");
                return;
            }

            try
            {
                _player?.Stop();
                _player = new SoundPlayer(_currentFilePath);
                _player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при воспроизведении: " + ex.Message);
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            try
            {
                _player?.Stop();
            }
            catch
            {
                // игнорируем
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentFilePath) || !File.Exists(_currentFilePath))
            {
                MessageBox.Show("Сначала выберите аудиофайл.");
                return;
            }

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку для копии аудиофайла";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string destDir = dialog.SelectedPath;
                        string fileName = Path.GetFileName(_currentFilePath);
                        string destPath = Path.Combine(destDir, fileName);

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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                _player?.Stop();
                _player?.Dispose();
            }
            catch
            {
            }
            base.OnFormClosed(e);
        }
    }
}