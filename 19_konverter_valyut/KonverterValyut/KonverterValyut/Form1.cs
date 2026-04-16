using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace KonverterValyut
{
    public class Form1 : Form
    {
        private Label lblAmount;
        private TextBox txtAmount;

        private Label lblFrom;
        private Label lblTo;
        private ComboBox cmbFrom;
        private ComboBox cmbTo;

        private Button btnConvert;
        private Label lblResult;

        // Жёстко зашитые курсы относительно RUB (1 единица валюты = X RUB)
        private readonly Dictionary<string, decimal> _ratesToRub = new Dictionary<string, decimal>
        {
            { "RUB", 1m },
            { "USD", 90m },   // 1 USD = 90 RUB (пример)
            { "EUR", 100m }   // 1 EUR = 100 RUB (пример)
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Конвертер валют";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(420, 220);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Сумма
            lblAmount = new Label();
            lblAmount.Text = "Сумма:";
            lblAmount.Location = new Point(10, 10);
            lblAmount.AutoSize = true;
            this.Controls.Add(lblAmount);

            txtAmount = new TextBox();
            txtAmount.Location = new Point(70, 8);
            txtAmount.Size = new Size(150, 25);
            this.Controls.Add(txtAmount);

            // Из валюты
            lblFrom = new Label();
            lblFrom.Text = "Из валюты:";
            lblFrom.Location = new Point(10, 50);
            lblFrom.AutoSize = true;
            this.Controls.Add(lblFrom);

            cmbFrom = new ComboBox();
            cmbFrom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFrom.Location = new Point(100, 48);
            cmbFrom.Size = new Size(100, 25);
            this.Controls.Add(cmbFrom);

            // В валюту
            lblTo = new Label();
            lblTo.Text = "В валюту:";
            lblTo.Location = new Point(220, 50);
            lblTo.AutoSize = true;
            this.Controls.Add(lblTo);

            cmbTo = new ComboBox();
            cmbTo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTo.Location = new Point(300, 48);
            cmbTo.Size = new Size(100, 25);
            this.Controls.Add(cmbTo);

            // Кнопка "Конвертировать"
            btnConvert = CreateButton("Конвертировать", 10, 90, 390, 30);
            btnConvert.Click += BtnConvert_Click;

            // Результат
            lblResult = new Label();
            lblResult.Text = "Результат: -";
            lblResult.Location = new Point(10, 140);
            lblResult.AutoSize = true;
            this.Controls.Add(lblResult);

            // Заполняем валюты
            string[] currencies = { "RUB", "USD", "EUR" };
            cmbFrom.Items.AddRange(currencies);
            cmbTo.Items.AddRange(currencies);
            cmbFrom.SelectedItem = "RUB";
            cmbTo.SelectedItem = "USD";
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

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            lblResult.Text = "Результат: -";

            if (cmbFrom.SelectedItem == null || cmbTo.SelectedItem == null)
            {
                MessageBox.Show("Выберите валюты \"из\" и \"в\".");
                return;
            }

            string from = cmbFrom.SelectedItem.ToString();
            string to = cmbTo.SelectedItem.ToString();

            if (!_ratesToRub.ContainsKey(from) || !_ratesToRub.ContainsKey(to))
            {
                MessageBox.Show("Неизвестная валюта.");
                return;
            }

            if (!decimal.TryParse(
                    txtAmount.Text.Replace(",", "."),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal amount))
            {
                MessageBox.Show("Введите корректную сумму.");
                return;
            }

            if (amount < 0)
            {
                MessageBox.Show("Сумма не может быть отрицательной.");
                return;
            }

            // Приводим к рублям, затем к целевой валюте
            decimal inRub = amount * _ratesToRub[from];
            decimal result = inRub / _ratesToRub[to];

            lblResult.Text = $"Результат: {result:F2} {to}";
        }
    }
}