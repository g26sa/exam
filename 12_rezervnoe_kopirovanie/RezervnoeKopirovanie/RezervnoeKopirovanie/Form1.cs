using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RezervnoeKopirovanie
{
    public class Form1 : Form
    {
        private TextBox txtSource;
        private TextBox txtDestination;
        private Button btnBrowseSource;
        private Button btnBrowseDestination;
        private CheckBox chkRecursive;
        private Button btnCopy;
        private TextBox txtLog;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Резервное копирование папки";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(700, 400);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Источник
            var lblSource = new Label();
            lblSource.Text = "Папка-источник:";
            lblSource.Location = new Point(10, 10);
            lblSource.AutoSize = true;
            this.Controls.Add(lblSource);

            txtSource = new TextBox();
            txtSource.Location = new Point(10, 30);
            txtSource.Size = new Size(520, 25);
            txtSource.ReadOnly = true;
            this.Controls.Add(txtSource);

            btnBrowseSource = CreateButton("Обзор", 540, 30, 140, 25);
            btnBrowseSource.Click += BtnBrowseSource_Click;

            // Назначение
            var lblDestination = new Label();
            lblDestination.Text = "Папка-назначение:";
            lblDestination.Location = new Point(10, 70);
            lblDestination.AutoSize = true;
            this.Controls.Add(lblDestination);

            txtDestination = new TextBox();
            txtDestination.Location = new Point(10, 90);
            txtDestination.Size = new Size(520, 25);
            txtDestination.ReadOnly = true;
            this.Controls.Add(txtDestination);

            btnBrowseDestination = CreateButton("Обзор", 540, 90, 140, 25);
            btnBrowseDestination.Click += BtnBrowseDestination_Click;

            // Чекбокс рекурсивного копирования
            chkRecursive = new CheckBox();
            chkRecursive.Text = "Копировать подпапки";
            chkRecursive.Location = new Point(10, 130);
            chkRecursive.AutoSize = true;
            chkRecursive.Checked = true;
            this.Controls.Add(chkRecursive);

            // Кнопка "Скопировать"
            btnCopy = CreateButton("Скопировать", 10, 160, 670, 30);
            btnCopy.Click += BtnCopy_Click;

            // Лог
            txtLog = new TextBox();
            txtLog.Location = new Point(10, 200);
            txtLog.Size = new Size(670, 180);
            txtLog.Multiline = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.ReadOnly = true;
            txtLog.BackColor = Color.White;
            txtLog.ForeColor = Color.FromArgb(32, 32, 32);
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

        private void BtnBrowseSource_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку-источник";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtSource.Text = dialog.SelectedPath;
                }
            }
        }

        private void BtnBrowseDestination_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку-назначение";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDestination.Text = dialog.SelectedPath;
                }
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            txtLog.Clear();

            string sourceDir = txtSource.Text;
            string destDir = txtDestination.Text;

            if (string.IsNullOrWhiteSpace(sourceDir) || !Directory.Exists(sourceDir))
            {
                MessageBox.Show("Укажите существующую папку-источник.");
                return;
            }

            if (string.IsNullOrWhiteSpace(destDir))
            {
                MessageBox.Show("Укажите папку-назначение.");
                return;
            }

            bool recursive = chkRecursive.Checked;

            try
            {
                string targetRoot = Path.Combine(destDir, Path.GetFileName(sourceDir));
                CopyDirectory(sourceDir, targetRoot, recursive);
                AppendLog("Резервное копирование завершено.");
            }
            catch (Exception ex)
            {
                AppendLog("Ошибка: " + ex.Message);
            }
        }

        // Функция копирования каталога (по образцу из документации .NET) [web:140][web:144][web:139]
        private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Исходная папка не найдена: {dir.FullName}");

            // Получаем подкаталоги
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Создаём целевую папку
            Directory.CreateDirectory(destinationDir);
            AppendLog("Создана папка: " + destinationDir);

            // Копируем файлы
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true); // true — перезаписывать
                AppendLog("Скопирован файл: " + targetFilePath);
            }

            // Если нужно копировать подпапки — рекурсия
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        private void AppendLog(string text)
        {
            txtLog.AppendText(text + Environment.NewLine);
        }
    }
}