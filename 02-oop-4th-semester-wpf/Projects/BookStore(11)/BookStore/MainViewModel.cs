using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media;

namespace BookStore
{
    public class MainViewModel : ViewModelBase
    {
        private readonly EFService _db = new EFService();
        private CommandHistory _commandHistory;

        // Коллекции
        private ObservableCollection<Book> _books = new ObservableCollection<Book>();
        private ObservableCollection<Book> _filteredBooks = new ObservableCollection<Book>();
        private ObservableCollection<CartItem> _cart = new ObservableCollection<CartItem>();

        // Выбранные элементы
        private Book? _selectedBook;
        private Book? _currentDetailsBook;
        private User _currentUser = new User();

        // Состояния UI
        private bool _isAdminMode = false;
        private bool _isListViewVisible = true;
        private bool _isTileViewVisible = false;
        private bool _isDetailsPopupVisible = false;
        private bool _isAdminPanelVisible = false;
        private bool _isBookDialogVisible = false;
        private bool _isMainDataGridVisible = false;

        // Данные для диалога книги
        private Book? _selectedBookForEdit;
        private byte[]? _currentSelectedImageBytes;
        private string _dialogTitle = "";
        private string _dialogTitleInput = "";
        private string _dialogAuthorInput = "";
        private int _dialogCategorySelectedIndex = 0;
        private string _dialogYearInput = "";
        private string _dialogPagesInput = "";
        private string _dialogPriceInput = "";

        // Данные для деталей книги
        private string _detailsTitle = "";
        private string _detailsAuthor = "";
        private string _detailsCategory = "";
        private string _detailsYear = "";
        private string _detailsPages = "";
        private string _detailsPrice = "";
        private string _detailsDescription = "";

        // Поиск и фильтры
        private string _searchText = "";
        private int _categoryFilterSelectedIndex = 0;
        private int _sortFilterSelectedIndex = 0;

        private string[] _categories = { "Все категории", "Романы", "Поэзия", "Драма", "Фэнтези", "Классика" };
        private string[] _sortOptions = { "Сортировка", "Цена ↑", "Цена ↓", "По названию" };

        public MainViewModel()
        {
            LoadUserData();
            _commandHistory = new CommandHistory(this);
            LoadBooksCommand = new RelayCommand(async _ => await LoadBooksAsync());
            FilterBooksCommand = new RelayCommand(_ => FilterBooks());
            AddToCartCommand = new RelayCommand(book => AddToCart(book as Book));
            RemoveFromCartCommand = new RelayCommand(bookId => RemoveFromCart((int)bookId!));
            CheckoutCommand = new RelayCommand(async _ => await CheckoutAsync());
            ClearCartCommand = new RelayCommand(_ => ClearCart(), _ => Cart.Count > 0);
            ShowDetailsCommand = new RelayCommand(book => ShowDetails(book as Book));
            CloseDetailsCommand = new RelayCommand(_ => CloseDetails());
            SwitchToListViewCommand = new RelayCommand(_ => SwitchToListView());
            SwitchToTileViewCommand = new RelayCommand(_ => SwitchToTileView());
            ChangeLanguageCommand = new RelayCommand(lang => ChangeLanguage(lang as string));
            ChangeThemeCommand = new RelayCommand(theme => ChangeTheme(theme as string));
            SwitchToUserModeCommand = new RelayCommand(_ => SwitchToUserMode());
            SwitchToAdminModeCommand = new RelayCommand(_ => SwitchToAdminMode());
            OpenAccountCommand = new RelayCommand(_ => OpenAccount());
            UndoCommand = new RelayCommand(_ => Undo(), _ => _commandHistory.CanUndo);
            RedoCommand = new RelayCommand(_ => Redo(), _ => _commandHistory.CanRedo);
            AddBookCommand = new RelayCommand(_ => OpenAddBookDialog());
            EditBookCommand = new RelayCommand(book => OpenEditBookDialog(book as Book));
            DeleteBookCommand = new RelayCommand(async book => await DeleteBookAsync(book as Book));
            SaveBookDialogCommand = new RelayCommand(async _ => await SaveBookDialogAsync());
            CancelBookDialogCommand = new RelayCommand(_ => CancelBookDialog());
            SelectImageCommand = new RelayCommand(_ => SelectImage());
            NavigateBooksCommand = new RelayCommand(async _ => await NavigateToBooksAsync());
            NavigateCategoriesCommand = new RelayCommand(_ => NavigateToCategories());
            NavigateOrdersCommand = new RelayCommand(_ => NavigateToOrders());
        }

