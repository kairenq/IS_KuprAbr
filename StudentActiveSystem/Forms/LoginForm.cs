using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
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
        private Panel panelLeft;
        private Panel panelRight;

        public string LoggedInUser { get; private set; } = string.Empty;
        public string LoggedInFullName { get; private set; } = string.Empty;
        public bool IsAdmin { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "БППК - Приемная комиссия";
            this.Size = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 250);

            // Левая панель с информацией
            panelLeft = new Panel
            {
                Size = new Size(280, 450),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(25, 55, 109)
            };

            lblTitle = new Label
            {
                Text = "БППК",
                Font = new Font("Tahoma", 32, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 60),
                AutoSize = true
            };

            lblSubtitle = new Label
            {
                Text = "Приемная комиссия\n\nул. Почтовая, 4",
                Font = new Font("Tahoma", 11),
                ForeColor = Color.FromArgb(200, 210, 230),
                Location = new Point(20, 120),
                Size = new Size(240, 80)
            };

            btnInstruction = new Button
            {
                Text = "Инструкция",
                Location = new Point(20, 350),
                Size = new Size(240, 35),
                BackColor = Color.FromArgb(45, 75, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 9),
                Cursor = Cursors.Hand
            };
            btnInstruction.FlatAppearance.BorderSize = 0;
            btnInstruction.Click += BtnInstruction_Click;

            panelLeft.Controls.Add(lblTitle);
            panelLeft.Controls.Add(lblSubtitle);
            panelLeft.Controls.Add(btnInstruction);

            // Правая панель с формой входа
            panelRight = new Panel
            {
                Size = new Size(420, 450),
                Location = new Point(280, 0),
                BackColor = Color.FromArgb(245, 245, 250)
            };

            Label lblFormTitle = new Label
            {
                Text = "Вход в систему",
                Font = new Font("Tahoma", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 55, 109),
                Location = new Point(40, 60),
                AutoSize = true
            };

            lblUsername = new Label
            {
                Text = "Имя пользователя",
                Location = new Point(40, 120),
                AutoSize = true,
                Font = new Font("Tahoma", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtUsername = new TextBox
            {
                Location = new Point(40, 145),
                Size = new Size(320, 28),
                Font = new Font("Tahoma", 11),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblPassword = new Label
            {
                Text = "Пароль",
                Location = new Point(40, 190),
                AutoSize = true,
                Font = new Font("Tahoma", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            txtPassword = new TextBox
            {
                Location = new Point(40, 215),
                Size = new Size(320, 28),
                Font = new Font("Tahoma", 11),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle
            };

            btnLogin = new Button
            {
                Text = "Войти",
                Location = new Point(40, 280),
                Size = new Size(320, 45),
                BackColor = Color.FromArgb(25, 55, 109),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnRegister = new Button
            {
                Text = "Создать аккаунт",
                Location = new Point(40, 335),
                Size = new Size(320, 40),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(25, 55, 109),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 10),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderColor = Color.FromArgb(25, 55, 109);
            btnRegister.FlatAppearance.BorderSize = 1;
            btnRegister.Click += BtnRegister_Click;

            panelRight.Controls.AddRange(new Control[] { lblFormTitle, lblUsername, txtUsername, lblPassword, txtPassword, btnLogin, btnRegister });

            this.Controls.Add(panelLeft);
            this.Controls.Add(panelRight);

            txtPassword.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) BtnLogin_Click(s, e); };
            txtUsername.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) txtPassword.Focus(); };
        }

        private void BtnInstruction_Click(object? sender, EventArgs e)
        {
            string instructionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Инструкция_пользователя.docx");

            if (!File.Exists(instructionPath))
            {
                instructionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Инструкция_пользователя.docx");
            }

            if (File.Exists(instructionPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = instructionPath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть инструкцию: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Файл инструкции не найден!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Пожалуйста, введите имя пользователя и пароль!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT FullName, IsAdmin FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", txtUsername.Text);
                        command.Parameters.AddWithValue("@PasswordHash", DatabaseInitializer.HashPassword(txtPassword.Text));

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                LoggedInUser = txtUsername.Text;
                                LoggedInFullName = reader.GetString(0);
                                IsAdmin = reader.GetInt32(1) == 1;
                                this.DialogResult = DialogResult.OK;
                                this.Close();
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
            using (var registerForm = new RegisterForm())
            {
                if (registerForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
