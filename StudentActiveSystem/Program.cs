using System;
using System.Windows.Forms;
using StudentActiveSystem.Data;

namespace StudentActiveSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Инициализация базы данных
            DatabaseInitializer.Initialize();

            // Запуск формы логина
            Application.Run(new Forms.LoginForm());
        }
    }
}