        // Публичные свойства

        public ObservableCollection<Book> Books { get => _books; set => SetProperty(ref _books, value); }
        public ObservableCollection<Book> FilteredBooks { get => _filteredBooks; set => SetProperty(ref _filteredBooks, value); }
        public ObservableCollection<CartItem> Cart { get => _cart; set => SetProperty(ref _cart, value); }

        public Book? SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        public Book? CurrentDetailsBook
        {
            get => _currentDetailsBook;
            set => SetProperty(ref _currentDetailsBook, value);
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public bool IsAdminMode
        {
            get => _isAdminMode;
            set => SetProperty(ref _isAdminMode, value);
        }

        public bool IsListViewVisible
        {
            get => _isListViewVisible;
            set => SetProperty(ref _isListViewVisible, value);
        }

        public bool IsTileViewVisible
        {
            get => _isTileViewVisible;
            set => SetProperty(ref _isTileViewVisible, value);
        }

        public bool IsDetailsPopupVisible
        {
            get => _isDetailsPopupVisible;
            set => SetProperty(ref _isDetailsPopupVisible, value);
        }

        public bool IsAdminPanelVisible
        {
            get => _isAdminPanelVisible;
            set => SetProperty(ref _isAdminPanelVisible, value);
        }

        public bool IsBookDialogVisible
        {
            get => _isBookDialogVisible;
            set => SetProperty(ref _isBookDialogVisible, value);
        }

        public bool IsMainDataGridVisible
        {
            get => _isMainDataGridVisible;
            set => SetProperty(ref _isMainDataGridVisible, value);
        }

        public DataGridItemsSource DataGridSource { get; set; } = new DataGridItemsSource();

        // Данные диалога
        public string DialogTitleText { get => _dialogTitle; set => SetProperty(ref _dialogTitle, value); }
        public string DialogTitleInput { get => _dialogTitleInput; set => SetProperty(ref _dialogTitleInput, value); }
        public string DialogAuthorInput { get => _dialogAuthorInput; set => SetProperty(ref _dialogAuthorInput, value); }
        public int DialogCategorySelectedIndex { get => _dialogCategorySelectedIndex; set => SetProperty(ref _dialogCategorySelectedIndex, value); }
        public string DialogYearInput { get => _dialogYearInput; set => SetProperty(ref _dialogYearInput, value); }
        public string DialogPagesInput { get => _dialogPagesInput; set => SetProperty(ref _dialogPagesInput, value); }
        public string DialogPriceInput { get => _dialogPriceInput; set => SetProperty(ref _dialogPriceInput, value); }

        // Данные деталей
        public string DetailsTitle { get => _detailsTitle; set => SetProperty(ref _detailsTitle, value); }
        public string DetailsAuthor { get => _detailsAuthor; set => SetProperty(ref _detailsAuthor, value); }
        public string DetailsCategory { get => _detailsCategory; set => SetProperty(ref _detailsCategory, value); }
        public string DetailsYear { get => _detailsYear; set => SetProperty(ref _detailsYear, value); }
        public string DetailsPages { get => _detailsPages; set => SetProperty(ref _detailsPages, value); }
        public string DetailsPrice { get => _detailsPrice; set => SetProperty(ref _detailsPrice, value); }
        public string DetailsDescription { get => _detailsDescription; set => SetProperty(ref _detailsDescription, value); }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    FilterBooks();
            }
        }

        public int CategoryFilterSelectedIndex
        {
            get => _categoryFilterSelectedIndex;
            set
            {
                if (SetProperty(ref _categoryFilterSelectedIndex, value))
                    FilterBooks();
            }
        }

        public int SortFilterSelectedIndex
        {
            get => _sortFilterSelectedIndex;
            set
            {
                if (SetProperty(ref _sortFilterSelectedIndex, value))
                    FilterBooks();
            }
        }

        public string[] Categories => _categories;
        public string[] SortOptions => _sortOptions;

        public string TotalAmount => $"{Cart.Sum(item => item.Book.Price * item.Quantity)} BYN";
        public string CartItemsCount => $"Всего товаров: {Cart.Sum(i => i.Quantity)}";

        // ==================== Команды ====================

