using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OchistkaKesha
{
    public class Form1 : Form
    {
        private Label lblTempPath;
        private TextBox txtTempPath;

        private Button btnAnalyze;
        private Button btnClean;

        private Label lblFilesCount;
        private Label lblDirsCount;
        private Label lblSize;

        private Label lblDeletedFiles;
        private Label lblDeletedDirs;

        private TextBox txtLog;

        private long _totalBytes;
        private long _totalFiles;
        private long _totalDirs;

        private long _deletedFiles;
        private long _deletedDirs;

        public Form1()
        {
            InitializeComponent();
            LoadTempPath();
        }

        private void InitializeComponent()
        {
            this.Text = "Очистка кэша (временная папка)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(720, 420);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Путь к TEMP
            lblTempPath = new Label();
            lblTempPath.Text = "Временная папка (TEMP):";
            lblTempPath.Location = new Point(10, 10);
            lblTempPath.AutoSize = true;
            this.Controls.Add(lblTempPath);

            txtTempPath = new TextBox();
            txtTempPath.Location = new Point(10, 30);
            txtTempPath.Size = new Size(690, 25);
            txtTempPath.ReadOnly = true;
            txtTempPath.BackColor = Color.White;
            this.Controls.Add(txtTempPath);

            // Кнопки
            btnAnalyze = CreateButton("Посчитать", 10, 70, 150, 30);
            btnAnalyze.Click += BtnAnalyze_Click;

            btnClean = CreateButton("Очистить", 170, 70, 150, 30);
            btnClean.Click += BtnClean_Click;

            // Статистика
            lblFilesCount = new Label();
            lblFilesCount.Text = "Файлов: -";
            lblFilesCount.Location = new Point(10, 115);
            lblFilesCount.AutoSize = true;
            this.Controls.Add(lblFilesCount);

            lblDirsCount = new Label();
            lblDirsCount.Text = "Папок: -";
            lblDirsCount.Location = new Point(10, 140);
            lblDirsCount.AutoSize = true;
            this.Controls.Add(lblDirsCount);

            lblSize = new Label();
            lblSize.Text = "Размер: -";
            lblSize.Location = new Point(10, 165);
            lblSize.AutoSize = true;
            this.Controls.Add(lblSize);

            lblDeletedFiles = new Label();
            lblDeletedFiles.Text = "Удалено файлов: -";
            lblDeletedFiles.Location = new Point(250, 115);
            lblDeletedFiles.AutoSize = true;
            this.Controls.Add(lblDeletedFiles);

            lblDeletedDirs = new Label();
            lblDeletedDirs.Text = "Удалено папок: -";
            lblDeletedDirs.Location = new Point(250, 140);
            lblDeletedDirs.AutoSize = true;
            this.Controls.Add(lblDeletedDirs);

            // Лог
            txtLog = new TextBox();
            txtLog.Location = new Point(10, 200);
            txtLog.Size = new Size(690, 200);
            txtLog.Multiline = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.BackColor = Color.White;
            txtLog.Font = new Font("Consolas", 9F);
            this.Controls.Add(txtLog);
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

        private void LoadTempPath()
        {
            // Безопасный способ получить путь к временной папке пользователя [web:219][web:224][web:228]
            string tempPath = Path.GetTempPath();
            txtTempPath.Text = tempPath;
        }

        private void BtnAnalyze_Click(object sender, EventArgs e)
        {
            string path = txtTempPath.Text;

            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                MessageBox.Show("Временная папка не найдена.");
                return;
            }

            _totalBytes = 0;
            _totalFiles = 0;
            _totalDirs = 0;

            lblFilesCount.Text = "Файлов: -";
            lblDirsCount.Text = "Папок: -";
            lblSize.Text = "Размер: -";

            txtLog.Clear();
            txtLog.AppendText("Анализ временной папки...\r\n");

            try
            {
                AnalyzeDirectory(path);
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"Ошибка анализа: {ex.Message}\r\n");
            }

            lblFilesCount.Text = $"Файлов: {_totalFiles}";
            lblDirsCount.Text = $"Папок: {_totalDirs}";
            lblSize.Text = $"Размер: {FormatSize(_totalBytes)}";
            txtLog.AppendText("Анализ завершён.\r\n");
        }

        private void AnalyzeDirectory(string path)
        {
            // Рекурсивный обход каталога и подсчёт файлов/папок, как в типичных кодах клинеров [web:220][web:227][web:228]
            string[] files = Array.Empty<string>();
            string[] dirs = Array.Empty<string>();

            try
            {
                files = Directory.GetFiles(path);
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"[АНАЛИЗ] Нет доступа к файлам в \"{path}\": {ex.Message}\r\n");
            }

            foreach (var file in files)
            {
                try
                {
                    var info = new FileInfo(file);
                    _totalBytes += info.Length;
                    _totalFiles++;
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"[АНАЛИЗ] Ошибка файла \"{file}\": {ex.Message}\r\n");
                }
            }

            try
            {
                dirs = Directory.GetDirectories(path);
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"[АНАЛИЗ] Нет доступа к папкам в \"{path}\": {ex.Message}\r\n");
            }

            foreach (var dir in dirs)
            {
                _totalDirs++;
                AnalyzeDirectory(dir);
            }
        }

        private void BtnClean_Click(object sender, EventArgs e)
        {
            string path = txtTempPath.Text;

            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                MessageBox.Show("Временная папка не найдена.");
                return;
            }

            var result = MessageBox.Show(
                "Будет выполнена попытка удалить временные файлы текущего пользователя.\n" +
                "Файлы, используемые системой или приложениями, пропускаются.\n\n" +
                "Продолжить?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            _deletedFiles = 0;
            _deletedDirs = 0;

            lblDeletedFiles.Text = "Удалено файлов: -";
            lblDeletedDirs.Text = "Удалено папок: -";

            txtLog.AppendText("\r\nОчистка временной папки...\r\n");

            try
            {
                CleanDirectory(path);
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"Ошибка очистки: {ex.Message}\r\n");
            }

            lblDeletedFiles.Text = $"Удалено файлов: {_deletedFiles}";
            lblDeletedDirs.Text = $"Удалено папок: {_deletedDirs}";
            txtLog.AppendText("Очистка завершена.\r\n");
        }

        private void CleanDirectory(string path)
        {
            // Логика основана на типичных примерах: удаляем файлы, затем папки, с try/catch вокруг каждого вызова [web:220][web:227][web:228]
            string[] files = Array.Empty<string>();
            string[] dirs = Array.Empty<string>();

            try
            {
                files = Directory.GetFiles(path);
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"[ОЧИСТКА] Нет доступа к файлам в \"{path}\": {ex.Message}\r\n");
            }

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                    _deletedFiles++;
                    txtLog.AppendText($"[OK] Файл удалён: {file}\r\n");
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"[SKIP] Не удалось удалить файл \"{file}\": {ex.Message}\r\n");
                }
            }

            try
            {
                dirs = Directory.GetDirectories(path);
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"[ОЧИСТКА] Нет доступа к папкам в \"{path}\": {ex.Message}\r\n");
            }

            foreach (var dir in dirs)
            {
                CleanDirectory(dir); // сначала чистим содержимое

                try
                {
                    Directory.Delete(dir, false);
                    _deletedDirs++;
                    txtLog.AppendText($"[OK] Папка удалена: {dir}\r\n");
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"[SKIP] Не удалось удалить папку \"{dir}\": {ex.Message}\r\n");
                }
            }
        }

        private string FormatSize(long bytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (bytes >= GB)
                return $"{bytes / (double)GB:0.00} ГБ";
            if (bytes >= MB)
                return $"{bytes / (double)MB:0.00} МБ";
            if (bytes >= KB)
                return $"{bytes / (double)KB:0.00} КБ";
            return $"{bytes} байт";
        }
    }
}