using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace StudentActiveSystem.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "AdmissionCommittee.db");
            string directory = Path.GetDirectoryName(dbPath)!;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            bool isNewDatabase = !File.Exists(dbPath);

            if (isNewDatabase)
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var connection = DatabaseContext.GetConnection())
            {
                connection.Open();

                // Создание таблицы пользователей
                string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        FullName TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        IsAdmin INTEGER NOT NULL DEFAULT 0
                    )";

                // Создание таблицы специальностей
                string createGroupsTable = @"
                    CREATE TABLE IF NOT EXISTS Groups (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE,
                        Faculty TEXT NOT NULL,
                        Course INTEGER NOT NULL
                    )";

                // Создание таблицы статусов
                string createRolesTable = @"
                    CREATE TABLE IF NOT EXISTS Roles (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE,
                        Description TEXT NOT NULL
                    )";

                // Создание таблицы абитуриентов
                string createStudentsTable = @"
                    CREATE TABLE IF NOT EXISTS Students (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FullName TEXT NOT NULL,
                        GroupId INTEGER NOT NULL,
                        RoleId INTEGER,
                        Email TEXT,
                        Phone TEXT,
                        FOREIGN KEY (GroupId) REFERENCES Groups(Id),
                        FOREIGN KEY (RoleId) REFERENCES Roles(Id)
                    )";

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = createUsersTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createGroupsTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createRolesTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createStudentsTable;
                    command.ExecuteNonQuery();
                }

                // Если база данных новая, добавим тестовые данные
                if (isNewDatabase)
                {
                    SeedData(connection);
                }
            }
        }

        private static void SeedData(SQLiteConnection connection)
        {
            using (var command = new SQLiteCommand(connection))
            {
                // Добавление администратора по умолчанию (admin/admin)
                string adminPasswordHash = HashPassword("admin");
                command.CommandText = @"
                    INSERT INTO Users (Username, PasswordHash, FullName, CreatedAt, IsAdmin)
                    VALUES ('admin', @PasswordHash, 'Администратор', @CreatedAt, 1)";
                command.Parameters.AddWithValue("@PasswordHash", adminPasswordHash);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.ExecuteNonQuery();
                command.Parameters.Clear();

                // Добавление статусов заявлений
                var roles = new[]
                {
                    ("Подано", "Заявление подано и ожидает рассмотрения"),
                    ("На рассмотрении", "Заявление находится на рассмотрении комиссии"),
                    ("Принят", "Абитуриент зачислен в учебное заведение"),
                    ("В резерве", "Абитуриент находится в резервном списке"),
                    ("Отклонено", "Заявление отклонено")
                };

                foreach (var role in roles)
                {
                    command.CommandText = @"
                        INSERT INTO Roles (Name, Description)
                        VALUES (@Name, @Description)";
                    command.Parameters.AddWithValue("@Name", role.Item1);
                    command.Parameters.AddWithValue("@Description", role.Item2);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }

                // Добавление специальностей БППК
                var groups = new[]
                {
                    ("09.02.07", "Информационные системы и программирование", 1),
                    ("38.02.01", "Экономика и бухгалтерский учет", 2),
                    ("40.02.01", "Право и организация социального обеспечения", 3),
                    ("43.02.10", "Туризм", 4),
                    ("46.02.01", "Документационное обеспечение управления", 5),
                    ("38.02.04", "Коммерция", 6)
                };

                foreach (var group in groups)
                {
                    command.CommandText = @"
                        INSERT INTO Groups (Name, Faculty, Course)
                        VALUES (@Name, @Faculty, @Course)";
                    command.Parameters.AddWithValue("@Name", group.Item1);
                    command.Parameters.AddWithValue("@Faculty", group.Item2);
                    command.Parameters.AddWithValue("@Course", group.Item3);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
