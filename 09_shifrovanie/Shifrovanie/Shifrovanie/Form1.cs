using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shifrovanie
{
    public class Form1 : Form
    {
        private TextBox txtInput;
        private TextBox txtKey;
        private TextBox txtOutput;

        private Label lblInput;
        private Label lblKey;
        private Label lblOutput;

        private Button btnEncrypt;
        private Button btnDecrypt;
        private Button btnClear;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Шифрование / дешифрование";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(700, 400);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            int margin = 10;

            // Входной текст
            lblInput = new Label();
            lblInput.Text = "Исходный текст:";
            lblInput.Location = new Point(10, 10);
            lblInput.AutoSize = true;
            this.Controls.Add(lblInput);

            txtInput = new TextBox();
            txtInput.Location = new Point(10, 30);
            txtInput.Size = new Size(660, 100);
            txtInput.Multiline = true;
            txtInput.ScrollBars = ScrollBars.Vertical;
            txtInput.BackColor = Color.White;
            txtInput.ForeColor = Color.FromArgb(32, 32, 32);
            this.Controls.Add(txtInput);

            // Ключ
            lblKey = new Label();
            lblKey.Text = "Ключ:";
            lblKey.Location = new Point(10, 140);
            lblKey.AutoSize = true;
            this.Controls.Add(lblKey);

            txtKey = new TextBox();
            txtKey.Location = new Point(60, 138);
            txtKey.Size = new Size(200, 25);
            txtKey.Text = "secret"; // пример
            this.Controls.Add(txtKey);

            // Кнопки
            btnEncrypt = CreateButton("Зашифровать", 280, 135, 130, 30);
            btnEncrypt.Click += BtnEncrypt_Click;

            btnDecrypt = CreateButton("Расшифровать", 420, 135, 130, 30);
            btnDecrypt.Click += BtnDecrypt_Click;

            btnClear = CreateButton("Очистить", 560, 135, 110, 30);
            btnClear.Click += BtnClear_Click;

            // Выходной текст
            lblOutput = new Label();
            lblOutput.Text = "Результат:";
            lblOutput.Location = new Point(10, 180);
            lblOutput.AutoSize = true;
            this.Controls.Add(lblOutput);

            txtOutput = new TextBox();
            txtOutput.Location = new Point(10, 200);
            txtOutput.Size = new Size(660, 160);
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

        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                MessageBox.Show("Введите ключ шифрования.");
                return;
            }

            string input = txtInput.Text;
            string key = txtKey.Text;

            string encrypted = XorEncryptDecrypt(input, key);

            txtOutput.Text = encrypted;
        }

        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                MessageBox.Show("Введите ключ шифрования.");
                return;
            }

            string input = txtOutput.Text;
            string key = txtKey.Text;

            string decrypted = XorEncryptDecrypt(input, key);

            txtInput.Text = decrypted;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtInput.Clear();
            txtOutput.Clear();
            // ключ оставляем, чтобы не вводить каждый раз
        }

        // Простой XOR-алгоритм: один и тот же метод шифрует и дешифрует [web:109][web:113][web:115][web:118]
        private string XorEncryptDecrypt(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var result = new StringBuilder(text.Length);
            int keyLength = key.Length;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                char k = key[i % keyLength];
                char encryptedChar = (char)(c ^ k);
                result.Append(encryptedChar);
            }

            return result.ToString();
        }
    }
}