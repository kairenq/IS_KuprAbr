using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using StudentActiveSystem.Data;

namespace StudentActiveSystem.Forms
{
    public class RoleEditForm : Form
    {
        private int? _roleId;
        private TextBox txtName;
        private TextBox txtDescription;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;

        public RoleEditForm(int? roleId = null)
        {
            _roleId = roleId;
            InitializeComponent();

            if (_roleId.HasValue)
            {
                LoadRoleData();
            }
        }

        private void InitializeComponent()
        {
            this.Text = _roleId.HasValue ? "Редактирование роли" : "Добавление роли";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            lblTitle = new Label
            {
                Text = _roleId.HasValue ? "РЕДАКТИРОВАНИЕ РОЛИ" : "ДОБАВЛЕНИЕ РОЛИ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label lblName = new Label { Text = "Название роли:", Location = new Point(20, 70), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtName = new TextBox { Location = new Point(20, 95), Size = new Size(440, 30), Font = new Font("Segoe UI", 11) };

            Label lblDescription = new Label { Text = "Описание:", Location = new Point(20, 135), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtDescription = new TextBox
            {
                Location = new Point(20, 160),
                Size = new Size(440, 80),
                Font = new Font("Segoe UI", 11),
                Multiline = true
            };

            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(20, 260),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(260, 260),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { lblTitle, lblName, txtName, lblDescription, txtDescription, btnSave, btnCancel });
        }

        private void LoadRoleData()
        {
            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT Name, Description FROM Roles WHERE Id = @Id";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", _roleId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtName.Text = reader.GetString(0);
                                txtDescription.Text = reader.GetString(1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных роли: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Пожалуйста, введите название роли!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Пожалуйста, введите описание роли!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query;

                    if (_roleId.HasValue)
                    {
                        query = "UPDATE Roles SET Name = @Name, Description = @Description WHERE Id = @Id";
                    }
                    else
                    {
                        query = "INSERT INTO Roles (Name, Description) VALUES (@Name, @Description)";
                    }

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                        command.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());

                        if (_roleId.HasValue)
                        {
                            command.Parameters.AddWithValue("@Id", _roleId.Value);
                        }

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Роль успешно сохранена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
