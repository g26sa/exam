using System;
using System.Drawing;
using System.Windows.Forms;

namespace KonvertaciyaEdinic
{
    public class Form1 : Form
    {
        private TextBox txtInput;
        private Label lblInput;

        private ComboBox cmbCategory;
        private ComboBox cmbFrom;
        private ComboBox cmbTo;

        private Label lblCategory;
        private Label lblFrom;
        private Label lblTo;

        private Button btnConvert;
        private Label lblResult;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Конвертер единиц измерения";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(520, 230);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Ввод значения
            lblInput = new Label();
            lblInput.Text = "Значение:";
            lblInput.Location = new Point(10, 10);
            lblInput.AutoSize = true;
            this.Controls.Add(lblInput);

            txtInput = new TextBox();
            txtInput.Location = new Point(90, 8);
            txtInput.Size = new Size(150, 25);
            this.Controls.Add(txtInput);

            // Категория
            lblCategory = new Label();
            lblCategory.Text = "Величина:";
            lblCategory.Location = new Point(260, 10);
            lblCategory.AutoSize = true;
            this.Controls.Add(lblCategory);

            cmbCategory = new ComboBox();
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Location = new Point(340, 8);
            cmbCategory.Size = new Size(160, 25);
            cmbCategory.Items.AddRange(new object[]
            {
                "Длина",
                "Масса",
                "Температура"
            });
            cmbCategory.SelectedIndexChanged += CmbCategory_SelectedIndexChanged;
            this.Controls.Add(cmbCategory);

            // Из / В
            lblFrom = new Label();
            lblFrom.Text = "Из:";
            lblFrom.Location = new Point(10, 50);
            lblFrom.AutoSize = true;
            this.Controls.Add(lblFrom);

            cmbFrom = new ComboBox();
            cmbFrom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFrom.Location = new Point(90, 48);
            cmbFrom.Size = new Size(160, 25);
            this.Controls.Add(cmbFrom);

            lblTo = new Label();
            lblTo.Text = "В:";
            lblTo.Location = new Point(260, 50);
            lblTo.AutoSize = true;
            this.Controls.Add(lblTo);

            cmbTo = new ComboBox();
            cmbTo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTo.Location = new Point(340, 48);
            cmbTo.Size = new Size(160, 25);
            this.Controls.Add(cmbTo);

            // Кнопка Конвертировать
            btnConvert = CreateButton("Конвертировать", 10, 90, 490, 30);
            btnConvert.Click += BtnConvert_Click;

            // Результат
            lblResult = new Label();
            lblResult.Text = "Результат: -";
            lblResult.Location = new Point(10, 140);
            lblResult.AutoSize = true;
            this.Controls.Add(lblResult);

            // ВАЖНО: только сейчас устанавливаем SelectedIndex и вызываем UpdateUnitCombos
            cmbCategory.SelectedIndex = 0;
            UpdateUnitCombos();
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

        private void CmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUnitCombos();
        }

        private void UpdateUnitCombos()
        {
            // Защита от null при раннем вызове
            if (cmbCategory == null || cmbFrom == null || cmbTo == null)
                return;

            cmbFrom.Items.Clear();
            cmbTo.Items.Clear();

            string category;

            if (cmbCategory.SelectedItem == null)
                category = "Длина";
            else
                category = cmbCategory.SelectedItem.ToString();

            if (category == "Длина")
            {
                string[] units = { "метр", "километр", "сантиметр" };
                cmbFrom.Items.AddRange(units);
                cmbTo.Items.AddRange(units);
            }
            else if (category == "Масса")
            {
                string[] units = { "килограмм", "грамм", "тонна" };
                cmbFrom.Items.AddRange(units);
                cmbTo.Items.AddRange(units);
            }
            else if (category == "Температура")
            {
                string[] units = { "Цельсий", "Фаренгейт", "Кельвин" };
                cmbFrom.Items.AddRange(units);
                cmbTo.Items.AddRange(units);
            }

            if (cmbFrom.Items.Count > 0)
                cmbFrom.SelectedIndex = 0;

            if (cmbTo.Items.Count > 1)
                cmbTo.SelectedIndex = 1;
            else if (cmbTo.Items.Count > 0)
                cmbTo.SelectedIndex = 0;
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            lblResult.Text = "Результат: -";

            if (!double.TryParse(
                    txtInput.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double value))
            {
                MessageBox.Show("Введите корректное число.");
                return;
            }

            string category = cmbCategory.SelectedItem?.ToString() ?? "Длина";
            string fromUnit = cmbFrom.SelectedItem?.ToString();
            string toUnit = cmbTo.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(fromUnit) || string.IsNullOrEmpty(toUnit))
            {
                MessageBox.Show("Выберите единицы измерения.");
                return;
            }

            double result;

            try
            {
                if (category == "Длина")
                {
                    result = ConvertLength(value, fromUnit, toUnit);
                }
                else if (category == "Масса")
                {
                    result = ConvertWeight(value, fromUnit, toUnit);
                }
                else // Температура
                {
                    result = ConvertTemperature(value, fromUnit, toUnit);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка конвертации: " + ex.Message);
                return;
            }

            lblResult.Text = $"Результат: {result:F4} {toUnit}";
        }

        // Длина: приводим к метрам
        private double ConvertLength(double value, string from, string to)
        {
            double inMeters = value;

            switch (from)
            {
                case "метр":
                    inMeters = value;
                    break;
                case "километр":
                    inMeters = value * 1000.0;
                    break;
                case "сантиметр":
                    inMeters = value / 100.0;
                    break;
            }

            switch (to)
            {
                case "метр":
                    return inMeters;
                case "километр":
                    return inMeters / 1000.0;
                case "сантиметр":
                    return inMeters * 100.0;
            }

            return value;
        }

        // Масса: приводим к килограммам
        private double ConvertWeight(double value, string from, string to)
        {
            double inKg = value;

            switch (from)
            {
                case "килограмм":
                    inKg = value;
                    break;
                case "грамм":
                    inKg = value / 1000.0;
                    break;
                case "тонна":
                    inKg = value * 1000.0;
                    break;
            }

            switch (to)
            {
                case "килограмм":
                    return inKg;
                case "грамм":
                    return inKg * 1000.0;
                case "тонна":
                    return inKg / 1000.0;
            }

            return value;
        }

        // Температура: через Цельсий [web:104][web:103]
        private double ConvertTemperature(double value, string from, string to)
        {
            if (from == to)
                return value;

            double celsius;

            // В Цельсий
            if (from == "Цельсий")
            {
                celsius = value;
            }
            else if (from == "Фаренгейт")
            {
                celsius = (value - 32.0) * 5.0 / 9.0;
            }
            else // Кельвин
            {
                celsius = value - 273.15;
            }

            // Из Цельсия в целевую единицу
            if (to == "Цельсий")
            {
                return celsius;
            }
            else if (to == "Фаренгейт")
            {
                return celsius * 9.0 / 5.0 + 32.0;
            }
            else // Кельвин
            {
                return celsius + 273.15;
            }
        }
    }
}