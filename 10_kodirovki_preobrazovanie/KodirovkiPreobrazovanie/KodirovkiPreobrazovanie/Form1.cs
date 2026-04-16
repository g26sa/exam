using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KodirovkiPreobrazovanie
{
    public class Form1 : Form
    {
        private TextBox txtInput;
        private TextBox txtOutput;
        private ComboBox cmbFromEncoding;
        private ComboBox cmbToEncoding;
        private Label lblInput;
        private Label lblOutput;
        private Label lblFrom;
        private Label lblTo;
        private Button btnConvert;
        private Button btnSwap;

        private Dictionary<string, Encoding> _encodings;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Кодировки и преобразование текста";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(800, 450);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Подготовим словарь кодировок
            _encodings = new Dictionary<string, Encoding>
            {
                { "UTF-8", Encoding.UTF8 },
                { "Windows-1251", Encoding.GetEncoding("windows-1251") },
                { "ASCII", Encoding.ASCII },
                { "Unicode (UTF-16)", Encoding.Unicode }
            };

            // Исходный текст
            lblInput = new Label();
            lblInput.Text = "Исходный текст:";
            lblInput.Location = new Point(10, 10);
            lblInput.AutoSize = true;
            this.Controls.Add(lblInput);

            txtInput = new TextBox();
            txtInput.Location = new Point(10, 30);
            txtInput.Size = new Size(770, 140);
            txtInput.Multiline = true;
            txtInput.ScrollBars = ScrollBars.Vertical;
            txtInput.BackColor = Color.White;
            txtInput.ForeColor = Color.FromArgb(32, 32, 32);
            this.Controls.Add(txtInput);

            // Кодировки
            lblFrom = new Label();
            lblFrom.Text = "Из кодировки:";
            lblFrom.Location = new Point(10, 180);
            lblFrom.AutoSize = true;
            this.Controls.Add(lblFrom);

            cmbFromEncoding = new ComboBox();
            cmbFromEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFromEncoding.Location = new Point(120, 178);
            cmbFromEncoding.Size = new Size(180, 25);
            this.Controls.Add(cmbFromEncoding);

            lblTo = new Label();
            lblTo.Text = "В кодировку:";
            lblTo.Location = new Point(320, 180);
            lblTo.AutoSize = true;
            this.Controls.Add(lblTo);

            cmbToEncoding = new ComboBox();
            cmbToEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbToEncoding.Location = new Point(420, 178);
            cmbToEncoding.Size = new Size(180, 25);
            this.Controls.Add(cmbToEncoding);

            btnSwap = CreateButton("↔", 610, 176, 40, 28);
            btnSwap.Click += BtnSwap_Click;

            btnConvert = CreateButton("Преобразовать", 660, 176, 120, 28);
            btnConvert.Click += BtnConvert_Click;

            // Результат
            lblOutput = new Label();
            lblOutput.Text = "Результат:";
            lblOutput.Location = new Point(10, 220);
            lblOutput.AutoSize = true;
            this.Controls.Add(lblOutput);

            txtOutput = new TextBox();
            txtOutput.Location = new Point(10, 240);
            txtOutput.Size = new Size(770, 180);
            txtOutput.Multiline = true;
            txtOutput.ScrollBars = ScrollBars.Vertical;
            txtOutput.BackColor = Color.White;
            txtOutput.ForeColor = Color.FromArgb(32, 32, 32);
            this.Controls.Add(txtOutput);

            // Заполняем списки кодировок
            foreach (var name in _encodings.Keys)
            {
                cmbFromEncoding.Items.Add(name);
                cmbToEncoding.Items.Add(name);
            }

            cmbFromEncoding.SelectedItem = "Windows-1251";
            cmbToEncoding.SelectedItem = "UTF-8";
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

        private void BtnSwap_Click(object sender, EventArgs e)
        {
            // Поменять местами выбранные кодировки
            var from = cmbFromEncoding.SelectedItem;
            var to = cmbToEncoding.SelectedItem;
            cmbFromEncoding.SelectedItem = to;
            cmbToEncoding.SelectedItem = from;
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();

            if (cmbFromEncoding.SelectedItem == null || cmbToEncoding.SelectedItem == null)
            {
                MessageBox.Show("Выберите кодировки \"Из\" и \"В\".");
                return;
            }

            string fromName = cmbFromEncoding.SelectedItem.ToString();
            string toName = cmbToEncoding.SelectedItem.ToString();

            Encoding srcEncoding = _encodings[fromName];
            Encoding dstEncoding = _encodings[toName];

            string input = txtInput.Text;

            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Введите текст для преобразования.");
                return;
            }

            try
            {
                string result = ConvertEncoding(input, srcEncoding, dstEncoding);
                txtOutput.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при преобразовании: " + ex.Message);
            }
        }

        // Универсальная функция конвертации строки между кодировками [web:119][web:123][web:121]
        private string ConvertEncoding(string text, Encoding srcEncoding, Encoding dstEncoding)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Строка в .NET уже в UTF-16, но мы моделируем ситуацию,
            // когда text считается «находящимся» в srcEncoding.
            byte[] srcBytes = srcEncoding.GetBytes(text);
            byte[] dstBytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(dstBytes);
        }
    }
}