using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using StudentActiveSystem.Data;

namespace StudentActiveSystem.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnInstruction;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private Panel panelMain;

        public LoginForm()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Вход в систему - Студенческий актив";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Главная панель
            panelMain = new Panel
            {
                Size = new Size(350, 320),
                Location = new Point(50, 30),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Заголовок
            lblTitle = new Label
            {
                Text = "СТУДЕНЧЕСКИЙ АКТИВ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                AutoSize = false,
                Size = new Size(300, 40),
                Location = new Point(25, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Метка имени пользователя
            lblUsername = new Label
            {
                Text = "Имя пользователя:",
                Location = new Point(30, 80),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            // Поле имени пользователя
            txtUsername = new TextBox
            {
                Location = new Point(30, 105),
                Size = new Size(290, 30),
                Font = new Font("Segoe UI", 11)
            };

            // Метка пароля
            lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(30, 145),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            // Поле пароля
            txtPassword = new TextBox
            {
                Location = new Point(30, 170),
                Size = new Size(290, 30),
                Font = new Font("Segoe UI", 11),
                PasswordChar = '●'
            };

            // Кнопка входа
            btnLogin = new Button
            {
                Text = "Войти",
                Location = new Point(30, 220),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // Кнопка регистрации
            btnRegister = new Button
            {
                Text = "Регистрация",
                Location = new Point(180, 220),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            // Кнопка инструкции
            btnInstruction = new Button
            {
                Text = "📖 Инструкция пользователя",
                Location = new Point(30, 270),
                Size = new Size(290, 35),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnInstruction.FlatAppearance.BorderSize = 0;
            btnInstruction.Click += BtnInstruction_Click;

            // Добавление контролов на панель
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(lblUsername);
            panelMain.Controls.Add(txtUsername);
            panelMain.Controls.Add(lblPassword);
            panelMain.Controls.Add(txtPassword);
            panelMain.Controls.Add(btnLogin);
            panelMain.Controls.Add(btnRegister);
            panelMain.Controls.Add(btnInstruction);

            this.Controls.Add(panelMain);
        }

        private void SetupUI()
        {
            // Обработка Enter для входа
            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string passwordHash = DatabaseInitializer.HashPassword(password);

                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT Id, FullName, IsAdmin FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string fullName = reader.GetString(1);
                                bool isAdmin = reader.GetInt32(2) == 1;

                                MessageBox.Show($"Добро пожаловать, {fullName}!", "Успешный вход",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Открываем главную форму
                                this.Hide();
                                MainForm mainForm = new MainForm(userId, fullName, isAdmin);
                                mainForm.FormClosed += (s, args) => this.Close();
                                mainForm.Show();
                            }
                            else
                            {
                                MessageBox.Show("Неверное имя пользователя или пароль!", "Ошибка входа",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }

        private void BtnInstruction_Click(object? sender, EventArgs e)
        {
            try
            {
                string instructionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Resources", "Инструкция_пользователя.docx");

                if (File.Exists(instructionPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = instructionPath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show($"Файл инструкции не найден:\n{instructionPath}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть инструкцию: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
