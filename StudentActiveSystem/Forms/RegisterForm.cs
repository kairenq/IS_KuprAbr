using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using StudentActiveSystem.Data;

namespace StudentActiveSystem.Forms
{
    public class RegisterForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtFullName;
        private Button btnRegister;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblConfirmPassword;
        private Label lblFullName;
        private Panel panelMain;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Регистрация - Студенческий актив";
            this.Size = new Size(600, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Главная панель
            panelMain = new Panel
            {
                Size = new Size(480, 520),
                Location = new Point(60, 40),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Заголовок
            lblTitle = new Label
            {
                Text = "РЕГИСТРАЦИЯ",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                AutoSize = false,
                Size = new Size(420, 50),
                Location = new Point(30, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ФИО
            lblFullName = new Label
            {
                Text = "ФИО:",
                Location = new Point(40, 100),
                AutoSize = true,
                Font = new Font("Segoe UI", 11)
            };

            txtFullName = new TextBox
            {
                Location = new Point(40, 130),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 12)
            };

            // Имя пользователя
            lblUsername = new Label
            {
                Text = "Имя пользователя:",
                Location = new Point(40, 185),
                AutoSize = true,
                Font = new Font("Segoe UI", 11)
            };

            txtUsername = new TextBox
            {
                Location = new Point(40, 215),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 12)
            };

            // Пароль
            lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(40, 270),
                AutoSize = true,
                Font = new Font("Segoe UI", 11)
            };

            txtPassword = new TextBox
            {
                Location = new Point(40, 300),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 12),
                PasswordChar = '●'
            };

            // Подтверждение пароля
            lblConfirmPassword = new Label
            {
                Text = "Подтвердите пароль:",
                Location = new Point(40, 355),
                AutoSize = true,
                Font = new Font("Segoe UI", 11)
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(40, 385),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 12),
                PasswordChar = '●'
            };

            // Кнопка регистрации
            btnRegister = new Button
            {
                Text = "Зарегистрироваться",
                Location = new Point(40, 450),
                Size = new Size(190, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            // Кнопка отмены
            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(250, 450),
                Size = new Size(190, 45),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            // Добавление контролов на панель
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(lblFullName);
            panelMain.Controls.Add(txtFullName);
            panelMain.Controls.Add(lblUsername);
            panelMain.Controls.Add(txtUsername);
            panelMain.Controls.Add(lblPassword);
            panelMain.Controls.Add(txtPassword);
            panelMain.Controls.Add(lblConfirmPassword);
            panelMain.Controls.Add(txtConfirmPassword);
            panelMain.Controls.Add(btnRegister);
            panelMain.Controls.Add(btnCancel);

            this.Controls.Add(panelMain);
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Валидация
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать минимум 4 символа!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string passwordHash = DatabaseInitializer.HashPassword(password);

                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();

                    // Проверка существования пользователя
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Username", username);
                        long count = (long)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Пользователь с таким именем уже существует!", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Регистрация нового пользователя
                    string insertQuery = @"
                        INSERT INTO Users (Username, PasswordHash, FullName, CreatedAt, IsAdmin)
                        VALUES (@Username, @PasswordHash, @FullName, @CreatedAt, 0)";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        command.Parameters.AddWithValue("@FullName", fullName);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        command.ExecuteNonQuery();

                        MessageBox.Show("Регистрация прошла успешно!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
