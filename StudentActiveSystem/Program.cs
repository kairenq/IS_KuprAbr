using System;
using System.Windows.Forms;
using StudentActiveSystem.Data;
using StudentActiveSystem.Forms;

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
            var loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Запуск главной формы после успешного входа
                Application.Run(new MainForm(loginForm.UserId, loginForm.LoggedInFullName, loginForm.IsAdmin));
            }
        }
    }
}
