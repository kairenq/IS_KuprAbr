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
            this.Text = _groupId.HasValue ? "Редактирование группы" : "Добавление группы";
            this.Size = new Size(500, 380);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            lblTitle = new Label
            {
                Text = _groupId.HasValue ? "РЕДАКТИРОВАНИЕ ГРУППЫ" : "ДОБАВЛЕНИЕ ГРУППЫ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label lblName = new Label { Text = "Название группы:", Location = new Point(20, 70), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtName = new TextBox { Location = new Point(20, 95), Size = new Size(440, 30), Font = new Font("Segoe UI", 11) };

            Label lblFaculty = new Label { Text = "Факультет:", Location = new Point(20, 135), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtFaculty = new TextBox { Location = new Point(20, 160), Size = new Size(440, 30), Font = new Font("Segoe UI", 11) };

            Label lblCourse = new Label { Text = "Курс:", Location = new Point(20, 200), AutoSize = true, Font = new Font("Segoe UI", 10) };
            numCourse = new NumericUpDown
            {
                Location = new Point(20, 225),
                Size = new Size(440, 30),
                Font = new Font("Segoe UI", 11),
                Minimum = 1,
                Maximum = 6,
                Value = 1
            };

            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(20, 280),
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
                Location = new Point(260, 280),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { lblTitle, lblName, txtName, lblFaculty, txtFaculty, lblCourse, numCourse, btnSave, btnCancel });
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
                MessageBox.Show($"Ошибка загрузки данных группы: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Пожалуйста, введите название группы!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFaculty.Text))
            {
                MessageBox.Show("Пожалуйста, введите факультет!", "Ошибка",
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

                MessageBox.Show("Группа успешно сохранена!", "Успех",
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
