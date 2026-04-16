using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TekstoviyRedaktor
{
    public class Form1 : Form
    {
        private RichTextBox txtEditor;
        private Button btnOpen;
        private Button btnSave;
        private Button btnClear;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Текстовый редактор";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(600, 400);
            this.BackColor = Color.FromArgb(240, 240, 240);   // фон
            this.ForeColor = Color.FromArgb(32, 32, 32);      // текст
            this.Font = new Font("Segoe UI", 10F);

            // Поле редактора
            txtEditor = new RichTextBox();
            txtEditor.Location = new Point(10, 50);
            txtEditor.Size = new Size(580, 340);
            txtEditor.BackColor = Color.White;
            txtEditor.ForeColor = Color.FromArgb(32, 32, 32);
            txtEditor.Font = new Font("Consolas", 11F); // моноширинный шрифт — удобно для кода
            this.Controls.Add(txtEditor);

            // Общий стиль кнопок
            int btnWidth = 100;
            int btnHeight = 30;
            int startX = 10;
            int startY = 10;
            int margin = 10;

            // Кнопка Открыть
            btnOpen = CreateButton("Открыть", startX, startY, btnWidth, btnHeight);
            btnOpen.Click += BtnOpen_Click;

            // Кнопка Сохранить
            btnSave = CreateButton("Сохранить", startX + (btnWidth + margin) * 1, startY, btnWidth, btnHeight);
            btnSave.Click += BtnSave_Click;

            // Кнопка Очистить
            btnClear = CreateButton("Очистить", startX + (btnWidth + margin) * 2, startY, btnWidth, btnHeight);
            btnClear.Click += BtnClear_Click;
        }

        private Button CreateButton(string text, int x, int y, int width, int height)
        {
            var button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(width, height);
            button.BackColor = Color.FromArgb(47, 79, 111);   // тот же тёмно-синий
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
                dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string text = File.ReadAllText(dialog.FileName);
                        txtEditor.Text = text;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при открытии файла: " + ex.Message);
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(dialog.FileName, txtEditor.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении файла: " + ex.Message);
                    }
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Очистить текст?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                txtEditor.Clear();
            }
        }
    }
}