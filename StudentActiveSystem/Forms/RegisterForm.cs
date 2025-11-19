using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using StudentActiveSystem.Data;

namespace StudentActiveSystem.Forms
{
    public class RegisterForm : Form
    {
        private TextBox txtFullName;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblFullName;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblConfirmPassword;
        private Panel panelLeft;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Регистрация - БППК";
            this.Size = new Size(600, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 250);

            // Левая декоративная панель
            panelLeft = new Panel
            {
                Size = new Size(180, 480),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(25, 55, 109)
            };

            Label lblLogo = new Label
            {
                Text = "БППК",
                Font = new Font("Tahoma", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 180),
                AutoSize = true
            };
            panelLeft.Controls.Add(lblLogo);

            // Заголовок формы
            lblTitle = new Label
            {
                Text = "Создание аккаунта",
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 55, 109),
                Location = new Point(210, 30),
                AutoSize = true
            };

            // ФИО
            lblFullName = new Label
            {
                Text = "ФИО:",
                Location = new Point(210, 80),
                AutoSize = true,
                Font = new Font("Tahoma", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtFullName = new TextBox
            {
                Location = new Point(210, 105),
                Size = new Size(350, 28),
                Font = new Font("Tahoma", 11),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Логин
            lblUsername = new Label
            {
                Text = "Имя пользователя:",
                Location = new Point(210, 150),
                AutoSize = true,
                Font = new Font("Tahoma", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtUsername = new TextBox
            {
                Location = new Point(210, 175),
                Size = new Size(350, 28),
                Font = new Font("Tahoma", 11),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Пароль
            lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(210, 220),
                AutoSize = true,
                Font = new Font("Tahoma", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtPassword = new TextBox
            {
                Location = new Point(210, 245),
                Size = new Size(350, 28),
                Font = new Font("Tahoma", 11),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle
            };

            // Подтверждение пароля
            lblConfirmPassword = new Label
            {
                Text = "Подтвердите пароль:",
                Location = new Point(210, 290),
                AutoSize = true,
                Font = new Font("Tahoma", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(210, 315),
                Size = new Size(350, 28),
                Font = new Font("Tahoma", 11),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle
            };

            // Кнопки
            btnRegister = new Button
            {
                Text = "Зарегистрироваться",
                Location = new Point(210, 375),
                Size = new Size(170, 42),
                BackColor = Color.FromArgb(25, 55, 109),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(390, 375),
                Size = new Size(170, 42),
                BackColor = Color.FromArgb(120, 120, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { panelLeft, lblTitle, lblFullName, txtFullName, lblUsername, txtUsername, lblPassword, txtPassword, lblConfirmPassword, txtConfirmPassword, btnRegister, btnCancel });
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать минимум 4 символа!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();

                    // Проверка существования пользователя
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Username", txtUsername.Text);
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
                    using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Username", txtUsername.Text);
                        insertCommand.Parameters.AddWithValue("@PasswordHash", DatabaseInitializer.HashPassword(txtPassword.Text));
                        insertCommand.Parameters.AddWithValue("@FullName", txtFullName.Text);
                        insertCommand.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        insertCommand.ExecuteNonQuery();
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
