using System;
using System.Drawing;
using System.Windows.Forms;

namespace KalkulyatorBasic
{
    public class Form1 : Form
    {
        private TextBox txtDisplay;

        private Button btn0;
        private Button btn1;
        private Button btn2;
        private Button btn3;
        private Button btn4;
        private Button btn5;
        private Button btn6;
        private Button btn7;
        private Button btn8;
        private Button btn9;

        private Button btnPlus;
        private Button btnMinus;
        private Button btnMultiply;
        private Button btnDivide;
        private Button btnEquals;
        private Button btnClear;
        private Button btnSign;

        private double _firstValue = 0;
        private string _operation = "";
        private bool _isNewInput = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Настройки формы
            this.Text = "Калькулятор";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(260, 340);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Поле вывода
            txtDisplay = new TextBox();
            txtDisplay.ReadOnly = true;
            txtDisplay.Text = "0";
            txtDisplay.TextAlign = HorizontalAlignment.Right;
            txtDisplay.Font = new Font("Segoe UI", 16F);
            txtDisplay.Location = new Point(10, 10);
            txtDisplay.Size = new Size(230, 35);
            this.Controls.Add(txtDisplay);

            // Общий стиль кнопок
            Size btnSize = new Size(50, 40);
            int startX = 10;
            int startY = 60;
            int margin = 5;

            // Создаём кнопки цифр
            btn7 = CreateButton("7", startX, startY + (btnSize.Height + margin) * 0);
            btn8 = CreateButton("8", startX + (btnSize.Width + margin) * 1, startY + (btnSize.Height + margin) * 0);
            btn9 = CreateButton("9", startX + (btnSize.Width + margin) * 2, startY + (btnSize.Height + margin) * 0);

            btn4 = CreateButton("4", startX, startY + (btnSize.Height + margin) * 1);
            btn5 = CreateButton("5", startX + (btnSize.Width + margin) * 1, startY + (btnSize.Height + margin) * 1);
            btn6 = CreateButton("6", startX + (btnSize.Width + margin) * 2, startY + (btnSize.Height + margin) * 1);

            btn1 = CreateButton("1", startX, startY + (btnSize.Height + margin) * 2);
            btn2 = CreateButton("2", startX + (btnSize.Width + margin) * 1, startY + (btnSize.Height + margin) * 2);
            btn3 = CreateButton("3", startX + (btnSize.Width + margin) * 2, startY + (btnSize.Height + margin) * 2);

            btn0 = CreateButton("0", startX, startY + (btnSize.Height + margin) * 3);
            btn0.Size = new Size(btnSize.Width * 2 + margin, btnSize.Height);

            // Кнопки операций
            btnPlus = CreateButton("+", startX + (btnSize.Width + margin) * 3, startY + (btnSize.Height + margin) * 0);
            btnMinus = CreateButton("-", startX + (btnSize.Width + margin) * 3, startY + (btnSize.Height + margin) * 1);
            btnMultiply = CreateButton("*", startX + (btnSize.Width + margin) * 3, startY + (btnSize.Height + margin) * 2);
            btnDivide = CreateButton("/", startX + (btnSize.Width + margin) * 3, startY + (btnSize.Height + margin) * 3);

            btnEquals = CreateButton("=", startX + (btnSize.Width + margin) * 2, startY + (btnSize.Height + margin) * 3);
            btnClear = CreateButton("C", startX, startY + (btnSize.Height + margin) * 4);
            btnSign = CreateButton("+/-", startX + (btnSize.Width + margin) * 1, startY + (btnSize.Height + margin) * 4);

            // Подключаем обработчики для цифр
            btn0.Click += DigitButton_Click;
            btn1.Click += DigitButton_Click;
            btn2.Click += DigitButton_Click;
            btn3.Click += DigitButton_Click;
            btn4.Click += DigitButton_Click;
            btn5.Click += DigitButton_Click;
            btn6.Click += DigitButton_Click;
            btn7.Click += DigitButton_Click;
            btn8.Click += DigitButton_Click;
            btn9.Click += DigitButton_Click;

            // Обработчики операций
            btnPlus.Click += OperationButton_Click;
            btnMinus.Click += OperationButton_Click;
            btnMultiply.Click += OperationButton_Click;
            btnDivide.Click += OperationButton_Click;

            // Обработчики специальных кнопок
            btnEquals.Click += btnEquals_Click;
            btnClear.Click += btnClear_Click;
            btnSign.Click += btnSign_Click;
        }

        private Button CreateButton(string text, int x, int y)
        {
            var button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(50, 40);
            button.BackColor = Color.FromArgb(47, 79, 111);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            this.Controls.Add(button);
            return button;
        }

        private void DigitButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (_isNewInput || txtDisplay.Text == "0")
            {
                txtDisplay.Text = button.Text;
                _isNewInput = false;
            }
            else
            {
                txtDisplay.Text += button.Text;
            }
        }

        private void OperationButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            if (double.TryParse(txtDisplay.Text, out var value))
            {
                _firstValue = value;
                _operation = button.Text; // "+", "-", "*", "/"
                _isNewInput = true;
            }
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtDisplay.Text, out var secondValue))
                return;

            double result = _firstValue;

            switch (_operation)
            {
                case "+":
                    result = _firstValue + secondValue;
                    break;
                case "-":
                    result = _firstValue - secondValue;
                    break;
                case "*":
                    result = _firstValue * secondValue;
                    break;
                case "/":
                    if (secondValue == 0)
                    {
                        MessageBox.Show("Деление на ноль невозможно");
                        return;
                    }
                    result = _firstValue / secondValue;
                    break;
            }

            txtDisplay.Text = result.ToString();
            _isNewInput = true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtDisplay.Text = "0";
            _firstValue = 0;
            _operation = "";
            _isNewInput = true;
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtDisplay.Text, out var value))
            {
                value = -value;
                txtDisplay.Text = value.ToString();
            }
        }
    }
}