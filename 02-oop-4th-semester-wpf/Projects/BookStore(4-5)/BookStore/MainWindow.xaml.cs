using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookStore
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Book> books = new ObservableCollection<Book>();
        private ObservableCollection<Book> filteredBooks = new ObservableCollection<Book>();
        private ObservableCollection<CartItem> cart = new ObservableCollection<CartItem>();

        private bool isAdminMode = false;
        private Book selectedBookForEdit = null!;
        private Book _selectedBook = null!;
        private Book _currentDetailsBook = null!;

        public ICommand AddToCartCommand { get; private set; }
        public ICommand RemoveFromCartCommand { get; private set; }
        public ICommand CheckoutCommand { get; private set; }
        public ICommand ApplyFilterCommand { get; private set; }
        public ICommand ShowDetailsCommand { get; private set; }
        public ICommand CloseDetailsCommand { get; private set; }
        public ICommand AddToCartFromDetailsCommand { get; private set; }

        public Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value;
                OnPropertyChanged(nameof(SelectedBook));
            }
        }

        public Book CurrentDetailsBook
        {
            get { return _currentDetailsBook; }
            set
            {
                _currentDetailsBook = value;
                OnPropertyChanged(nameof(CurrentDetailsBook));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeCommands();
            LoadSampleBooks();
            DataContext = this;
        }

        private void InitializeCommands()
        {
            AddToCartCommand = new RelayCommand(ExecuteAddToCart, CanAddToCart);
            RemoveFromCartCommand = new RelayCommand(ExecuteRemoveFromCart);
            CheckoutCommand = new RelayCommand(ExecuteCheckout, CanCheckout);
            ApplyFilterCommand = new RelayCommand(ExecuteApplyFilter);
            ShowDetailsCommand = new RelayCommand(ExecuteShowDetails);
            CloseDetailsCommand = new RelayCommand(ExecuteCloseDetails);
            AddToCartFromDetailsCommand = new RelayCommand(ExecuteAddToCartFromDetails, CanAddToCartFromDetails);
        }

        private bool CanAddToCart(object parameter)
        {
            return parameter is Book book && book != null;
        }

        private void ExecuteAddToCart(object parameter)
        {
            if (parameter is Book book)
            {
                AddToCart(book);
            }
        }

        private void ExecuteRemoveFromCart(object parameter)
        {
            if (parameter is int bookId)
            {
                var item = cart.FirstOrDefault(i => i.Book.Id == bookId);
                if (item != null)
                {
                    if (item.Quantity > 1)
                    {
                        item.Quantity--;
                    }
                    else
                    {
                        cart.Remove(item);
                    }
                    CartListView.Items.Refresh();
                    UpdateTotalAmount();
                }
            }
        }

        private bool CanCheckout(object parameter)
        {
            return cart.Count > 0;
        }

        private async void ExecuteCheckout(object parameter)
        {
            var order = new Order
            {
                Id = new Random().Next(1000, 9999),
                OrderDate = DateTime.Now,
                Items = cart.ToList(),
                TotalAmount = cart.Sum(item => item.Book.Price * item.Quantity)
            };

            try
            {
                string fileName = $"order_{order.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                string json = JsonSerializer.Serialize(order, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(fileName, json);

                MessageBox.Show($"Заказ №{order.Id} оформлен!\nСохранен в файл: {fileName}");

                cart.Clear();
                CartListView.Items.Refresh();
                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ExecuteApplyFilter(object parameter)
        {
            FilterBooks();
        }

        private void ExecuteShowDetails(object parameter)
        {
            if (parameter is int bookId)
            {
                CurrentDetailsBook = books.FirstOrDefault(b => b.Id == bookId)!;
                if (CurrentDetailsBook != null)
                {
                    DetailsTitle.Text = CurrentDetailsBook.Title;
                    DetailsAuthor.Text = CurrentDetailsBook.Author;
                    DetailsCategory.Text = CurrentDetailsBook.Category;
                    DetailsYear.Text = CurrentDetailsBook.Year.ToString();
                    DetailsPages.Text = CurrentDetailsBook.Pages.ToString();
                    DetailsPrice.Text = $"{CurrentDetailsBook.Price} ₽";
                    DetailsDescription.Text = CurrentDetailsBook.Description;

                    DetailsPopup.Visibility = Visibility.Visible;
                }
            }
        }

        private void ExecuteCloseDetails(object parameter)
        {
            DetailsPopup.Visibility = Visibility.Collapsed;
        }

        private bool CanAddToCartFromDetails(object parameter)
        {
            return CurrentDetailsBook != null;
        }

        private void ExecuteAddToCartFromDetails(object parameter)
        {
            if (CurrentDetailsBook != null)
            {
                AddToCart(CurrentDetailsBook);
                DetailsPopup.Visibility = Visibility.Collapsed;
            }
        }

        private void ChangeLanguage(string language)
        {
            Application.Current.Resources.MergedDictionaries.Clear();

            ResourceDictionary dict = new ResourceDictionary();

            if (language == "en")
                dict.Source = new Uri("Strings.en.xaml", UriKind.Relative);
            else
                dict.Source = new Uri("Strings.ru.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Add(dict);

            if (SearchBox != null)
            {
                string searchPlaceholder = (string)Application.Current.Resources["SearchPlaceholder"];
                if (string.IsNullOrWhiteSpace(SearchBox.Text) ||
                    SearchBox.Text == "Search books..." ||
                    SearchBox.Text == "Поиск книг...")
                {
                    SearchBox.Text = searchPlaceholder;
                }
            }
        }

        private void LoadSampleBooks()
        {
            books.Add(new Book
            {
                Id = 1,
                Title = "Белый олеандр",
                Author = "Джанет Фитч",
                Category = "Романы",
                Year = 1999,
                Pages = 468,
                Price = 450,
                Description = "Пронзительная история о взрослении, любви и поиске себя.",
                ImagePath = "1.jpg"
            });

            books.Add(new Book
            {
                Id = 2,
                Title = "Под стеклянным колпаком",
                Author = "Сильвия Плат",
                Category = "Драма",
                Year = 1963,
                Pages = 320,
                Price = 380,
                Description = "Единственный роман знаменитой поэтессы, ставший классикой.",
                ImagePath = "2.jpg"
            });

            books.Add(new Book
            {
                Id = 3,
                Title = "Война и мир",
                Author = "Лев Толстой",
                Category = "Классика",
                Year = 1869,
                Pages = 1300,
                Price = 890,
                Description = "Роман-эпопея о жизни русского общества в эпоху наполеоновских войск.",
                ImagePath = "4.jpg"
            });

            books.Add(new Book
            {
                Id = 4,
                Title = "Преступление и наказание",
                Author = "Фёдор Достоевский",
                Category = "Классика",
                Year = 1866,
                Pages = 672,
                Price = 550,
                Description = "Роман о студенте Раскольникове и его теории о праве сильной личности.",
                ImagePath = "3.jpg"
            });

            FilterBooks();
            BooksListView.ItemsSource = filteredBooks;
            CartListView.ItemsSource = cart;
        }

        private void FilterBooks()
        {
            if (CategoryFilter == null || SortFilter == null || SearchBox == null)
                return;

            var filtered = books.AsEnumerable();

            string searchText = SearchBox.Text;
            string searchPlaceholder = (string)Application.Current.Resources["SearchPlaceholder"];

            if (!string.IsNullOrWhiteSpace(searchText) && searchText != searchPlaceholder)
            {
                filtered = filtered.Where(b =>
                    !string.IsNullOrEmpty(b.Title) &&
                    b.Title.ToLower().Contains(searchText.ToLower()));
            }

            if (CategoryFilter.SelectedIndex > 0)
            {
                var selectedCategory = (CategoryFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (!string.IsNullOrEmpty(selectedCategory))
                {
                    filtered = filtered.Where(b => b.Category == selectedCategory);
                }
            }

            if (SortFilter.SelectedIndex == 1)
                filtered = filtered.OrderBy(b => b.Price);
            else if (SortFilter.SelectedIndex == 2)
                filtered = filtered.OrderByDescending(b => b.Price);
            else if (SortFilter.SelectedIndex == 3)
                filtered = filtered.OrderBy(b => b.Title);

            filteredBooks.Clear();
            foreach (var book in filtered)
                filteredBooks.Add(book);

            BooksListView.Items.Refresh();
        }

        private void UpdateTotalAmount()
        {
            decimal total = cart.Sum(item => item.Book.Price * item.Quantity);
            TotalAmount.Text = $"{total} ₽";
        }

        private void AddToCart(Book book)
        {
            var existingItem = cart.FirstOrDefault(item => item.Book.Id == book.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem { Book = book, Quantity = 1 });
            }

            CartListView.Items.Refresh();
            UpdateTotalAmount();
            MessageBox.Show($"Книга '{book.Title}' добавлена в корзину", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTotalAmount();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterBooks();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string searchPlaceholder = (string)Application.Current.Resources["SearchPlaceholder"];
            if (SearchBox.Text == searchPlaceholder)
            {
                SearchBox.Text = "";
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = (string)Application.Current.Resources["SearchPlaceholder"];
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterBooks();
            }
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void BooksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksListView.SelectedItem is Book selectedBook)
            {
                SelectedBook = selectedBook;
            }
        }

        private void UserModeBtn_Click(object sender, RoutedEventArgs e)
        {
            isAdminMode = false;
            UserModeBtn.Style = (Style)FindResource("WhiteButtonStyle");
            AdminModeBtn.Style = (Style)FindResource("GreenButtonStyle");
            AdminPanel.Visibility = Visibility.Collapsed;
        }

        private void AdminModeBtn_Click(object sender, RoutedEventArgs e)
        {
            isAdminMode = true;
            AdminModeBtn.Style = (Style)FindResource("WhiteButtonStyle");
            UserModeBtn.Style = (Style)FindResource("GreenButtonStyle");
            AdminPanel.Visibility = Visibility.Visible;
        }

        private void RussianLangBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeLanguage("ru");
            RussianLangBtn.Style = (Style)FindResource("WhiteButtonStyle");
            EnglishLangBtn.Style = (Style)FindResource("GreenButtonStyle");
        }

        private void EnglishLangBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeLanguage("en");
            EnglishLangBtn.Style = (Style)FindResource("WhiteButtonStyle");
            RussianLangBtn.Style = (Style)FindResource("GreenButtonStyle");
        }

        private void AddBookBtn_Click(object sender, RoutedEventArgs e)
        {
            selectedBookForEdit = null;
            DialogTitle.Text = (string)Application.Current.Resources["AddTitle"];
            ClearDialogFields();
            BookDialog.Visibility = Visibility.Visible;
        }

        private void EditBookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBook == null)
            {
                MessageBox.Show("Выберите книгу для редактирования!");
                return;
            }

            selectedBookForEdit = SelectedBook;
            DialogTitle.Text = (string)Application.Current.Resources["EditTitle"];

            DialogTitleInput.Text = selectedBookForEdit.Title;
            DialogAuthorInput.Text = selectedBookForEdit.Author;

            for (int i = 0; i < DialogCategoryInput.Items.Count; i++)
            {
                var item = DialogCategoryInput.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == selectedBookForEdit.Category)
                {
                    DialogCategoryInput.SelectedIndex = i;
                    break;
                }
            }

            DialogYearInput.Text = selectedBookForEdit.Year.ToString();
            DialogPagesInput.Text = selectedBookForEdit.Pages.ToString();
            DialogPriceInput.Text = selectedBookForEdit.Price.ToString();

            BookDialog.Visibility = Visibility.Visible;
        }

        private void DeleteBookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBook == null)
            {
                MessageBox.Show("Выберите книгу для удаления!");
                return;
            }

            var bookToDelete = SelectedBook;

            var result = MessageBox.Show(
                $"Удалить книгу '{bookToDelete.Title}'?",
                "Подтверждение",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                books.Remove(bookToDelete);

                var cartItem = cart.FirstOrDefault(item => item.Book.Id == bookToDelete.Id);
                if (cartItem != null)
                    cart.Remove(cartItem);

                FilterBooks();
                CartListView.Items.Refresh();
                UpdateTotalAmount();
                SelectedBook = null;
            }
        }

        private void ClearDialogFields()
        {
            DialogTitleInput.Text = "";
            DialogAuthorInput.Text = "";
            DialogCategoryInput.SelectedIndex = 0;
            DialogYearInput.Text = "";
            DialogPagesInput.Text = "";
            DialogPriceInput.Text = "";
        }

        private void DialogSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DialogTitleInput.Text) ||
                    string.IsNullOrWhiteSpace(DialogAuthorInput.Text) ||
                    string.IsNullOrWhiteSpace(DialogYearInput.Text) ||
                    string.IsNullOrWhiteSpace(DialogPagesInput.Text) ||
                    string.IsNullOrWhiteSpace(DialogPriceInput.Text))
                {
                    MessageBox.Show("Заполните все поля!");
                    return;
                }

                var category = (DialogCategoryInput.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (selectedBookForEdit == null)
                {
                    // Добавление новой книги
                    var newBook = new Book
                    {
                        Id = books.Any() ? books.Max(b => b.Id) + 1 : 1,
                        Title = DialogTitleInput.Text,
                        Author = DialogAuthorInput.Text,
                        Category = category,
                        Year = int.Parse(DialogYearInput.Text),
                        Pages = int.Parse(DialogPagesInput.Text),
                        Price = decimal.Parse(DialogPriceInput.Text),
                        Description = $"Книга {DialogTitleInput.Text}"
                    };
                    books.Add(newBook);
                }
                else
                {
                    // Редактирование существующей книги
                    selectedBookForEdit.Title = DialogTitleInput.Text;
                    selectedBookForEdit.Author = DialogAuthorInput.Text;
                    selectedBookForEdit.Category = category;
                    selectedBookForEdit.Year = int.Parse(DialogYearInput.Text);
                    selectedBookForEdit.Pages = int.Parse(DialogPagesInput.Text);
                    selectedBookForEdit.Price = decimal.Parse(DialogPriceInput.Text);
                }

                FilterBooks();
                BookDialog.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void DialogCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            BookDialog.Visibility = Visibility.Collapsed;
        }
    }
}