using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BystroyeSozdanieZametok
{
    public class Form1 : Form
    {
        private TextBox txtNote;
        private Button btnClear;
        private Label lblInfo;

        private string _notesFilePath;

        public Form1()
        {
            InitializeComponent();
            InitNotesFilePath();
            LoadNote();
        }

        private void InitializeComponent()
        {
            this.Text = "Быстрые заметки";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(700, 420);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ForeColor = Color.FromArgb(32, 32, 32);
            this.Font = new Font("Segoe UI", 10F);

            // Информация
            lblInfo = new Label();
            lblInfo.Text = "Пишите заметку. Содержимое автоматически сохраняется.";
            lblInfo.Location = new Point(10, 10);
            lblInfo.AutoSize = true;
            this.Controls.Add(lblInfo);

            // Поле заметки
            txtNote = new TextBox();
            txtNote.Location = new Point(10, 35);
            txtNote.Size = new Size(670, 330);
            txtNote.Multiline = true;
            txtNote.ScrollBars = ScrollBars.Vertical;
            txtNote.BackColor = Color.White;
            txtNote.Font = new Font("Consolas", 11F);
            txtNote.TextChanged += TxtNote_TextChanged;
            this.Controls.Add(txtNote);

            // Кнопка очистки
            btnClear = new Button();
            btnClear.Text = "Очистить";
            btnClear.Location = new Point(10, 375);
            btnClear.Size = new Size(100, 30);
            btnClear.BackColor = Color.FromArgb(47, 79, 111);
            btnClear.ForeColor = Color.White;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += BtnClear_Click;
            this.Controls.Add(btnClear);

            this.FormClosing += Form1_FormClosing;
        }

        private void InitNotesFilePath()
        {
            // Будем хранить заметку в файле notes.txt рядом с exe
            // Такой способ часто используют для простых заметочников и автосохранений [web:248][web:247]
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _notesFilePath = Path.Combine(baseDir, "notes.txt");
        }

        private void LoadNote()
        {
            try
            {
                if (File.Exists(_notesFilePath))
                {
                    txtNote.Text = File.ReadAllText(_notesFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить заметку: {ex.Message}");
            }
        }

        private void SaveNote()
        {
            try
            {
                // Для небольшого объёма текста достаточно простого File.WriteAllText [web:247][web:244]
                File.WriteAllText(_notesFilePath, txtNote.Text);
            }
            catch (Exception ex)
            {
                // Не спамим MessageBox, чтобы не мешать пользователю
                // Можно писать в лог/игнорировать, но тут просто один раз покажем
                Console.WriteLine("Ошибка сохранения заметки: " + ex.Message);
            }
        }

        private void TxtNote_TextChanged(object sender, EventArgs e)
        {
            // Автосохранение при каждом изменении.
            // Для очень частого набора можно было бы добавить таймер, как в примерах авто-бэкапа блокнота [web:248],
            // но для учебной утилиты достаточно прямого сохранения.
            SaveNote();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Очистить текущую заметку?\nТекст будет удалён и файл перезаписан.",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                txtNote.Clear();
                SaveNote();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // На всякий случай сохраняем при закрытии
            SaveNote();
        }
    }
}