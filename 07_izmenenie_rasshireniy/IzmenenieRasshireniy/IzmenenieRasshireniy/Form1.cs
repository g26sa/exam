using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace IzmenenieRasshireniy
{
    public class Form1 : Form
    {
        private TextBox txtFolder;
        private Button btnBrowseFolder;

        private TextBox txtOldExt;
        private TextBox txtNewExt;
        private Label lblOldExt;
        private Label lblNewExt;

        private Button btnPreview;
        private Button btnRename;

        private ListBox lstFiles;

        private List<(string OldPath, string NewPath)> _previewList = new List<(string, string)>();

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Расширение файлов";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(700, 400);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            int margin = 10;

            // Папка
            var lblFolder = new Label();
            lblFolder.Text = "Папка:";
            lblFolder.Location = new Point(10, 10);
            lblFolder.AutoSize = true;
            this.Controls.Add(lblFolder);

            txtFolder = new TextBox();
            txtFolder.Location = new Point(10, 30);
            txtFolder.Size = new Size(520, 25);
            txtFolder.ReadOnly = true;
            this.Controls.Add(txtFolder);

            btnBrowseFolder = CreateButton("Найти", 540, 30, 140, 25);
            btnBrowseFolder.Click += BtnBrowseFolder_Click;

            // Старое и новое расширение
            lblOldExt = new Label();
            lblOldExt.Text = "Старое:";
            lblOldExt.Location = new Point(10, 70);
            lblOldExt.AutoSize = true;
            this.Controls.Add(lblOldExt);

            txtOldExt = new TextBox();
            txtOldExt.Location = new Point(70, 68);
            txtOldExt.Size = new Size(80, 25);
            txtOldExt.Text = "txt"; // пример по умолчанию
            this.Controls.Add(txtOldExt);

            lblNewExt = new Label();
            lblNewExt.Text = "Новое:";
            lblNewExt.Location = new Point(170, 70);
            lblNewExt.AutoSize = true;
            this.Controls.Add(lblNewExt);

            txtNewExt = new TextBox();
            txtNewExt.Location = new Point(240, 68);
            txtNewExt.Size = new Size(80, 25);
            txtNewExt.Text = "log"; // пример
            this.Controls.Add(txtNewExt);

            // Кнопки Preview и Rename
            btnPreview = CreateButton("Превью", 340, 68, 100, 25);
            btnPreview.Click += BtnPreview_Click;

            btnRename = CreateButton("Переименовать", 450, 68, 120, 25);
            btnRename.Click += BtnRename_Click;

            // Список файлов
            lstFiles = new ListBox();
            lstFiles.Location = new Point(10, 110);
            lstFiles.Size = new Size(670, 270);
            this.Controls.Add(lstFiles);
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

        private void BtnBrowseFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку с файлами";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtFolder.Text = dialog.SelectedPath;
                    lstFiles.Items.Clear();
                    _previewList.Clear();

                    try
                    {
                        var files = Directory.GetFiles(dialog.SelectedPath);
                        foreach (var file in files)
                        {
                            lstFiles.Items.Add(Path.GetFileName(file));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при чтении папки: " + ex.Message);
                    }
                }
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            lstFiles.Items.Clear();
            _previewList.Clear();

            if (string.IsNullOrWhiteSpace(txtFolder.Text) || !Directory.Exists(txtFolder.Text))
            {
                MessageBox.Show("Сначала выберите папку.");
                return;
            }

            string oldExt = NormalizeExtension(txtOldExt.Text);
            string newExt = NormalizeExtension(txtNewExt.Text);

            if (string.IsNullOrEmpty(oldExt) || string.IsNullOrEmpty(newExt))
            {
                MessageBox.Show("Укажите старое и новое расширение (например, txt и log или .txt и .log).");
                return;
            }

            try
            {
                var files = Directory.GetFiles(txtFolder.Text);

                foreach (var file in files)
                {
                    if (string.Equals(Path.GetExtension(file), oldExt, StringComparison.OrdinalIgnoreCase))
                    {
                        string newPath = Path.ChangeExtension(file, newExt);
                        _previewList.Add((file, newPath));
                        lstFiles.Items.Add($"{Path.GetFileName(file)}  ->  {Path.GetFileName(newPath)}");
                    }
                }

                if (_previewList.Count == 0)
                {
                    lstFiles.Items.Add("Нет файлов с указанным исходным расширением.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при формировании предварительного просмотра: " + ex.Message);
            }
        }

        private void BtnRename_Click(object sender, EventArgs e)
        {
            if (_previewList.Count == 0)
            {
                MessageBox.Show("Нет файлов для переименования. Сначала сделайте Preview.");
                return;
            }

            var result = MessageBox.Show(
                $"Переименовать {_previewList.Count} файлов?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            int successCount = 0;
            int errorCount = 0;

            foreach (var item in _previewList)
            {
                try
                {
                    // Если файл с таким именем уже существует, пропускаем
                    if (File.Exists(item.NewPath))
                    {
                        errorCount++;
                        continue;
                    }

                    File.Move(item.OldPath, item.NewPath);
                    successCount++;
                }
                catch
                {
                    errorCount++;
                }
            }

            MessageBox.Show($"Готово. Успешно: {successCount}, ошибок: {errorCount}.");

            // Обновляем список
            BtnPreview_Click(sender, e);
        }

        private string NormalizeExtension(string ext)
        {
            if (string.IsNullOrWhiteSpace(ext))
                return null;

            ext = ext.Trim();

            // Если пользователь ввёл без точки – добавим её
            if (!ext.StartsWith("."))
                ext = "." + ext;

            return ext;
        }
    }
}