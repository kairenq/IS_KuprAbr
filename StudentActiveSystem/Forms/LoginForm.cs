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
        private Label lblSubtitle;
        private Label lblUsername;
        private Label lblPassword;
        private Panel panelMain;
        private Panel panelHeader;

        public LoginForm()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Избирательная комиссия - Вход";
            this.Size = new Size(520, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Верхняя панель с заголовком
            panelHeader = new Panel
            {
                Size = new Size(520, 110),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(140, 20, 20)
            };

            // Заголовок
            lblTitle = new Label
            {
                Text = "ИЗБИРАТЕЛЬНАЯ КОМИССИЯ",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(520, 45),
                Location = new Point(0, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Подзаголовок
            lblSubtitle = new Label
            {
                Text = "Система учета членов комиссии",
                Font = new Font("Arial", 10),
                ForeColor = Color.FromArgb(255, 200, 200),
                AutoSize = false,
                Size = new Size(520, 25),
                Location = new Point(0, 65),
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblSubtitle);

            // Главная панель с полями
            panelMain = new Panel
            {
                Size = new Size(400, 370),
                Location = new Point(60, 140),
                BackColor = Color.FromArgb(248, 248, 248),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Метка имени пользователя
            lblUsername = new Label
            {
                Text = "Логин:",
                Location = new Point(30, 35),
                AutoSize = true,
                Font = new Font("Arial", 11),
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            // Поле имени пользователя
            txtUsername = new TextBox
            {
                Location = new Point(30, 65),
                Size = new Size(340, 30),
                Font = new Font("Arial", 12),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Метка пароля
            lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(30, 115),
                AutoSize = true,
                Font = new Font("Arial", 11),
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            // Поле пароля
            txtPassword = new TextBox
            {
                Location = new Point(30, 145),
                Size = new Size(340, 30),
                Font = new Font("Arial", 12),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle
            };

            // Кнопка входа
            btnLogin = new Button
            {
                Text = "ВОЙТИ",
                Location = new Point(30, 205),
                Size = new Size(340, 48),
                BackColor = Color.FromArgb(140, 20, 20),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 13, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // Кнопка регистрации
            btnRegister = new Button
            {
                Text = "Регистрация",
                Location = new Point(30, 265),
                Size = new Size(165, 40),
                BackColor = Color.FromArgb(80, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            // Кнопка инструкции
            btnInstruction = new Button
            {
                Text = "Справка",
                Location = new Point(205, 265),
                Size = new Size(165, 40),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10),
                Cursor = Cursors.Hand
            };
            btnInstruction.FlatAppearance.BorderSize = 0;
            btnInstruction.Click += BtnInstruction_Click;

            // Добавление контролов на панель
            panelMain.Controls.Add(lblUsername);
            panelMain.Controls.Add(txtUsername);
            panelMain.Controls.Add(lblPassword);
            panelMain.Controls.Add(txtPassword);
            panelMain.Controls.Add(btnLogin);
            panelMain.Controls.Add(btnRegister);
            panelMain.Controls.Add(btnInstruction);

            this.Controls.Add(panelHeader);
            this.Controls.Add(panelMain);
        }

        private void SetupUI()
        {
            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Внимание",
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

                                MessageBox.Show($"Добро пожаловать, {fullName}!", "Вход выполнен",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                this.Hide();
                                MainForm mainForm = new MainForm(userId, fullName, isAdmin);
                                mainForm.FormClosed += (s, args) => this.Close();
                                mainForm.Show();
                            }
                            else
                            {
                                MessageBox.Show("Неверный логин или пароль!", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
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
                    "..", "..", "..", "..", "Инструкция_пользователя.docx");
                instructionPath = Path.GetFullPath(instructionPath);

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
                    MessageBox.Show($"Файл справки не найден:\n{instructionPath}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
