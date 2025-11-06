using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using StudentActiveSystem.Data;
using StudentActiveSystem.Models;

namespace StudentActiveSystem.Forms
{
    public class MainForm : Form
    {
        private int _userId;
        private string _fullName;
        private bool _isAdmin;

        private TabControl tabControl;
        private TabPage tabStudents;
        private TabPage tabGroups;
        private TabPage tabRoles;

        private DataGridView dgvStudents;
        private DataGridView dgvGroups;
        private DataGridView dgvRoles;

        private Button btnAddStudent;
        private Button btnEditStudent;
        private Button btnDeleteStudent;
        private Button btnRefreshStudents;

        private Button btnAddGroup;
        private Button btnEditGroup;
        private Button btnDeleteGroup;
        private Button btnRefreshGroups;

        private Button btnAddRole;
        private Button btnEditRole;
        private Button btnDeleteRole;
        private Button btnRefreshRoles;

        private Label lblWelcome;

        public MainForm(int userId, string fullName, bool isAdmin)
        {
            _userId = userId;
            _fullName = fullName;
            _isAdmin = isAdmin;

            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Студенческий актив - Главное меню";
            this.Size = new Size(1280, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Приветствие
            lblWelcome = new Label
            {
                Text = $"Добро пожаловать, {_fullName}!",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                AutoSize = false,
                Size = new Size(1220, 50),
                Location = new Point(30, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblWelcome);

            // Вкладки
            tabControl = new TabControl
            {
                Location = new Point(30, 80),
                Size = new Size(1220, 670)
            };

            // Вкладка "Студенты"
            tabStudents = new TabPage("Студенты");
            InitializeStudentsTab();

            // Вкладка "Группы"
            tabGroups = new TabPage("Группы");
            InitializeGroupsTab();

            // Вкладка "Роли"
            tabRoles = new TabPage("Роли");
            InitializeRolesTab();

            tabControl.TabPages.Add(tabStudents);
            tabControl.TabPages.Add(tabGroups);
            tabControl.TabPages.Add(tabRoles);

            this.Controls.Add(tabControl);
        }

        private void InitializeStudentsTab()
        {
            // DataGridView для студентов
            dgvStudents = new DataGridView
            {
                Location = new Point(15, 60),
                Size = new Size(1180, 520),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            // Кнопки управления
            btnAddStudent = CreateButton("Добавить студента", 15, 595, Color.FromArgb(46, 204, 113));
            btnAddStudent.Click += BtnAddStudent_Click;

            btnEditStudent = CreateButton("Редактировать", 245, 595, Color.FromArgb(52, 152, 219));
            btnEditStudent.Click += BtnEditStudent_Click;

            btnDeleteStudent = CreateButton("Удалить", 475, 595, Color.FromArgb(231, 76, 60));
            btnDeleteStudent.Click += BtnDeleteStudent_Click;

            btnRefreshStudents = CreateButton("Обновить", 705, 595, Color.FromArgb(155, 89, 182));
            btnRefreshStudents.Click += (s, e) => LoadStudents();

            tabStudents.Controls.Add(dgvStudents);
            tabStudents.Controls.Add(btnAddStudent);
            tabStudents.Controls.Add(btnEditStudent);
            tabStudents.Controls.Add(btnDeleteStudent);
            tabStudents.Controls.Add(btnRefreshStudents);
        }

        private void InitializeGroupsTab()
        {
            dgvGroups = new DataGridView
            {
                Location = new Point(15, 60),
                Size = new Size(1180, 520),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            btnAddGroup = CreateButton("Добавить группу", 15, 595, Color.FromArgb(46, 204, 113));
            btnAddGroup.Click += BtnAddGroup_Click;

            btnEditGroup = CreateButton("Редактировать", 245, 595, Color.FromArgb(52, 152, 219));
            btnEditGroup.Click += BtnEditGroup_Click;

            btnDeleteGroup = CreateButton("Удалить", 475, 595, Color.FromArgb(231, 76, 60));
            btnDeleteGroup.Click += BtnDeleteGroup_Click;

            btnRefreshGroups = CreateButton("Обновить", 705, 595, Color.FromArgb(155, 89, 182));
            btnRefreshGroups.Click += (s, e) => LoadGroups();

            tabGroups.Controls.Add(dgvGroups);
            tabGroups.Controls.Add(btnAddGroup);
            tabGroups.Controls.Add(btnEditGroup);
            tabGroups.Controls.Add(btnDeleteGroup);
            tabGroups.Controls.Add(btnRefreshGroups);
        }

        private void InitializeRolesTab()
        {
            dgvRoles = new DataGridView
            {
                Location = new Point(15, 60),
                Size = new Size(1180, 520),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            btnAddRole = CreateButton("Добавить роль", 15, 595, Color.FromArgb(46, 204, 113));
            btnAddRole.Click += BtnAddRole_Click;

            btnEditRole = CreateButton("Редактировать", 245, 595, Color.FromArgb(52, 152, 219));
            btnEditRole.Click += BtnEditRole_Click;

            btnDeleteRole = CreateButton("Удалить", 475, 595, Color.FromArgb(231, 76, 60));
            btnDeleteRole.Click += BtnDeleteRole_Click;

            btnRefreshRoles = CreateButton("Обновить", 705, 595, Color.FromArgb(155, 89, 182));
            btnRefreshRoles.Click += (s, e) => LoadRoles();

            tabRoles.Controls.Add(dgvRoles);
            tabRoles.Controls.Add(btnAddRole);
            tabRoles.Controls.Add(btnEditRole);
            tabRoles.Controls.Add(btnDeleteRole);
            tabRoles.Controls.Add(btnRefreshRoles);
        }

        private Button CreateButton(string text, int x, int y, Color color)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(220, 45),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void LoadData()
        {
            LoadStudents();
            LoadGroups();
            LoadRoles();
        }

        private void LoadStudents()
        {
            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT s.Id, s.FullName as 'ФИО', g.Name as 'Группа',
                               IFNULL(r.Name, 'Не назначена') as 'Роль',
                               s.Email as 'Email', s.Phone as 'Телефон'
                        FROM Students s
                        LEFT JOIN Groups g ON s.GroupId = g.Id
                        LEFT JOIN Roles r ON s.RoleId = r.Id
                        ORDER BY s.FullName";

                    using (var adapter = new SQLiteDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvStudents.DataSource = dt;

                        // Скрываем колонку Id
                        if (dgvStudents.Columns["Id"] != null)
                            dgvStudents.Columns["Id"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки студентов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGroups()
        {
            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT Id, Name as 'Название', Faculty as 'Факультет', Course as 'Курс'
                        FROM Groups
                        ORDER BY Course, Name";

                    using (var adapter = new SQLiteDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvGroups.DataSource = dt;

                        if (dgvGroups.Columns["Id"] != null)
                            dgvGroups.Columns["Id"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRoles()
        {
            try
            {
                using (var connection = DatabaseContext.GetConnection())
                {
                    connection.Open();
                    string query = @"
                        SELECT Id, Name as 'Название', Description as 'Описание'
                        FROM Roles
                        ORDER BY Name";

                    using (var adapter = new SQLiteDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvRoles.DataSource = dt;

                        if (dgvRoles.Columns["Id"] != null)
                            dgvRoles.Columns["Id"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ролей: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddStudent_Click(object? sender, EventArgs e)
        {
            StudentEditForm form = new StudentEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadStudents();
            }
        }

        private void BtnEditStudent_Click(object? sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count > 0)
            {
                int studentId = Convert.ToInt32(dgvStudents.SelectedRows[0].Cells["Id"].Value);
                StudentEditForm form = new StudentEditForm(studentId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadStudents();
                }
            }
            else
            {
                MessageBox.Show("Выберите студента для редактирования!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDeleteStudent_Click(object? sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить этого студента?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        int studentId = Convert.ToInt32(dgvStudents.SelectedRows[0].Cells["Id"].Value);

                        using (var connection = DatabaseContext.GetConnection())
                        {
                            connection.Open();
                            string query = "DELETE FROM Students WHERE Id = @Id";
                            using (var command = new SQLiteCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Id", studentId);
                                command.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Студент успешно удален!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadStudents();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите студента для удаления!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAddGroup_Click(object? sender, EventArgs e)
        {
            GroupEditForm form = new GroupEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadGroups();
            }
        }

        private void BtnEditGroup_Click(object? sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count > 0)
            {
                int groupId = Convert.ToInt32(dgvGroups.SelectedRows[0].Cells["Id"].Value);
                GroupEditForm form = new GroupEditForm(groupId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadGroups();
                }
            }
            else
            {
                MessageBox.Show("Выберите группу для редактирования!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDeleteGroup_Click(object? sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту группу?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        int groupId = Convert.ToInt32(dgvGroups.SelectedRows[0].Cells["Id"].Value);

                        using (var connection = DatabaseContext.GetConnection())
                        {
                            connection.Open();
                            string query = "DELETE FROM Groups WHERE Id = @Id";
                            using (var command = new SQLiteCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Id", groupId);
                                command.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Группа успешно удалена!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadGroups();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите группу для удаления!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAddRole_Click(object? sender, EventArgs e)
        {
            RoleEditForm form = new RoleEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadRoles();
            }
        }

        private void BtnEditRole_Click(object? sender, EventArgs e)
        {
            if (dgvRoles.SelectedRows.Count > 0)
            {
                int roleId = Convert.ToInt32(dgvRoles.SelectedRows[0].Cells["Id"].Value);
                RoleEditForm form = new RoleEditForm(roleId);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadRoles();
                }
            }
            else
            {
                MessageBox.Show("Выберите роль для редактирования!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDeleteRole_Click(object? sender, EventArgs e)
        {
            if (dgvRoles.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту роль?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        int roleId = Convert.ToInt32(dgvRoles.SelectedRows[0].Cells["Id"].Value);

                        using (var connection = DatabaseContext.GetConnection())
                        {
                            connection.Open();
                            string query = "DELETE FROM Roles WHERE Id = @Id";
                            using (var command = new SQLiteCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Id", roleId);
                                command.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show("Роль успешно удалена!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRoles();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите роль для удаления!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
