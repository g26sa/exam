using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Bloknot
{
    public class Form1 : Form
    {
        private RichTextBox richText;
        private Button btnNew;
        private Button btnOpen;
        private Button btnSave;
        private Button btnFont;

        private Button btnBold;
        private Button btnItalic;
        private Button btnUnderline;

        private string _currentFilePath = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Блокнот";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(800, 500);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            int margin = 10;

            // Панель кнопок (верхняя)
            int buttonHeight = 30;
            int buttonWidth = 89;
            int yTop = 10;

            btnNew = CreateButton("Создать", 10, yTop, buttonWidth, buttonHeight);
            btnNew.Click += BtnNew_Click;

            btnOpen = CreateButton("Открыть", 10 + (buttonWidth + margin) * 1, yTop, buttonWidth, buttonHeight);
            btnOpen.Click += BtnOpen_Click;

            btnSave = CreateButton("Сохранить", 10 + (buttonWidth + margin) * 2, yTop, buttonWidth, buttonHeight);
            btnSave.Click += BtnSave_Click;

            btnFont = CreateButton("Шрифт...", 10 + (buttonWidth + margin) * 3, yTop, buttonWidth, buttonHeight);
            btnFont.Click += BtnFont_Click;

            btnBold = CreateButton("B", 10 + (buttonWidth + margin) * 4, yTop, 40, buttonHeight);
            btnBold.Font = new Font(this.Font, FontStyle.Bold);
            btnBold.Click += BtnBold_Click;

            btnItalic = CreateButton("I", 10 + (buttonWidth + margin) * 4 + 40 + margin, yTop, 40, buttonHeight);
            btnItalic.Font = new Font(this.Font, FontStyle.Italic);
            btnItalic.Click += BtnItalic_Click;

            btnUnderline = CreateButton("U", 10 + (buttonWidth + margin) * 4 + (40 + margin) * 2, yTop, 40, buttonHeight);
            btnUnderline.Font = new Font(this.Font, FontStyle.Underline);
            btnUnderline.Click += BtnUnderline_Click;

            // Поле текста
            richText = new RichTextBox();
            richText.Location = new Point(10, yTop + buttonHeight + margin);
            richText.Size = new Size(780, 500 - (yTop + buttonHeight + margin * 2));
            richText.BackColor = Color.White;
            richText.ForeColor = Color.FromArgb(32, 32, 32);
            richText.Font = new Font("Consolas", 11F);
            this.Controls.Add(richText);
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

        // New (очистить редактор и сбросить путь)
        private void BtnNew_Click(object sender, EventArgs e)
        {
            if (richText.Modified)
            {
                var result = MessageBox.Show(
                    "Сохранить изменения перед созданием нового файла?",
                    "Блокнот",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                    return;

                if (result == DialogResult.Yes)
                {
                    if (!SaveCurrentFile())
                        return;
                }
            }

            richText.Clear();
            _currentFilePath = null;
            this.Text = "Bloknot - New file";
            richText.Modified = false;
        }

        // Open
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string text = File.ReadAllText(dialog.FileName);
                        richText.Text = text;
                        _currentFilePath = dialog.FileName;
                        this.Text = "Bloknot - " + Path.GetFileName(_currentFilePath);
                        richText.Modified = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при открытии файла: " + ex.Message);
                    }
                }
            }
        }

        // Save
        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentFile();
        }

        private bool SaveCurrentFile()
        {
            try
            {
                if (string.IsNullOrEmpty(_currentFilePath))
                {
                    using (var dialog = new SaveFileDialog())
                    {
                        dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            _currentFilePath = dialog.FileName;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                File.WriteAllText(_currentFilePath, richText.Text);
                this.Text = "Bloknot - " + Path.GetFileName(_currentFilePath);
                richText.Modified = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении файла: " + ex.Message);
                return false;
            }
        }

        // Font...
        private void BtnFont_Click(object sender, EventArgs e)
        {
            using (var dialog = new FontDialog())
            {
                dialog.Font = richText.SelectionFont ?? richText.Font;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (richText.SelectionLength > 0)
                    {
                        richText.SelectionFont = dialog.Font;
                    }
                    else
                    {
                        richText.Font = dialog.Font;
                    }
                }
            }
        }

        // Bold / Italic / Underline — переключают стиль выделенного текста
        private void BtnBold_Click(object sender, EventArgs e)
        {
            ToggleSelectionStyle(FontStyle.Bold);
        }

        private void BtnItalic_Click(object sender, EventArgs e)
        {
            ToggleSelectionStyle(FontStyle.Italic);
        }

        private void BtnUnderline_Click(object sender, EventArgs e)
        {
            ToggleSelectionStyle(FontStyle.Underline);
        }

        private void ToggleSelectionStyle(FontStyle style)
        {
            if (richText.SelectionFont == null)
                return;

            Font currentFont = richText.SelectionFont;
            FontStyle newStyle;

            // Переключаем конкретный стиль (оставляя другие), как советуют для RichTextBox [web:71][web:85]
            if (currentFont.Style.HasFlag(style))
            {
                newStyle = currentFont.Style & ~style;
            }
            else
            {
                newStyle = currentFont.Style | style;
            }

            richText.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newStyle);
        }
    }
}