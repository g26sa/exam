using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GeneratorChisel
{
    public class Form1 : Form
    {
        private NumericUpDown numFrom;
        private NumericUpDown numTo;
        private NumericUpDown numCount;

        private Label lblFrom;
        private Label lblTo;
        private Label lblCount;

        private CheckBox chkUnique;
        private Button btnGenerate;
        private ListBox lstResults;

        private Random _random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Генератор случайных чисел";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(420, 320);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Диапазон "От"
            lblFrom = new Label();
            lblFrom.Text = "От:";
            lblFrom.Location = new Point(10, 10);
            lblFrom.AutoSize = true;
            this.Controls.Add(lblFrom);

            numFrom = new NumericUpDown();
            numFrom.Location = new Point(40, 8);
            numFrom.Minimum = -1000000;
            numFrom.Maximum = 1000000;
            numFrom.Value = 1;
            numFrom.Width = 80;
            this.Controls.Add(numFrom);

            // Диапазон "До"
            lblTo = new Label();
            lblTo.Text = "До:";
            lblTo.Location = new Point(140, 10);
            lblTo.AutoSize = true;
            this.Controls.Add(lblTo);

            numTo = new NumericUpDown();
            numTo.Location = new Point(170, 8);
            numTo.Minimum = -1000000;
            numTo.Maximum = 1000000;
            numTo.Value = 10;
            numTo.Width = 80;
            this.Controls.Add(numTo);

            // Кол-во
            lblCount = new Label();
            lblCount.Text = "Кол-во:";
            lblCount.Location = new Point(270, 10);
            lblCount.AutoSize = true;
            this.Controls.Add(lblCount);

            numCount = new NumericUpDown();
            numCount.Location = new Point(330, 8);
            numCount.Minimum = 1;
            numCount.Maximum = 1000;
            numCount.Value = 5;
            numCount.Width = 60;
            this.Controls.Add(numCount);

            // Чекбокс уникальности
            chkUnique = new CheckBox();
            chkUnique.Text = "Только уникальные";
            chkUnique.Location = new Point(10, 45);
            chkUnique.AutoSize = true;
            this.Controls.Add(chkUnique);

            // Кнопка "Сгенерировать"
            btnGenerate = CreateButton("Сгенерировать", 10, 75, 380, 30);
            btnGenerate.Click += BtnGenerate_Click;

            // Результаты
            lstResults = new ListBox();
            lstResults.Location = new Point(10, 115);
            lstResults.Size = new Size(380, 180);
            lstResults.BackColor = Color.White;
            lstResults.ForeColor = Color.FromArgb(32, 32, 32);
            this.Controls.Add(lstResults);
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

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            lstResults.Items.Clear();

            int from = (int)numFrom.Value;
            int to = (int)numTo.Value;
            int count = (int)numCount.Value;

            if (from >= to)
            {
                MessageBox.Show("Значение \"От\" должно быть меньше \"До\".");
                return;
            }

            int range = to - from + 1;

            if (chkUnique.Checked && count > range)
            {
                MessageBox.Show("Невозможно сгенерировать столько уникальных чисел в указанном диапазоне.");
                return;
            }

            if (chkUnique.Checked)
            {
                GenerateUniqueNumbers(from, to, count);
            }
            else
            {
                GenerateNumbers(from, to, count);
            }
        }

        private void GenerateNumbers(int from, int to, int count)
        {
            // Random.Next(min, maxExclusive), поэтому maxExclusive = to + 1 [web:171][web:173]
            for (int i = 0; i < count; i++)
            {
                int value = _random.Next(from, to + 1);
                lstResults.Items.Add(value);
            }
        }

        private void GenerateUniqueNumbers(int from, int to, int count)
        {
            var used = new HashSet<int>();
            int maxAttempts = count * 10;
            int attempts = 0;

            while (used.Count < count && attempts < maxAttempts)
            {
                int value = _random.Next(from, to + 1);
                if (used.Add(value))
                {
                    lstResults.Items.Add(value);
                }
                attempts++;
            }

            // При корректном диапазоне и проверке сверху проблем быть не должно [web:174][web:177]
        }
    }
}