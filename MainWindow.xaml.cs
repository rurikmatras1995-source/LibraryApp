using System.Windows;

namespace LibraryApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenBooks_Click(object sender, RoutedEventArgs e)
        {
            BooksWindow booksWindow = new BooksWindow();
            booksWindow.Show();
            this.Close();
        }
        // Добавьте кнопку выхода в MainWindow.xaml или этот обработчик:
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow.CurrentUser = null;
            LoginWindow login = new LoginWindow();
            login.Show();
            Close();
        }
    }
}