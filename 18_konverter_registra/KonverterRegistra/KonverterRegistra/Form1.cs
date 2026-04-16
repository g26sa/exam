using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KonverterRegistra
{
    public class Form1 : Form
    {
        private TextBox txtInput;
        private TextBox txtOutput;

        private Label lblInput;
        private Label lblOutput;

        private Button btnUpper;
        private Button btnLower;
        private Button btnTitle;
        private Button btnInvert;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Конвертер регистра текста";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(700, 420);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Исходный текст
            lblInput = new Label();
            lblInput.Text = "Исходный текст:";
            lblInput.Location = new Point(10, 10);
            lblInput.AutoSize = true;
            this.Controls.Add(lblInput);

            txtInput = new TextBox();
            txtInput.Location = new Point(10, 30);
            txtInput.Size = new Size(660, 140);
            txtInput.Multiline = true;
            txtInput.ScrollBars = ScrollBars.Vertical;
            txtInput.BackColor = Color.White;
            txtInput.ForeColor = Color.FromArgb(32, 32, 32);
            this.Controls.Add(txtInput);

            // Кнопки
            btnUpper = CreateButton("ВСЕ ЗАГЛАВНЫЕ", 10, 180, 160, 30);
            btnUpper.Click += BtnUpper_Click;

            btnLower = CreateButton("все строчные", 180, 180, 160, 30);
            btnLower.Click += BtnLower_Click;

            btnTitle = CreateButton("Каждое Слово С Большой", 350, 180, 200, 30);
            btnTitle.Click += BtnTitle_Click;

            btnInvert = CreateButton("Конвертировать", 560, 180, 160, 30);
            btnInvert.Click += BtnInvert_Click;

            // Результат
            lblOutput = new Label();
            lblOutput.Text = "Результат:";
            lblOutput.Location = new Point(10, 220);
            lblOutput.AutoSize = true;
            this.Controls.Add(lblOutput);

            txtOutput = new TextBox();
            txtOutput.Location = new Point(10, 240);
            txtOutput.Size = new Size(660, 150);
            txtOutput.Multiline = true;
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.BackColor = Color.White;
            txtOutput.ForeColor = Color.FromArgb(32, 32, 32);
            this.Controls.Add(txtOutput);
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

        private string GetInput()
        {
            return txtInput.Text ?? string.Empty;
        }

        private void BtnUpper_Click(object sender, EventArgs e)
        {
            string input = GetInput();
            txtOutput.Text = input.ToUpper(); // стандартный ToUpper [web:191][web:196]
        }

        private void BtnLower_Click(object sender, EventArgs e)
        {
            string input = GetInput();
            txtOutput.Text = input.ToLower();
        }

        private void BtnTitle_Click(object sender, EventArgs e)
        {
            string input = GetInput();
            txtOutput.Text = ToTitleCaseSimple(input);
        }

        private void BtnInvert_Click(object sender, EventArgs e)
        {
            string input = GetInput();
            txtOutput.Text = InvertCase(input);
        }

        // Простой Title Case: первая буква слова – верхний регистр, остальное – нижний [web:197]
        private string ToTitleCaseSimple(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var result = new StringBuilder(text.Length);
            bool newWord = true;

            foreach (char c in text)
            {
                if (char.IsWhiteSpace(c) || char.IsPunctuation(c))
                {
                    result.Append(c);
                    newWord = true;
                }
                else
                {
                    if (newWord)
                    {
                        result.Append(char.ToUpper(c));
                        newWord = false;
                    }
                    else
                    {
                        result.Append(char.ToLower(c));
                    }
                }
            }

            return result.ToString();
        }

        // Инвертирование регистра: нижние -> верхние, верхние -> нижние [web:189][web:195][web:198]
        private string InvertCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var result = new StringBuilder(text.Length);

            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    if (char.IsUpper(c))
                        result.Append(char.ToLower(c));
                    else
                        result.Append(char.ToUpper(c));
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }
}