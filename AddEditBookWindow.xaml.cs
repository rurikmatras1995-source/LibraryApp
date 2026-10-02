using System;
using System.Windows;

namespace LibraryApp
{
    public partial class AddEditBookWindow : Window
    {
        private books _editingBook;
        private bool _isEditMode = false;

        public AddEditBookWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            tbTitle.Text = "Добавление книги";
        }

        public AddEditBookWindow(books book)
        {
            InitializeComponent();
            _isEditMode = true;
            _editingBook = book;
            tbTitle.Text = "Редактирование книги";
            LoadBookData();
        }

        private void LoadBookData()
        {
            tbTitleBook.Text = _editingBook.book_title;
            tbAuthor.Text = _editingBook.author;
            tbGenre.Text = _editingBook.genre;
            tbYear.Text = _editingBook.year_of_publication.ToString();
            tbPublisher.Text = _editingBook.publisher;
            tbPages.Text = _editingBook.page_count.ToString();
            tbCopiesTotal.Text = _editingBook.copies_total.ToString();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbTitleBook.Text))
            {
                MessageBox.Show("Введите название книги!", "Внимание");
                return;
            }

            if (string.IsNullOrWhiteSpace(tbAuthor.Text))
            {
                MessageBox.Show("Введите автора!", "Внимание");
                return;
            }

            int year = 0;
            int pages = 0;
            int copies = 1;

            if (!string.IsNullOrWhiteSpace(tbYear.Text))
                int.TryParse(tbYear.Text, out year);
            if (!string.IsNullOrWhiteSpace(tbPages.Text))
                int.TryParse(tbPages.Text, out pages);
            if (!string.IsNullOrWhiteSpace(tbCopiesTotal.Text))
                int.TryParse(tbCopiesTotal.Text, out copies);

            try
            {
                using (var db = new БиблиотекаEntities())
                {
                    if (_isEditMode)
                    {
                        var book = await db.books.FindAsync(_editingBook.book_id);
                        if (book != null)
                        {
                            book.book_title = tbTitleBook.Text.Trim();
                            book.author = tbAuthor.Text.Trim();
                            book.genre = tbGenre.Text.Trim();
                            book.year_of_publication = year;
                            book.publisher = tbPublisher.Text.Trim();
                            book.page_count = pages;
                            book.copies_total = copies;
                            book.copies_available = copies;
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var newBook = new books
                        {
                            book_title = tbTitleBook.Text.Trim(),
                            author = tbAuthor.Text.Trim(),
                            genre = tbGenre.Text.Trim(),
                            year_of_publication = year,
                            publisher = tbPublisher.Text.Trim(),
                            page_count = pages,
                            copies_total = copies,
                            copies_available = copies,
                            is_active = true
                        };
                        db.books.Add(newBook);
                        await db.SaveChangesAsync();
                    }
                }

                MessageBox.Show("Данные сохранены!", "Успешно");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}