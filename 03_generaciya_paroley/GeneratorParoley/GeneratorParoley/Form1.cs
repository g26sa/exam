using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GeneratorParoley
{
    public class Form1 : Form
    {
        private Label lblLength;
        private NumericUpDown nudLength;

        private CheckBox chkLower;
        private CheckBox chkUpper;
        private CheckBox chkDigits;
        private CheckBox chkSpecial;

        private TextBox txtPassword;
        private Button btnGenerate;
        private Button btnCopy;

        private Random _random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Генератор случайных паролей";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(420, 200);
            this.BackColor = Color.FromArgb(240, 240, 240);   // фон
            this.ForeColor = Color.FromArgb(32, 32, 32);      // текст
            this.Font = new Font("Segoe UI", 10F);

            // Длина
            lblLength = new Label();
            lblLength.Text = "Длина:";
            lblLength.Location = new Point(10, 10);
            lblLength.AutoSize = true;
            this.Controls.Add(lblLength);

            nudLength = new NumericUpDown();
            nudLength.Minimum = 4;
            nudLength.Maximum = 64;
            nudLength.Value = 12;
            nudLength.Location = new Point(70, 8);
            nudLength.Size = new Size(60, 20);
            this.Controls.Add(nudLength);

            // Чекбоксы
            chkLower = new CheckBox();
            chkLower.Text = "Строчные (a-z)";
            chkLower.Checked = true;
            chkLower.Location = new Point(10, 40);
            chkLower.AutoSize = true;
            this.Controls.Add(chkLower);

            chkUpper = new CheckBox();
            chkUpper.Text = "Прописные (A-Z)";
            chkUpper.Checked = true;
            chkUpper.Location = new Point(10, 65);
            chkUpper.AutoSize = true;
            this.Controls.Add(chkUpper);

            chkDigits = new CheckBox();
            chkDigits.Text = "Цифры (0-9)";
            chkDigits.Checked = true;
            chkDigits.Location = new Point(200, 40);
            chkDigits.AutoSize = true;
            this.Controls.Add(chkDigits);

            chkSpecial = new CheckBox();
            chkSpecial.Text = "Символы (!@#$%)";
            chkSpecial.Checked = false;
            chkSpecial.Location = new Point(200, 65);
            chkSpecial.AutoSize = true;
            this.Controls.Add(chkSpecial);

            // Поле пароля
            txtPassword = new TextBox();
            txtPassword.Location = new Point(10, 100);
            txtPassword.Size = new Size(390, 25);
            txtPassword.ReadOnly = true;
            txtPassword.Font = new Font("Consolas", 11F);
            this.Controls.Add(txtPassword);

            // Кнопка Generate
            btnGenerate = CreateButton("Генерировать", 10, 140, 120, 30);
            btnGenerate.Click += BtnGenerate_Click;

            // Кнопка Copy
            btnCopy = CreateButton("Копировать", 140, 140, 110, 30);
            btnCopy.Click += BtnCopy_Click;
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

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string digits = "0123456789";
            string special = "!@#$%&*?";

            var chars = new StringBuilder();

            if (chkLower.Checked) chars.Append(lower);
            if (chkUpper.Checked) chars.Append(upper);
            if (chkDigits.Checked) chars.Append(digits);
            if (chkSpecial.Checked) chars.Append(special);

            if (chars.Length == 0)
            {
                MessageBox.Show("Выберите хотя бы один тип символов.");
                return;
            }

            int length = (int)nudLength.Value;
            var result = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int index = _random.Next(chars.Length);
                result.Append(chars[index]);
            }

            txtPassword.Text = result.ToString();
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Text))
            {
                Clipboard.SetText(txtPassword.Text);
                MessageBox.Show("Пароль скопирован в буфер обмена.");
            }
        }
    }
}