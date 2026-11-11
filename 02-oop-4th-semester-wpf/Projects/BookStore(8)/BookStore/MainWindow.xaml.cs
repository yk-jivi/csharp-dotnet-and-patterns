using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
        private DbService _db = new DbService();
        private ObservableCollection<Book> books = new ObservableCollection<Book>();
        private ObservableCollection<Book> filteredBooks = new ObservableCollection<Book>();
        private ObservableCollection<CartItem> cart = new ObservableCollection<CartItem>();
        private CommandHistory _commandHistory;
        private User _currentUser = new User();

        private bool isAdminMode = false;
        private Book selectedBookForEdit = null!;
        private Book _selectedBook = null!;
        private Book _currentDetailsBook = null!;

        private byte[] currentSelectedImageBytes;

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
            DataContext = this;
            LoadUserData();
            CartListView.ItemsSource = cart;
            UpdateTotalAmount();
            _commandHistory = new CommandHistory(this);
            this.Loaded += Window_Loaded;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Регистрация маршрутизируемых событий
            // Подписка на Bubbling событие (StarRatingControl)
            this.AddHandler(StarRatingControl.RatingChangedEvent,
                new RoutedEventHandler(Window_StarRating_RatingChanged));

            // Подписка на Tunneling событие (NumberBox)
            this.AddHandler(NumberBox.ValueChangedEvent,
                new RoutedEventHandler(Window_NumberBox_ValueChanged), true);

            // 2. Логирование для отладки
            System.Diagnostics.Debug.WriteLine("~~~ Демонстрация маршрутизации событий ~~~");
            System.Diagnostics.Debug.WriteLine("Туннелинг: срабатывает перед обработкой события элементом");
            System.Diagnostics.Debug.WriteLine("Бублинг: срабатывает после обработки события элементом");

            try
            {
                // 3. Асинхронная загрузка данных
                var data = await _db.GetAllBooksAsync();

                // Важно: очищаем и добавляем в ту же коллекцию, к которой привязан UI
                books.Clear();
                foreach (var b in data)
                {
                    books.Add(b);
                }

                // 4. Применение фильтров (поиск, категория, сортировка)
                FilterBooks();

                if (BooksListView.ItemsSource != filteredBooks)
                {
                    BooksListView.ItemsSource = filteredBooks;
                }

                // 6. Уведомление пользователя
                MessageBox.Show("Загружено книг: " + books.Count.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
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

        public void FilterBooks()
        {
            // 1. Проверка на null обязательна, так как метод вызывается при инициализации XAML
            if (books == null || CategoryFilter == null || SortFilter == null || SearchBox == null)
                return;

            // Сохраняем ID выбранной книги до фильтрации
            int? selectedBookId = SelectedBook?.Id;
            var filtered = books.AsEnumerable();

            // 2. Улучшенная логика поиска
            string searchText = SearchBox.Text.Trim().ToLower();

            // Пытаемся достать плейсхолдер динамически. 
            // Если поиск совпадает с плейсхолдером — игнорируем его.
            string searchPlaceholder = FindResource("SearchPlaceholder")?.ToString()?.ToLower();

            if (!string.IsNullOrWhiteSpace(searchText) && searchText != searchPlaceholder)
            {
                filtered = filtered.Where(b =>
                    (!string.IsNullOrEmpty(b.Title) && b.Title.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(b.Author) && b.Author.ToLower().Contains(searchText)));
            }

            // 3. Фильтрация по категории
            // Используем Index > 0, так как 0 — это обычно "Все категории"
            if (CategoryFilter.SelectedIndex > 0)
            {
                var selectedItem = CategoryFilter.SelectedItem as ComboBoxItem;
                var selectedCategory = selectedItem?.Content?.ToString();

                if (!string.IsNullOrEmpty(selectedCategory))
                {
                    filtered = filtered.Where(b => b.Category == selectedCategory);
                }
            }

            // 4. Сортировка
            switch (SortFilter.SelectedIndex)
            {
                case 1: filtered = filtered.OrderBy(b => b.Price); break;
                case 2: filtered = filtered.OrderByDescending(b => b.Price); break;
                case 3: filtered = filtered.OrderBy(b => b.Title); break;
            }

            // 5. Обновление коллекции
            var filteredList = filtered.ToList();

            filteredBooks.Clear();
            foreach (var book in filteredList)
            {
                filteredBooks.Add(book);
            }

            // 6. Восстанавливаем выбранную книгу, если она есть в отфильтрованном списке
            if (selectedBookId.HasValue)
            {
                var restoredSelection = filteredBooks.FirstOrDefault(b => b.Id == selectedBookId.Value);
                SelectedBook = restoredSelection;

                // Если книга найдена, синхронизируем с ListView
                if (restoredSelection != null && BooksListView != null)
                {
                    BooksListView.SelectedItem = restoredSelection;
                }
            }

            // 7. Обновление UI
            if (BooksListView.ItemsSource != filteredBooks)
            {
                BooksListView.ItemsSource = filteredBooks;
            }
            else
            {
                BooksListView.Items.Refresh();
            }

            // Если плитка видна, обновляем и её
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
            if (cart.Count == 0) return;

            try
            {
                decimal total = cart.Sum(item => item.Book.Price * item.Quantity);

                // Вызов асинхронного метода из DbService (Задание 4c)
                int orderId = await _db.CreateOrderAsync(total);

                MessageBox.Show($"Заказ №{orderId} успешно сохранен в базе данных SQL!", "Успех");

                cart.Clear();
                UpdateTotalAmount();
                CartListView.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при работе с транзакцией заказа: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"Выбрана книга: ID={selectedBook.Id}, Title={selectedBook.Title}");
            }
            else
            {   
                SelectedBook = null;
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
            if (_commandHistory != null && _commandHistory.CanUndo)
                _commandHistory.Undo();
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_commandHistory != null && _commandHistory.CanRedo)
                _commandHistory.Redo();
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
            // Получаем книгу из CommandParameter если есть, иначе из SelectedBook
            Book bookToEdit = null;

            if (sender is Button button && button.CommandParameter is Book parameterBook)
            {
                bookToEdit = parameterBook;
            }
            else if (SelectedBook != null)
            {
                bookToEdit = SelectedBook;
            }

            if (bookToEdit == null)
            {
                MessageBox.Show("Выберите книгу для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedBookForEdit = bookToEdit;
            DialogTitle.Text = (string)Application.Current.Resources["EditTitle"];

            DialogTitleInput.Text = selectedBookForEdit.Title;
            DialogAuthorInput.Text = selectedBookForEdit.Author;

            // Устанавливаем категорию
            bool categoryFound = false;
            for (int i = 0; i < DialogCategoryInput.Items.Count; i++)
            {
                var item = DialogCategoryInput.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == selectedBookForEdit.Category)
                {
                    DialogCategoryInput.SelectedIndex = i;
                    categoryFound = true;
                    break;
                }
            }

            if (!categoryFound)
            {
                DialogCategoryInput.SelectedIndex = 0; // Выбираем "Выберите категорию"
            }

            DialogYearInput.Text = selectedBookForEdit.Year > 0 ? selectedBookForEdit.Year.ToString() : "";
            DialogPagesInput.Text = selectedBookForEdit.Pages > 0 ? selectedBookForEdit.Pages.ToString() : "";
            DialogPriceInput.Text = selectedBookForEdit.Price.ToString();

            // Сбрасываем временные байты изображения
            currentSelectedImageBytes = null;

            BookDialog.Visibility = Visibility.Visible;
        }

        private async void DeleteBookBtn_Click(object sender, RoutedEventArgs e)
        {
            // Получаем книгу из CommandParameter если есть, иначе из SelectedBook
            Book bookToDelete = null;

            if (sender is Button button && button.CommandParameter is Book parameterBook)
            {
                bookToDelete = parameterBook;
            }
            else if (SelectedBook != null)
            {
                bookToDelete = SelectedBook;
            }

            if (bookToDelete != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить книгу '{bookToDelete.Title}'?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // 1. Удаляем из базы данных
                        _db.DeleteBook(bookToDelete.Id);

                        // 2. Перезагружаем данные из базы
                        var updatedBooks = await _db.GetAllBooksAsync();

                        // 3. Очищаем и обновляем коллекцию
                        books.Clear();
                        foreach (var book in updatedBooks)
                        {
                            books.Add(book);
                        }

                        // 4. Очищаем выбранную книгу
                        SelectedBook = null;
                        BooksListView.SelectedItem = null;

                        // 5. Обновляем отображение
                        FilterBooks();

                        // 6. Обновляем корзину (удаляем книгу из корзины, если она там была)
                        var cartItem = cart.FirstOrDefault(i => i.Book.Id == bookToDelete.Id);
                        if (cartItem != null)
                        {
                            cart.Remove(cartItem);
                            UpdateTotalAmount();
                            CartListView.Items.Refresh();
                        }

                        // 7. Обновляем плиточный вид, если он активен
                        if (TileViewPanel.Visibility == Visibility.Visible)
                        {
                            UpdateTileView();
                        }

                        MessageBox.Show("Книга успешно удалена из базы данных.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении из БД: " + ex.Message, "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите книгу для удаления.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
                // Валидация полей
                if (string.IsNullOrWhiteSpace(DialogTitleInput.Text))
                {
                    MessageBox.Show("Введите название книги!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DialogCategoryInput.SelectedIndex <= 0)
                {
                    MessageBox.Show("Выберите категорию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                decimal price = 0;
                if (!decimal.TryParse(DialogPriceInput.Text, out price) || price <= 0)
                {
                    MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 1. Сначала разбираемся с категорией
                string selectedCatName = (DialogCategoryInput.SelectedItem as ComboBoxItem)?.Content.ToString();
                int categoryId = 1;

                switch (selectedCatName)
                {
                    case "Романы": categoryId = 1; break;
                    case "Поэзия": categoryId = 2; break;
                    case "Драма": categoryId = 3; break;
                    case "Фэнтези": categoryId = 4; break;
                    case "Классика": categoryId = 5; break;
                    default: categoryId = 1; break;
                }

                // 2. Сбор данных с полей
                int year = 0;
                int.TryParse(DialogYearInput.Text, out year);

                int pages = 0;
                int.TryParse(DialogPagesInput.Text, out pages);

                var bookData = new Book
                {
                    Title = DialogTitleInput.Text.Trim(),
                    Author = DialogAuthorInput.Text.Trim(),
                    Price = price,
                    Year = year,
                    Pages = pages,
                    Category = selectedCatName,
                    ImageBytes = this.currentSelectedImageBytes
                };

                if (selectedBookForEdit == null)
                {
                    // --- ДОБАВЛЕНИЕ ---
                    _db.AddBook(bookData, categoryId);

                    // Перезагружаем данные из базы
                    LoadBooksFromDatabase();
                }
                else
                {
                    // --- РЕДАКТИРОВАНИЕ ---
                    bookData.Id = selectedBookForEdit.Id;

                    // Если в этом сеансе картинку не выбирали, оставляем ту, что была у книги
                    if (bookData.ImageBytes == null)
                    {
                        bookData.ImageBytes = selectedBookForEdit.ImageBytes;
                    }

                    _db.UpdateBook(bookData, categoryId);

                    // Перезагружаем данные из базы
                    LoadBooksFromDatabase();

                    selectedBookForEdit = null;
                }

                // 3. Финальные действия
                this.currentSelectedImageBytes = null;
                BookDialog.Visibility = Visibility.Collapsed;

                MessageBox.Show("Данные успешно синхронизированы с базой!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Добавьте вспомогательный метод для перезагрузки книг
        private async void LoadBooksFromDatabase()
        {
            try
            {
                var data = await _db.GetAllBooksAsync();
                books.Clear();
                foreach (var b in data)
                {
                    books.Add(b);
                }
                FilterBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void SelectImageBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Изображения|*.jpg;*.png;*.jpeg";
            if (dlg.ShowDialog() == true)
            {
                this.currentSelectedImageBytes = System.IO.File.ReadAllBytes(dlg.FileName);
                MessageBox.Show("Картинка успешно загружена в память!");
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

        // Показ таблицы Книг
        private async void NavBooks_Click(object sender, RoutedEventArgs e)
        {
            MainDataGrid.Visibility = Visibility.Collapsed;
            BooksListView.Visibility = Visibility.Visible; // Показываем основной список

            var data = await _db.GetAllBooksAsync();
            books.Clear();
            foreach (var b in data) books.Add(b);
            FilterBooks();
        }

        // Показ таблицы Категорий (в DataGrid)
        private async void NavCategories_Click(object sender, RoutedEventArgs e)
        {
            BooksListView.Visibility = Visibility.Collapsed; // Скрываем список книг
            MainDataGrid.Visibility = Visibility.Visible;    // Показываем таблицу

            MainDataGrid.ItemsSource = (await _db.GetCategoriesTableAsync()).DefaultView;
        }

        // Показ таблицы Заказов (в DataGrid)
        private async void NavOrders_Click(object sender, RoutedEventArgs e)
        {
            BooksListView.Visibility = Visibility.Collapsed;
            MainDataGrid.Visibility = Visibility.Visible;

            MainDataGrid.ItemsSource = (await _db.GetOrdersTableAsync()).DefaultView;
        }
    }
}