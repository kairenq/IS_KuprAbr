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
        private Panel panelHeader;

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
            this.Text = _roleId.HasValue ? "Редактирование должности" : "Добавление должности";
            this.Size = new Size(520, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Верхняя панель
            panelHeader = new Panel
            {
                Size = new Size(520, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(140, 20, 20)
            };

            lblTitle = new Label
            {
                Text = _roleId.HasValue ? "РЕДАКТИРОВАНИЕ ДОЛЖНОСТИ" : "ДОБАВЛЕНИЕ ДОЛЖНОСТИ",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(520, 60),
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelHeader.Controls.Add(lblTitle);

            Label lblName = new Label { Text = "Название должности:", Location = new Point(30, 80), AutoSize = true, Font = new Font("Arial", 10), ForeColor = Color.FromArgb(50, 50, 50) };
            txtName = new TextBox { Location = new Point(30, 105), Size = new Size(440, 28), Font = new Font("Arial", 11), BorderStyle = BorderStyle.FixedSingle };

            Label lblDescription = new Label { Text = "Описание:", Location = new Point(30, 145), AutoSize = true, Font = new Font("Arial", 10), ForeColor = Color.FromArgb(50, 50, 50) };
            txtDescription = new TextBox
            {
                Location = new Point(30, 170),
                Size = new Size(440, 100),
                Font = new Font("Arial", 11),
                Multiline = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(30, 300),
                Size = new Size(210, 42),
                BackColor = Color.FromArgb(140, 20, 20),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(260, 300),
                Size = new Size(210, 42),
                BackColor = Color.FromArgb(80, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { panelHeader, lblName, txtName, lblDescription, txtDescription, btnSave, btnCancel });
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
                MessageBox.Show($"Ошибка загрузки данных должности: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Пожалуйста, введите название должности!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Пожалуйста, введите описание должности!", "Ошибка",
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

                MessageBox.Show("Должность успешно сохранена!", "Успех",
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
