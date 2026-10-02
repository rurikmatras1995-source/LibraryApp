using System;
using System.Linq;
using System.Windows;

namespace LibraryApp
{
    public partial class LoginWindow : Window
    {
        public static users CurrentUser { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text.Trim();
            string password = pbPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                tbError.Text = "Введите логин и пароль!";
                tbError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                using (var db = new БиблиотекаEntities())
                {
                    var user = db.users.FirstOrDefault(u => u.login == login && u.password_hash == password);
                    if (user == null)
                    {
                        tbError.Text = "Неверный логин или пароль!";
                        tbError.Visibility = Visibility.Visible;
                        return;
                    }

                    CurrentUser = user;
                    MainWindow main = new MainWindow();
                    main.Show();
                    Close();
                }
            }
            catch (Exception ex)
            {
                tbError.Text = $"Ошибка: {ex.Message}";
                tbError.Visibility = Visibility.Visible;
            }
        }
    }
}