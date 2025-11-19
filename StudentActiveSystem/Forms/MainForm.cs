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
        private Panel panelHeader;

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
            this.Text = "БППК - Приемная комиссия";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 245);

            // Верхняя панель
            panelHeader = new Panel
            {
                Size = new Size(1200, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(25, 55, 109)
            };

            Label lblAppTitle = new Label
            {
                Text = "БППК - Приемная комиссия",
                Font = new Font("Tahoma", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            panelHeader.Controls.Add(lblAppTitle);

            // Приветствие
            lblWelcome = new Label
            {
                Text = $"Пользователь: {_fullName}",
                Font = new Font("Tahoma", 10),
                ForeColor = Color.White,
                Location = new Point(900, 20),
                AutoSize = true
            };
            panelHeader.Controls.Add(lblWelcome);

            this.Controls.Add(panelHeader);

            // Вкладки
            tabControl = new TabControl
            {
                Location = new Point(20, 75),
                Size = new Size(1145, 620),
                Font = new Font("Tahoma", 10)
            };

            // Вкладка "Абитуриенты"
            tabStudents = new TabPage("Абитуриенты");
            tabStudents.BackColor = Color.White;
            InitializeStudentsTab();

            // Вкладка "Специальности"
            tabGroups = new TabPage("Специальности");
            tabGroups.BackColor = Color.White;
            InitializeGroupsTab();

            // Вкладка "Статусы"
            tabRoles = new TabPage("Статусы");
            tabRoles.BackColor = Color.White;
            InitializeRolesTab();

            tabControl.TabPages.Add(tabStudents);
            tabControl.TabPages.Add(tabGroups);
            tabControl.TabPages.Add(tabRoles);

            this.Controls.Add(tabControl);
        }

        private void InitializeStudentsTab()
        {
            // DataGridView для абитуриентов
            dgvStudents = new DataGridView
            {
                Location = new Point(10, 15),
                Size = new Size(1110, 500),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                Font = new Font("Tahoma", 9)
            };

            // Кнопки управления
            btnAddStudent = CreateButton("Добавить", 10, 530, Color.FromArgb(25, 55, 109));
            btnAddStudent.Click += BtnAddStudent_Click;

            btnEditStudent = CreateButton("Изменить", 230, 530, Color.FromArgb(70, 100, 150));
            btnEditStudent.Click += BtnEditStudent_Click;

            btnDeleteStudent = CreateButton("Удалить", 450, 530, Color.FromArgb(150, 60, 60));
            btnDeleteStudent.Click += BtnDeleteStudent_Click;

            btnRefreshStudents = CreateButton("Обновить", 670, 530, Color.FromArgb(100, 100, 100));
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
                Location = new Point(10, 15),
                Size = new Size(1110, 500),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                Font = new Font("Tahoma", 9)
            };

            btnAddGroup = CreateButton("Добавить", 10, 530, Color.FromArgb(25, 55, 109));
            btnAddGroup.Click += BtnAddGroup_Click;

            btnEditGroup = CreateButton("Изменить", 230, 530, Color.FromArgb(70, 100, 150));
            btnEditGroup.Click += BtnEditGroup_Click;

            btnDeleteGroup = CreateButton("Удалить", 450, 530, Color.FromArgb(150, 60, 60));
            btnDeleteGroup.Click += BtnDeleteGroup_Click;

            btnRefreshGroups = CreateButton("Обновить", 670, 530, Color.FromArgb(100, 100, 100));
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
                Location = new Point(10, 15),
                Size = new Size(1110, 500),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                Font = new Font("Tahoma", 9)
            };

            btnAddRole = CreateButton("Добавить", 10, 530, Color.FromArgb(25, 55, 109));
            btnAddRole.Click += BtnAddRole_Click;

            btnEditRole = CreateButton("Изменить", 230, 530, Color.FromArgb(70, 100, 150));
            btnEditRole.Click += BtnEditRole_Click;

            btnDeleteRole = CreateButton("Удалить", 450, 530, Color.FromArgb(150, 60, 60));
            btnDeleteRole.Click += BtnDeleteRole_Click;

            btnRefreshRoles = CreateButton("Обновить", 670, 530, Color.FromArgb(100, 100, 100));
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
                Size = new Size(210, 42),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 10, FontStyle.Bold),
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
                        SELECT s.Id, s.FullName as 'ФИО',
                               g.Name || ' - ' || g.Faculty as 'Специальность',
                               IFNULL(r.Name, 'Не указан') as 'Статус',
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
                MessageBox.Show($"Ошибка загрузки абитуриентов: {ex.Message}", "Ошибка",
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
                        SELECT Id, Name as 'Код', Faculty as 'Название специальности', Course as 'Порядок'
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
                MessageBox.Show($"Ошибка загрузки специальностей: {ex.Message}", "Ошибка",
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
                        SELECT Id, Name as 'Статус', Description as 'Описание'
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
                MessageBox.Show($"Ошибка загрузки статусов: {ex.Message}", "Ошибка",
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
                MessageBox.Show("Выберите абитуриента для редактирования!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDeleteStudent_Click(object? sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Удалить выбранного абитуриента?",
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

                        MessageBox.Show("Запись удалена!", "Успех",
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
                MessageBox.Show("Выберите абитуриента для удаления!", "Предупреждение",
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
                MessageBox.Show("Выберите специальность для редактирования!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDeleteGroup_Click(object? sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Удалить выбранную специальность?",
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

                        MessageBox.Show("Специальность удалена!", "Успех",
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
                MessageBox.Show("Выберите специальность для удаления!", "Предупреждение",
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
                MessageBox.Show("Выберите статус для редактирования!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDeleteRole_Click(object? sender, EventArgs e)
        {
            if (dgvRoles.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Удалить выбранный статус?",
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

                        MessageBox.Show("Статус успешно удален!", "Успех",
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
                MessageBox.Show("Выберите статус для удаления!", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
