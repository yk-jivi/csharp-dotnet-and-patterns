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
using System.Windows.Media;

namespace BookStore
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Book> books = new ObservableCollection<Book>();
        private ObservableCollection<Book> filteredBooks = new ObservableCollection<Book>();
        private ObservableCollection<CartItem> cart = new ObservableCollection<CartItem>();
        private CommandHistory _commandHistory = new CommandHistory();
        private User _currentUser = new User();

        private bool isAdminMode = false;
        private Book selectedBookForEdit = null!;
        private Book _selectedBook = null!;
        private Book _currentDetailsBook = null!;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

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
            LoadSampleBooks();
            DataContext = this;
            LoadUserData();
            CartListView.ItemsSource = cart;
            UpdateTotalAmount();
        }

        private void LoadUserData()
        {
            CurrentUser = new User
            {
                Username = "Гость",
                Email = "guest@bookstore.com",
                Phone = "+7 (999) 123-45-67",
                Language = "ru",
                Theme = "DefaultTheme",
                Role = "User"
            };
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
                Price = 13.18m,
                Description = "Пронзительная история о взрослении, любви и поиске себя.",
                ImagePath = "1.jpg",
                Rating = 5,
                RatingCount = 128
            });

            books.Add(new Book
            {
                Id = 2,
                Title = "Под стеклянным колпаком",
                Author = "Сильвия Плат",
                Category = "Драма",
                Year = 1963,
                Pages = 320,
                Price = 18.13m,
                Description = "Единственный роман знаменитой поэтессы, ставший классикой.",
                ImagePath = "2.jpg",
                Rating = 4,        
                RatingCount = 95
            });

            books.Add(new Book
            {
                Id = 3,
                Title = "Война и мир",
                Author = "Лев Толстой",
                Category = "Классика",
                Year = 1869,
                Pages = 1300,
                Price = 18.18m,
                Description = "Роман-эпопея о жизни русского общества в эпоху наполеоновских войн.",
                ImagePath = "4.jpg",
                Rating = 5,
                RatingCount = 256  
            });

            books.Add(new Book
            {
                Id = 4,
                Title = "Преступление и наказание",
                Author = "Фёдор Достоевский",
                Category = "Классика",
                Year = 1866,
                Pages = 672,
                Price = 13.13m,
                Description = "Роман о студенте Раскольникове и его теории о праве сильной личности.",
                ImagePath = "3.jpg",
                Rating = 5,
                RatingCount = 312
            });

            FilterBooks();
            BooksListView.ItemsSource = filteredBooks;
            CartListView.ItemsSource = cart;
            UpdateTotalAmount();
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

            if (TileViewPanel.Visibility == Visibility.Visible)
            {
                UpdateTileView();
            }
        }

        private void UpdateTileView()
        {
            BooksWrapPanel.Children.Clear();
            foreach (var book in filteredBooks)
            {
                var card = new BookCardControl();
                card.DataContext = book;
                card.AddToCartClicked += (s, b) => AddToCart(b);
                BooksWrapPanel.Children.Add(card);
            }
        }

        private void SwitchToListBtn_Click(object sender, RoutedEventArgs e)
        {
            ListViewPanel.Visibility = Visibility.Visible;
            TileViewPanel.Visibility = Visibility.Collapsed;
            SwitchToListBtn.Style = (Style)FindResource("GreenButtonStyle");
            SwitchToTileBtn.Style = (Style)FindResource("WhiteButtonStyle");
        }

        private void SwitchToTileBtn_Click(object sender, RoutedEventArgs e)
        {
            ListViewPanel.Visibility = Visibility.Collapsed;
            TileViewPanel.Visibility = Visibility.Visible;
            SwitchToTileBtn.Style = (Style)FindResource("GreenButtonStyle");
            SwitchToListBtn.Style = (Style)FindResource("WhiteButtonStyle");
            UpdateTileView();
        }

        private void UpdateTotalAmount()
        {
            decimal total = cart.Sum(item => item.Book.Price * item.Quantity);
            TotalAmount.Text = $"{total} BYN";
        }

        private void AddToCart(Book book)
        {
            var existingItem = cart.FirstOrDefault(item => item.Book.Id == book.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += book.SelectedQuantity;
            }
            else
            {
                cart.Add(new CartItem { Book = book, Quantity = book.SelectedQuantity });
            }

            CartListView.Items.Refresh();
            UpdateTotalAmount();

            MessageBox.Show($"Книга '{book.Title}' добавлена в корзину в количестве {book.SelectedQuantity} шт.",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void Checkout()
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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

        public void ChangeLanguage(string language)
        {
            Application.Current.Resources.MergedDictionaries.Clear();

            ResourceDictionary dict = new ResourceDictionary();

            if (language == "en")
                dict.Source = new Uri("Strings.en.xaml", UriKind.Relative);
            else
                dict.Source = new Uri("Strings.ru.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Add(dict);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri($"{CurrentUser.Theme}.xaml", UriKind.Relative) });

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

        public void ChangeTheme(string themeName)
        {
            try
            {
                var themeDict = new ResourceDictionary();
                themeDict.Source = new Uri($"{themeName}.xaml", UriKind.Relative);

                var dictionariesToRemove = new List<ResourceDictionary>();
                foreach (var dict in Application.Current.Resources.MergedDictionaries)
                {
                    if (dict.Source != null && (dict.Source.ToString().Contains("Theme") ||
                        dict.Source.ToString().Contains("DefaultTheme") ||
                        dict.Source.ToString().Contains("MochaTheme") ||
                        dict.Source.ToString().Contains("BlueTheme")))
                    {
                        dictionariesToRemove.Add(dict);
                    }
                }

                foreach (var dict in dictionariesToRemove)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(dict);
                }

                Application.Current.Resources.MergedDictionaries.Add(themeDict);

                this.Resources = null;
                this.Resources = Application.Current.Resources;
                this.Background = (Brush)Application.Current.Resources["BackgroundBrush"];

                if (HeaderBorder != null)
                {
                    HeaderBorder.Background = (Brush)Application.Current.Resources["HeaderBrush"];
                }

                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this && window is PersonalAccountWindow accountWindow)
                    {
                        accountWindow.UpdateTheme();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при смене темы: {ex.Message}");
            }
        }

        public void AddBookDirect(Book book)
        {
            books.Add(book);
            FilterBooks();
        }

        public void RemoveBookDirect(int bookId)
        {
            var book = books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                books.Remove(book);
                var cartItem = cart.FirstOrDefault(i => i.Book.Id == bookId);
                if (cartItem != null)
                    cart.Remove(cartItem);
                FilterBooks();
                UpdateTotalAmount();
            }
        }

        public void EditBookDirect(Book book)
        {
            var existing = books.FirstOrDefault(b => b.Id == book.Id);
            if (existing != null)
            {
                existing.Title = book.Title;
                existing.Author = book.Author;
                existing.Category = book.Category;
                existing.Year = book.Year;
                existing.Pages = book.Pages;
                existing.Price = book.Price;
                FilterBooks();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => FilterBooks();

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string searchPlaceholder = (string)Application.Current.Resources["SearchPlaceholder"];
            if (SearchBox.Text == searchPlaceholder) SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
                SearchBox.Text = (string)Application.Current.Resources["SearchPlaceholder"];
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) FilterBooks();
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e) 
        {

            FilterBooks();
        }

        private void BooksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksListView.SelectedItem is Book selectedBook)
            {
                SelectedBook = selectedBook;
            }
        }

        private void ShowDetails_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.CommandParameter is Book book)
            {
                CurrentDetailsBook = book;
                DetailsTitle.Text = CurrentDetailsBook.Title;
                DetailsAuthor.Text = CurrentDetailsBook.Author;
                DetailsCategory.Text = CurrentDetailsBook.Category;
                DetailsYear.Text = CurrentDetailsBook.Year.ToString();
                DetailsPages.Text = CurrentDetailsBook.Pages.ToString();
                DetailsPrice.Text = $"{CurrentDetailsBook.Price} BYN";
                DetailsDescription.Text = CurrentDetailsBook.Description;
                DetailsPopup.Visibility = Visibility.Visible;
            }
        }

        private void CloseDetails_Click(object sender, RoutedEventArgs e) => DetailsPopup.Visibility = Visibility.Collapsed;

        private void AddToCartFromDetails_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentDetailsBook != null)
            {
                AddToCart(CurrentDetailsBook);
                DetailsPopup.Visibility = Visibility.Collapsed;
            }
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.CommandParameter is int bookId)
            {
                var item = cart.FirstOrDefault(i => i.Book.Id == bookId);
                if (item != null)
                {
                    if (item.Quantity > 1) item.Quantity--;
                    else cart.Remove(item);
                    CartListView.Items.Refresh();
                    UpdateTotalAmount();
                }
            }
        }

        private void CheckoutBtn_Click(object sender, RoutedEventArgs e) => Checkout();

        private void UserModeBtn_Click(object sender, RoutedEventArgs e)
        {
            isAdminMode = false;
            UserModeBtn.Style = (Style)FindResource("WhiteButtonStyle");
            AdminModeBtn.Style = (Style)FindResource("GreenButtonStyle");
            AdminPanel.Visibility = Visibility.Collapsed;
            CurrentUser.Role = "User";
            CurrentUser.Username = "Гость";

            MessageBox.Show("Вы переключились в режим пользователя", "Режим",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AdminModeBtn_Click(object sender, RoutedEventArgs e)
        {
            isAdminMode = true;
            AdminModeBtn.Style = (Style)FindResource("WhiteButtonStyle");
            UserModeBtn.Style = (Style)FindResource("GreenButtonStyle");
            AdminPanel.Visibility = Visibility.Visible;
            CurrentUser.Role = "Admin";
            CurrentUser.Username = "Администратор";

            MessageBox.Show("Вы переключились в режим администратора\nДоступны функции: добавление, редактирование, удаление книг",
                "Режим", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RussianLangBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeLanguage("ru");
            RussianLangBtn.Style = (Style)FindResource("WhiteButtonStyle");
            EnglishLangBtn.Style = (Style)FindResource("GreenButtonStyle");
            CurrentUser.Language = "ru";
        }

        private void EnglishLangBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeLanguage("en");
            EnglishLangBtn.Style = (Style)FindResource("WhiteButtonStyle");
            RussianLangBtn.Style = (Style)FindResource("GreenButtonStyle");
            CurrentUser.Language = "en";
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_commandHistory.CanUndo) _commandHistory.Undo();
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_commandHistory.CanRedo) _commandHistory.Redo();
        }

        private void AccountBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var accountWindow = new PersonalAccountWindow(this);
                accountWindow.Owner = this;
                accountWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия личного кабинета: {ex.Message}");
            }
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
                _commandHistory.ExecuteCommand(new RemoveBookCommand(this, bookToDelete));
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
                    var newBook = new Book
                    {
                        Id = books.Any() ? books.Max(b => b.Id) + 1 : 1,
                        Title = DialogTitleInput.Text,
                        Author = DialogAuthorInput.Text,
                        Category = category,
                        Year = int.Parse(DialogYearInput.Text),
                        Pages = int.Parse(DialogPagesInput.Text),
                        Price = decimal.Parse(DialogPriceInput.Text),
                        Description = $"Книга {DialogTitleInput.Text}",
                        ImagePath = "default-book.jpg"
                    };

                    _commandHistory.ExecuteCommand(new AddBookCommand(this, newBook));
                }
                else
                {
                    var oldBook = new Book
                    {
                        Id = selectedBookForEdit.Id,
                        Title = selectedBookForEdit.Title,
                        Author = selectedBookForEdit.Author,
                        Category = selectedBookForEdit.Category,
                        Year = selectedBookForEdit.Year,
                        Pages = selectedBookForEdit.Pages,
                        Price = selectedBookForEdit.Price,
                        Description = selectedBookForEdit.Description,
                        ImagePath = selectedBookForEdit.ImagePath
                    };

                    var newBook = new Book
                    {
                        Id = selectedBookForEdit.Id,
                        Title = DialogTitleInput.Text,
                        Author = DialogAuthorInput.Text,
                        Category = category,
                        Year = int.Parse(DialogYearInput.Text),
                        Pages = int.Parse(DialogPagesInput.Text),
                        Price = decimal.Parse(DialogPriceInput.Text),
                        Description = selectedBookForEdit.Description,
                        ImagePath = selectedBookForEdit.ImagePath
                    };

                    _commandHistory.ExecuteCommand(new EditBookCommand(this, oldBook, newBook));
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


        private void ClearCart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Очистить корзину?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                cart.Clear();
                CartListView.Items.Refresh();
                UpdateTotalAmount();

                MessageBox.Show("Корзина очищена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ClearCart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cart.Count > 0;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Подписка на Bubbling событие (StarRatingControl)
            this.AddHandler(StarRatingControl.RatingChangedEvent,
                new RoutedEventHandler(Window_StarRating_RatingChanged));

            // Подписка на Tunneling событие (NumberBox)
            this.AddHandler(NumberBox.ValueChangedEvent,
                new RoutedEventHandler(Window_NumberBox_ValueChanged), true);

            System.Diagnostics.Debug.WriteLine("~~~ Демонстрация маршрутизации событий ~~~");
            System.Diagnostics.Debug.WriteLine("Туннелинг: срабатывает перед обработкой события элементом");
            System.Diagnostics.Debug.WriteLine("Бублинг: срабатывает после обработки события элементом");
        }
        private void Window_StarRating_RatingChanged(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== Bubbling (Window уровень) ===");
            System.Diagnostics.Debug.WriteLine($"OriginalSource: {e.OriginalSource} ↑ Sender: {sender}");
            System.Diagnostics.Debug.WriteLine("=================");
        }
        private void Window_NumberBox_ValueChanged(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== Window Tunneling ===");
            System.Diagnostics.Debug.WriteLine($"Sender: {sender} ↓ OriginalSource: {e.OriginalSource}");
        }
    }
}