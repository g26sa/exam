using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PereimenovaniePoShablonu
{
    public class Form1 : Form
    {
        private TextBox txtFolder;
        private Button btnBrowseFolder;

        private Label lblFilter;
        private TextBox txtFilter;

        private Label lblPattern;
        private TextBox txtPattern;

        private Button btnPreview;
        private Button btnRename;

        private ListBox lstPreview;

        private List<(string OldPath, string NewPath)> _previewList = new List<(string, string)>();

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Переименование файлов по шаблону";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(750, 400);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Папка
            var lblFolder = new Label();
            lblFolder.Text = "Папка:";
            lblFolder.Location = new Point(10, 10);
            lblFolder.AutoSize = true;
            this.Controls.Add(lblFolder);

            txtFolder = new TextBox();
            txtFolder.Location = new Point(10, 30);
            txtFolder.Size = new Size(550, 25);
            txtFolder.ReadOnly = true;
            this.Controls.Add(txtFolder);

            btnBrowseFolder = CreateButton("Обзор", 570, 30, 160, 25);
            btnBrowseFolder.Click += BtnBrowseFolder_Click;

            // Фильтр
            lblFilter = new Label();
            lblFilter.Text = "Фильтр файлов (например, *.jpg, *.txt, *.*):";
            lblFilter.Location = new Point(10, 70);
            lblFilter.AutoSize = true;
            this.Controls.Add(lblFilter);

            txtFilter = new TextBox();
            txtFilter.Location = new Point(10, 90);
            txtFilter.Size = new Size(250, 25);
            txtFilter.Text = "*.*";
            this.Controls.Add(txtFilter);

            // Шаблон
            lblPattern = new Label();
            lblPattern.Text = "Шаблон имени (используйте {0} для номера, напр. photo_{0:D3}):";
            lblPattern.Location = new Point(10, 130);
            lblPattern.Size = new Size(720, 40);
            this.Controls.Add(lblPattern);

            txtPattern = new TextBox();
            txtPattern.Location = new Point(10, 170);
            txtPattern.Size = new Size(350, 25);
            txtPattern.Text = "file_{0:D3}";
            this.Controls.Add(txtPattern);

            // Кнопки
            btnPreview = CreateButton("Предпросмотр", 380, 170, 160, 25);
            btnPreview.Click += BtnPreview_Click;

            btnRename = CreateButton("Переименовать", 560, 170, 170, 25);
            btnRename.Click += BtnRename_Click;

            // Список предпросмотра
            lstPreview = new ListBox();
            lstPreview.Location = new Point(10, 210);
            lstPreview.Size = new Size(720, 170);
            lstPreview.BackColor = Color.White;
            lstPreview.ForeColor = Color.FromArgb(32, 32, 32);
            this.Controls.Add(lstPreview);
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
                    lstPreview.Items.Clear();
                    _previewList.Clear();
                }
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            lstPreview.Items.Clear();
            _previewList.Clear();

            if (string.IsNullOrWhiteSpace(txtFolder.Text) || !Directory.Exists(txtFolder.Text))
            {
                MessageBox.Show("Сначала выберите папку.");
                return;
            }

            string filter = txtFilter.Text.Trim();
            if (string.IsNullOrWhiteSpace(filter))
                filter = "*.*";

            string pattern = txtPattern.Text.Trim();
            if (string.IsNullOrWhiteSpace(pattern) || !pattern.Contains("{0"))
            {
                MessageBox.Show("Укажите шаблон имени, содержащий {0} (например, file_{0:D3}).");
                return;
            }

            try
            {
                string[] files = Directory.GetFiles(txtFolder.Text, filter);
                Array.Sort(files, StringComparer.OrdinalIgnoreCase);

                int index = 1;
                foreach (var file in files)
                {
                    string dir = Path.GetDirectoryName(file);
                    string ext = Path.GetExtension(file);

                    string newNameOnly;
                    try
                    {
                        // string.Format с {0}, {0:D3} и т.п. [web:185][web:188]
                        newNameOnly = string.Format(pattern, index);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка в шаблоне имени: " + ex.Message);
                        _previewList.Clear();
                        lstPreview.Items.Clear();
                        return;
                    }

                    string newPath = Path.Combine(dir, newNameOnly + ext);

                    _previewList.Add((file, newPath));
                    lstPreview.Items.Add($"{Path.GetFileName(file)}  ->  {Path.GetFileName(newPath)}");

                    index++;
                }

                if (_previewList.Count == 0)
                {
                    lstPreview.Items.Add("Нет файлов, подходящих под фильтр.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при формировании списка файлов: " + ex.Message);
            }
        }

        private void BtnRename_Click(object sender, EventArgs e)
        {
            if (_previewList.Count == 0)
            {
                MessageBox.Show("Нет файлов для переименования. Сначала сделайте предпросмотр.");
                return;
            }

            var result = MessageBox.Show(
                $"Переименовать {_previewList.Count} файлов?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            int success = 0;
            int errors = 0;

            foreach (var item in _previewList)
            {
                try
                {
                    if (File.Exists(item.NewPath))
                    {
                        errors++;
                        continue;
                    }

                    File.Move(item.OldPath, item.NewPath); // стандартный способ переименования [web:180][web:141]
                    success++;
                }
                catch
                {
                    errors++;
                }
            }

            MessageBox.Show($"Готово. Успешно: {success}, ошибок: {errors}.");

            // Обновить предпросмотр
            BtnPreview_Click(sender, e);
        }
    }
}