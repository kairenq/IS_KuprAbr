using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using StudentActiveSystem.Data;

namespace StudentActiveSystem.Forms
{
    public class GroupEditForm : Form
    {
        private int? _groupId;
        private TextBox txtName;
        private TextBox txtFaculty;
        private NumericUpDown numCourse;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;
        private Panel panelHeader;

        public GroupEditForm(int? groupId = null)
        {
            _groupId = groupId;
            InitializeComponent();

            if (_groupId.HasValue)
            {
                LoadGroupData();
            }
        }

        private void InitializeComponent()
        {
            this.Text = _groupId.HasValue ? "Редактирование участка" : "Добавление участка";
            this.Size = new Size(520, 420);
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
                Text = _groupId.HasValue ? "РЕДАКТИРОВАНИЕ УЧАСТКА" : "ДОБАВЛЕНИЕ УЧАСТКА",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(520, 60),
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelHeader.Controls.Add(lblTitle);

            Label lblName = new Label { Text = "Название участка:", Location = new Point(30, 80), AutoSize = true, Font = new Font("Arial", 10), ForeColor = Color.FromArgb(50, 50, 50) };
            txtName = new TextBox { Location = new Point(30, 105), Size = new Size(440, 28), Font = new Font("Arial", 11), BorderStyle = BorderStyle.FixedSingle };

            Label lblFaculty = new Label { Text = "Адрес:", Location = new Point(30, 145), AutoSize = true, Font = new Font("Arial", 10), ForeColor = Color.FromArgb(50, 50, 50) };
            txtFaculty = new TextBox { Location = new Point(30, 170), Size = new Size(440, 28), Font = new Font("Arial", 11), BorderStyle = BorderStyle.FixedSingle };

            Label lblCourse = new Label { Text = "Номер участка:", Location = new Point(30, 210), AutoSize = true, Font = new Font("Arial", 10), ForeColor = Color.FromArgb(50, 50, 50) };
            numCourse = new NumericUpDown
            {
                Location = new Point(30, 235),
                Size = new Size(440, 28),
                Font = new Font("Arial", 11),
                Minimum = 1,
                Maximum = 9999,
                Value = 1001
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

            this.Controls.AddRange(new Control[] { panelHeader, lblName, txtName, lblFaculty, txtFaculty, lblCourse, numCourse, btnSave, btnCancel });
        }

        private void LoadGroupData()
        {
            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT Name, Faculty, Course FROM Groups WHERE Id = @Id";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", _groupId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtName.Text = reader.GetString(0);
                                txtFaculty.Text = reader.GetString(1);
                                numCourse.Value = reader.GetInt32(2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных участка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Пожалуйста, введите название участка!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFaculty.Text))
            {
                MessageBox.Show("Пожалуйста, введите адрес участка!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query;

                    if (_groupId.HasValue)
                    {
                        query = "UPDATE Groups SET Name = @Name, Faculty = @Faculty, Course = @Course WHERE Id = @Id";
                    }
                    else
                    {
                        query = "INSERT INTO Groups (Name, Faculty, Course) VALUES (@Name, @Faculty, @Course)";
                    }

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                        command.Parameters.AddWithValue("@Faculty", txtFaculty.Text.Trim());
                        command.Parameters.AddWithValue("@Course", (int)numCourse.Value);

                        if (_groupId.HasValue)
                        {
                            command.Parameters.AddWithValue("@Id", _groupId.Value);
                        }

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Участок успешно сохранен!", "Успех",
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
