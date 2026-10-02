using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LibraryApp
{
    public partial class BooksWindow : Window
    {
        public BooksWindow()
        {
            InitializeComponent();
            LoadData();
            LoadFilters();
            SetPermissionsByRole();
        }

        private async void LoadData()
        {
            try
            {
                using (var db = new БиблиотекаEntities())
                {
                    var query = db.books.AsQueryable();

                    // Поиск по названию или автору
                    if (!string.IsNullOrWhiteSpace(tbSearch.Text))
                    { 
                        string search = tbSearch.Text.ToLower();
                        query = query.Where(b => b.book_title.ToLower().Contains(search) ||
                                                  b.author.ToLower().Contains(search));
                    }

                    // Фильтр по жанру
                    if (cbGenreFilter.SelectedItem != null && cbGenreFilter.SelectedIndex > 0)
                    {
                        string selected = cbGenreFilter.SelectedItem.ToString();
                        query = query.Where(b => b.genre == selected);
                    }

                    // Фильтр по году
                    if (cbYearFilter.SelectedItem != null && cbYearFilter.SelectedIndex > 0)
                    {
                        if (int.TryParse(cbYearFilter.SelectedItem.ToString(), out int year))
                            query = query.Where(b => b.year_of_publication == year);
                    }

                    dgBooks.ItemsSource = await query.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void LoadFilters()
        {
            using (var db = new БиблиотекаEntities())
            {
                // Жанры
                var genres = db.books.Select(b => b.genre).Distinct().ToList();
                genres.Insert(0, "Все жанры");
                cbGenreFilter.ItemsSource = genres;
                cbGenreFilter.SelectedIndex = 0;

                // Годы
                var years = db.books.Select(b => b.year_of_publication).Distinct()
                                    .OrderByDescending(y => y).ToList();
                var yearList = years.Select(y => y.ToString()).ToList();
                yearList.Insert(0, "Все годы");
                cbYearFilter.ItemsSource = yearList;
                cbYearFilter.SelectedIndex = 0;
            }
        }

        private void SetPermissionsByRole()
        {
            var user = LoginWindow.CurrentUser;
            if (user == null) return;

            // Admin — все кнопки, Reader — только просмотр
            if (user.role != "Admin")
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();
        private void cbGenreFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadData();
        private void cbYearFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadData();

        private void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = "";
            cbGenreFilter.SelectedIndex = 0;
            cbYearFilter.SelectedIndex = 0;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddEditBookWindow addWindow = new AddEditBookWindow();
            addWindow.ShowDialog();
            LoadData();
            LoadFilters();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgBooks.SelectedItem == null)
            {
                MessageBox.Show("Выберите книгу для редактирования!", "Внимание");
                return;
            }

            var selected = dgBooks.SelectedItem as books;
            if (selected != null)
            {
                AddEditBookWindow editWindow = new AddEditBookWindow(selected);
                editWindow.ShowDialog();
                LoadData();
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgBooks.SelectedItem == null)
            {
                MessageBox.Show("Выберите книгу для удаления!", "Внимание");
                return;
            }

            var selected = dgBooks.SelectedItem as books;
            if (selected == null) return;

            var result = MessageBox.Show($"Удалить книгу '{selected.book_title}'?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new БиблиотекаEntities())
                    {
                        var item = await db.books.FindAsync(selected.book_id);
                        if (item != null)
                        {
                            db.books.Remove(item);
                            await db.SaveChangesAsync();
                        }
                    }
                    LoadData();
                    MessageBox.Show("Запись удалена!", "Успешно");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}