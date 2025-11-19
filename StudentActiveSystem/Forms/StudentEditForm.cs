using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using StudentActiveSystem.Data;

namespace StudentActiveSystem.Forms
{
    public class StudentEditForm : Form
    {
        private int? _studentId;
        private TextBox txtFullName;
        private ComboBox cmbGroup;
        private ComboBox cmbRole;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;
        private Panel panelHeader;

        public StudentEditForm(int? studentId = null)
        {
            _studentId = studentId;
            InitializeComponent();
            LoadComboBoxData();

            if (_studentId.HasValue)
            {
                LoadStudentData();
            }
        }

        private void InitializeComponent()
        {
            this.Text = _studentId.HasValue ? "Редактирование члена комиссии" : "Добавление члена комиссии";
            this.Size = new Size(520, 520);
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
                Text = _studentId.HasValue ? "РЕДАКТИРОВАНИЕ ЧЛЕНА КОМИССИИ" : "ДОБАВЛЕНИЕ ЧЛЕНА КОМИССИИ",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(520, 60),
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelHeader.Controls.Add(lblTitle);

            Label lblFullName = new Label { Text = "ФИО:", Location = new Point(30, 80), AutoSize = true, Font = new Font("Arial", 10), ForeColor = Color.FromArgb(50, 50, 50) };
            txtFullName = new TextBox { Location = new Point(30, 105), Size = new Size(440, 28), Font = new Font("Arial", 11), BorderStyle = BorderStyle.FixedSingle };

            Label lblGroup = new Label { Text = "Участок:", Location = new Point(30, 145), AutoSize = true, Font = new Font("Arial", 10), ForeColor = Color.FromArgb(50, 50, 50) };
            cmbGroup = new ComboBox { Location = new Point(30, 170), Size = new Size(440, 28), Font = new Font("Arial", 11), DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblRole = new Label { Text = "Должность:", Location = new Point(30, 210), AutoSize = true, Font = new Font("Arial", 10), ForeColor = Color.FromArgb(50, 50, 50) };
            cmbRole = new ComboBox { Location = new Point(30, 235), Size = new Size(440, 28), Font = new Font("Arial", 11), DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblEmail = new Label { Text = "Email:", Location = new Point(30, 275), AutoSize = true, Font = new Font("Arial", 10), ForeColor = Color.FromArgb(50, 50, 50) };
            txtEmail = new TextBox { Location = new Point(30, 300), Size = new Size(440, 28), Font = new Font("Arial", 11), BorderStyle = BorderStyle.FixedSingle };

            Label lblPhone = new Label { Text = "Телефон:", Location = new Point(30, 340), AutoSize = true, Font = new Font("Arial", 10), ForeColor = Color.FromArgb(50, 50, 50) };
            txtPhone = new TextBox { Location = new Point(30, 365), Size = new Size(440, 28), Font = new Font("Arial", 11), BorderStyle = BorderStyle.FixedSingle };

            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(30, 420),
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
                Location = new Point(260, 420),
                Size = new Size(210, 42),
                BackColor = Color.FromArgb(80, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { panelHeader, lblFullName, txtFullName, lblGroup, cmbGroup, lblRole, cmbRole, lblEmail, txtEmail, lblPhone, txtPhone, btnSave, btnCancel });
        }

        private void LoadComboBoxData()
        {
            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();

                    // Загрузка участков
                    string groupQuery = "SELECT Id, Name FROM Groups ORDER BY Name";
                    using (var command = new SQLiteCommand(groupQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbGroup.Items.Add(new ComboBoxItem
                            {
                                Value = reader.GetInt32(0),
                                Text = reader.GetString(1)
                            });
                        }
                    }

                    // Загрузка должностей
                    cmbRole.Items.Add(new ComboBoxItem { Value = null, Text = "Не назначена" });
                    string roleQuery = "SELECT Id, Name FROM Roles ORDER BY Name";
                    using (var command = new SQLiteCommand(roleQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbRole.Items.Add(new ComboBoxItem
                            {
                                Value = reader.GetInt32(0),
                                Text = reader.GetString(1)
                            });
                        }
                    }

                    if (cmbGroup.Items.Count > 0) cmbGroup.SelectedIndex = 0;
                    if (cmbRole.Items.Count > 0) cmbRole.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudentData()
        {
            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT FullName, GroupId, RoleId, Email, Phone FROM Students WHERE Id = @Id";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", _studentId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtFullName.Text = reader.GetString(0);

                                int groupId = reader.GetInt32(1);
                                for (int i = 0; i < cmbGroup.Items.Count; i++)
                                {
                                    if (((ComboBoxItem)cmbGroup.Items[i]).Value?.ToString() == groupId.ToString())
                                    {
                                        cmbGroup.SelectedIndex = i;
                                        break;
                                    }
                                }

                                if (!reader.IsDBNull(2))
                                {
                                    int roleId = reader.GetInt32(2);
                                    for (int i = 0; i < cmbRole.Items.Count; i++)
                                    {
                                        if (((ComboBoxItem)cmbRole.Items[i]).Value?.ToString() == roleId.ToString())
                                        {
                                            cmbRole.SelectedIndex = i;
                                            break;
                                        }
                                    }
                                }

                                txtEmail.Text = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                txtPhone.Text = reader.IsDBNull(4) ? "" : reader.GetString(4);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных члена комиссии: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Пожалуйста, введите ФИО члена комиссии!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbGroup.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите участок!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int groupId = ((ComboBoxItem)cmbGroup.SelectedItem).Value ?? 0;
                int? roleId = ((ComboBoxItem)cmbRole.SelectedItem).Value;

                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query;

                    if (_studentId.HasValue)
                    {
                        query = @"UPDATE Students SET FullName = @FullName, GroupId = @GroupId, RoleId = @RoleId,
                                 Email = @Email, Phone = @Phone WHERE Id = @Id";
                    }
                    else
                    {
                        query = @"INSERT INTO Students (FullName, GroupId, RoleId, Email, Phone)
                                 VALUES (@FullName, @GroupId, @RoleId, @Email, @Phone)";
                    }

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                        command.Parameters.AddWithValue("@GroupId", groupId);
                        command.Parameters.AddWithValue("@RoleId", roleId.HasValue ? (object)roleId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        command.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());

                        if (_studentId.HasValue)
                        {
                            command.Parameters.AddWithValue("@Id", _studentId.Value);
                        }

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Данные успешно сохранены!", "Успех",
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

        private class ComboBoxItem
        {
            public int? Value { get; set; }
            public string Text { get; set; } = string.Empty;
            public override string ToString() => Text;
        }
    }
}