        public ICommand LoadBooksCommand { get; }
        public ICommand FilterBooksCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand ClearCartCommand { get; }
        public ICommand ShowDetailsCommand { get; }
        public ICommand CloseDetailsCommand { get; }
        public ICommand SwitchToListViewCommand { get; }
        public ICommand SwitchToTileViewCommand { get; }
        public ICommand ChangeLanguageCommand { get; }
        public ICommand ChangeThemeCommand { get; }
        public ICommand SwitchToUserModeCommand { get; }
        public ICommand SwitchToAdminModeCommand { get; }
        public ICommand OpenAccountCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand SaveBookDialogCommand { get; }
        public ICommand CancelBookDialogCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand NavigateBooksCommand { get; }
        public ICommand NavigateCategoriesCommand { get; }
        public ICommand NavigateOrdersCommand { get; }

        // ==================== Методы ====================

        private async Task LoadBooksAsync()
        {
            try
            {
                var data = await _db.GetAllBooksAsync();
                Books.Clear();
                foreach (var b in data)
                    Books.Add(b);
                FilterBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка");
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

        public async void FilterBooks()
        {
            if (Books == null) return;

            try
            {
                int? selectedBookId = SelectedBook?.Id;

                string searchText = SearchText?.Trim().ToLower() ?? "";
                string searchPlaceholder = GetSearchPlaceholder();
                if (searchText == searchPlaceholder) searchText = "";

                string? selectedCategory = null;
                if (CategoryFilterSelectedIndex > 0)
                    selectedCategory = Categories[CategoryFilterSelectedIndex];

                string? sortBy = null;
                bool ascending = true;

                switch (SortFilterSelectedIndex)
                {
                    case 1: sortBy = "price"; ascending = true; break;
                    case 2: sortBy = "price"; ascending = false; break;
                    case 3: sortBy = "title"; ascending = true; break;
                }

                var filtered = await _db.GetFilteredBooksAsync(searchText, selectedCategory, sortBy, ascending);
                FilteredBooks.Clear();
                foreach (var book in filtered)
                    FilteredBooks.Add(book);

                if (selectedBookId.HasValue)
                {
                    var restored = FilteredBooks.FirstOrDefault(b => b.Id == selectedBookId.Value);
                    SelectedBook = restored;
                }

                OnPropertyChanged(nameof(FilteredBooks));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка");
            }
        }

        private string GetSearchPlaceholder()
        {
            return Application.Current.Resources["SearchPlaceholder"]?.ToString()?.ToLower() ?? "";
        }

        private void AddToCart(Book? book)
        {
            if (book == null) return;

            var existing = Cart.FirstOrDefault(item => item.Book.Id == book.Id);
            if (existing != null)
                existing.Quantity += book.SelectedQuantity;
            else
                Cart.Add(new CartItem { Book = book, Quantity = book.SelectedQuantity });

            OnPropertyChanged(nameof(Cart));
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(CartItemsCount));
            ((RelayCommand)ClearCartCommand).RaiseCanExecuteChanged();

            MessageBox.Show($"Книга '{book.Title}' добавлена в корзину в количестве {book.SelectedQuantity} шт.",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RemoveFromCart(int bookId)
        {
            var item = Cart.FirstOrDefault(i => i.Book.Id == bookId);
            if (item != null)
            {
                if (item.Quantity > 1) item.Quantity--;
                else Cart.Remove(item);
                OnPropertyChanged(nameof(Cart));
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(CartItemsCount));
                ((RelayCommand)ClearCartCommand).RaiseCanExecuteChanged();
            }
        }

        private void ClearCart()
        {
            if (MessageBox.Show("Очистить корзину?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Cart.Clear();
                OnPropertyChanged(nameof(Cart));
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(CartItemsCount));
                ((RelayCommand)ClearCartCommand).RaiseCanExecuteChanged();
                MessageBox.Show("Корзина очищена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task CheckoutAsync()
        {
            if (Cart.Count == 0) return;

            try
            {
                int orderId = await _db.CreateOrderAsync(Cart);
                MessageBox.Show($"Заказ №{orderId} создан!", "Успех");
                Cart.Clear();
                OnPropertyChanged(nameof(Cart));
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(CartItemsCount));
                ((RelayCommand)ClearCartCommand).RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ShowDetails(Book? book)
        {
            if (book == null) return;
            CurrentDetailsBook = book;
            DetailsTitle = book.Title;
            DetailsAuthor = book.Author;
            DetailsCategory = book.Category;
            DetailsYear = book.Year.ToString();
            DetailsPages = book.Pages.ToString();
            DetailsPrice = $"{book.Price} BYN";
            DetailsDescription = book.Description;
            IsDetailsPopupVisible = true;
        }

        private void CloseDetails()
        {
            IsDetailsPopupVisible = false;
            CurrentDetailsBook = null;
        }

        private void SwitchToListView()
        {
            IsListViewVisible = true;
            IsTileViewVisible = false;
        }

        private void SwitchToTileView()
        {
            IsListViewVisible = false;
            IsTileViewVisible = true;
        }

        public void ChangeLanguage(string? language)
        {
            if (string.IsNullOrEmpty(language)) return;

            Application.Current.Resources.MergedDictionaries.Clear();
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = language == "en"
                ? new Uri("Strings.en.xaml", UriKind.Relative)
                : new Uri("Strings.ru.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(dict);
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary() { Source = new Uri($"{CurrentUser.Theme}.xaml", UriKind.Relative) });

            CurrentUser.Language = language;
            OnPropertyChanged(nameof(CurrentUser));
        }

        public void ChangeTheme(string? themeName)
        {
            if (string.IsNullOrEmpty(themeName)) return;

            try
            {
                var themeDict = new ResourceDictionary();
                themeDict.Source = new Uri($"{themeName}.xaml", UriKind.Relative);

                var toRemove = Application.Current.Resources.MergedDictionaries
                    .Where(d => d.Source != null && d.Source.ToString().Contains("Theme")).ToList();
                foreach (var dict in toRemove)
                    Application.Current.Resources.MergedDictionaries.Remove(dict);

                Application.Current.Resources.MergedDictionaries.Add(themeDict);
                CurrentUser.Theme = themeName;
                OnPropertyChanged(nameof(CurrentUser));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при смене темы: {ex.Message}");
            }
        }

        private void SwitchToUserMode()
        {
            IsAdminMode = false;
            IsAdminPanelVisible = false;
            CurrentUser.Role = "User";
            CurrentUser.Username = "Гость";
            OnPropertyChanged(nameof(CurrentUser));
            MessageBox.Show("Вы переключились в режим пользователя", "Режим", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SwitchToAdminMode()
        {
            IsAdminMode = true;
            IsAdminPanelVisible = true;
            CurrentUser.Role = "Admin";
            CurrentUser.Username = "Администратор";
            OnPropertyChanged(nameof(CurrentUser));
            MessageBox.Show("Вы переключились в режим администратора\nДоступны функции: добавление, редактирование, удаление книг",
                "Режим", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenAccount()
        {
            try
            {
                var accountWindow = new PersonalAccountWindow(this);
                accountWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия личного кабинета: {ex.Message}");
            }
        }

        private void Undo()
        {
            if (_commandHistory.CanUndo)
                _commandHistory.Undo();
        }

        private void Redo()
        {
            if (_commandHistory.CanRedo)
                _commandHistory.Redo();
        }

        private void OpenAddBookDialog()
        {
            _selectedBookForEdit = null;
            DialogTitleText = (string)Application.Current.Resources["AddTitle"];
            ClearDialogFields();
            IsBookDialogVisible = true;
        }

        private void OpenEditBookDialog(Book? book)
        {
            if (book == null)
            {
                if (SelectedBook == null)
                {
                    MessageBox.Show("Выберите книгу для редактирования!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                book = SelectedBook;
            }

            _selectedBookForEdit = book;
            DialogTitleText = (string)Application.Current.Resources["EditTitle"];
            DialogTitleInput = book.Title;
            DialogAuthorInput = book.Author;

            int catIndex = Array.FindIndex(Categories, c => c == book.Category);
            DialogCategorySelectedIndex = catIndex >= 0 ? catIndex : 0;

            DialogYearInput = book.Year > 0 ? book.Year.ToString() : "";
            DialogPagesInput = book.Pages > 0 ? book.Pages.ToString() : "";
            DialogPriceInput = book.Price.ToString();
            _currentSelectedImageBytes = null;
            IsBookDialogVisible = true;
        }

        private async Task DeleteBookAsync(Book? book)
        {
            if (book == null && SelectedBook == null)
            {
                MessageBox.Show("Выберите книгу для удаления.");
                return;
            }

            var bookToDelete = book ?? SelectedBook;
            if (bookToDelete == null) return;

            var result = MessageBox.Show($"Удалить книгу '{bookToDelete.Title}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _db.DeleteBookWithTransactionAsync(bookToDelete.Id);
                    await LoadBooksAsync();

                    var cartItem = Cart.FirstOrDefault(i => i.Book.Id == bookToDelete.Id);
                    if (cartItem != null)
                    {
                        Cart.Remove(cartItem);
                        OnPropertyChanged(nameof(Cart));
                        OnPropertyChanged(nameof(TotalAmount));
                        OnPropertyChanged(nameof(CartItemsCount));
                    }

                    SelectedBook = null;
                    MessageBox.Show("Книга удалена!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void ClearDialogFields()
        {
            DialogTitleInput = "";
            DialogAuthorInput = "";
            DialogCategorySelectedIndex = 0;
            DialogYearInput = "";
            DialogPagesInput = "";
            DialogPriceInput = "";
            _currentSelectedImageBytes = null;
        }

        private async Task SaveBookDialogAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DialogTitleInput))
                {
                    MessageBox.Show("Введите название книги!", "Ошибка");
                    return;
                }

                if (DialogCategorySelectedIndex <= 0)
                {
                    MessageBox.Show("Выберите категорию!", "Ошибка");
                    return;
                }

                if (!decimal.TryParse(DialogPriceInput, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Введите корректную цену!", "Ошибка");
                    return;
                }

                int categoryId = DialogCategorySelectedIndex; // 1-5

                int.TryParse(DialogYearInput, out int year);
                int.TryParse(DialogPagesInput, out int pages);

                var bookData = new Book
                {
                    Title = DialogTitleInput.Trim(),
                    Author = DialogAuthorInput.Trim(),
                    Price = price,
                    Year = year,
                    Pages = pages,
                    ImageBytes = _currentSelectedImageBytes
                };

                if (_selectedBookForEdit == null)
                {
                    await _db.AddBookWithTransactionAsync(bookData, categoryId);
                    MessageBox.Show("Книга добавлена!");
                }
                else
                {
                    bookData.Id = _selectedBookForEdit.Id;
                    await _db.UpdateBookWithTransactionAsync(bookData, categoryId);
                    MessageBox.Show("Книга обновлена!");
                }

                await LoadBooksAsync();
                _currentSelectedImageBytes = null;
                IsBookDialogVisible = false;
                _selectedBookForEdit = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void CancelBookDialog()
        {
            IsBookDialogVisible = false;
            _selectedBookForEdit = null;
            _currentSelectedImageBytes = null;
        }

        private void SelectImage()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Изображения|*.jpg;*.png;*.jpeg";
            if (dlg.ShowDialog() == true)
            {
                _currentSelectedImageBytes = System.IO.File.ReadAllBytes(dlg.FileName);
                MessageBox.Show("Картинка успешно загружена в память!");
            }
        }

        private async Task NavigateToBooksAsync()
        {
            IsMainDataGridVisible = false;
            IsListViewVisible = true;
            IsTileViewVisible = false;
            await LoadBooksAsync();
        }

        private void NavigateToCategories()
        {
            IsListViewVisible = false;
            IsTileViewVisible = false;
            IsMainDataGridVisible = true;
            DataGridSource.Items = _db.GetCategoriesTable().DefaultView;
            OnPropertyChanged(nameof(DataGridSource));
        }

        private void NavigateToOrders()
        {
            IsListViewVisible = false;
            IsTileViewVisible = false;
            IsMainDataGridVisible = true;
            DataGridSource.Items = _db.GetOrdersTable().DefaultView;
            OnPropertyChanged(nameof(DataGridSource));
        }

        // Методы для совместимости с CommandHistory
        public void AddBookDirect(Book book) { Books.Add(book); FilterBooks(); }
        public void RemoveBookDirect(int bookId)
        {
            var book = Books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                Books.Remove(book);
                var cartItem = Cart.FirstOrDefault(i => i.Book.Id == bookId);
                if (cartItem != null) Cart.Remove(cartItem);
                FilterBooks();
                OnPropertyChanged(nameof(Cart));
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(CartItemsCount));
            }
        }
        public void EditBookDirect(Book book)
        {
            var existing = Books.FirstOrDefault(b => b.Id == book.Id);
            if (existing != null)
            {
                existing.Title = book.Title;
                existing.Author = book.Author;
                existing.Year = book.Year;
                existing.Pages = book.Pages;
                existing.Price = book.Price;
                FilterBooks();
            }
        }
    }

    public class DataGridItemsSource : ViewModelBase
    {
        private System.Data.DataView? _items;
        public System.Data.DataView? Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }
    }
}