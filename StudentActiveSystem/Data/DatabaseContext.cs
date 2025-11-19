using System;
using System.Data.SQLite;
using System.IO;

namespace StudentActiveSystem.Data
{
    public static class DatabaseContext
    {
        private static string _connectionString = string.Empty;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "ElectionCommission.db");
                    _connectionString = $"Data Source={dbPath};Version=3;";
                }
                return _connectionString;
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}
