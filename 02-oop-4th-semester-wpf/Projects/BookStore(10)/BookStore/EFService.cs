using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BookStore.DAL;
using System.Data.Entity;

namespace BookStore
{
    // Сервис для работы с данными через паттерны Repository + Unit of Work
    public class EFService
    {

        // Получить все книги с категориями (синхронно, для обратной совместимости)
        public ObservableCollection<Book> GetAllBooks()
        {
            using (var uow = new UnitOfWork())
            {
                var books = uow.Books.Query()
                    .Include(b => b.CategoryEntity)
                    .ToList();
                return new ObservableCollection<Book>(books);
            }
        }

        // Получить все книги асинхронно
        public async Task<ObservableCollection<Book>> GetAllBooksAsync()
        {
            using (var uow = new UnitOfWork())
            {
                var books = await uow.Books.Query()
                    .Include(b => b.CategoryEntity)
                    .ToListAsync();
                return new ObservableCollection<Book>(books);
            }
        }

        // Получить отфильтрованные книги (для MainWindow.FilterBooks)
        public async Task<ObservableCollection<Book>> GetFilteredBooksAsync(
            string searchText = null,
            string category = null,
            string sortBy = null,
            bool ascending = true)
        {
            using (var uow = new UnitOfWork())
            {
                // Получаем IQueryable из репозитория
                IQueryable<Book> query = uow.Books.Query()
                    .Include(b => b.CategoryEntity);

                // Поиск по нескольким полям
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    searchText = searchText.ToLower();
                    query = query.Where(b =>
                        b.Title.ToLower().Contains(searchText) ||
                        b.Author.ToLower().Contains(searchText) ||
                        b.Description.ToLower().Contains(searchText));
                }

                // Фильтрация по категории
                if (!string.IsNullOrWhiteSpace(category) && category != "Все категории")
                {
                    query = query.Where(b => b.CategoryEntity.Name == category);
                }

                // Сортировка
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLower() switch
                    {
                        "title" => ascending
                            ? query.OrderBy(b => b.Title)
                            : query.OrderByDescending(b => b.Title),
                        "author" => ascending
                            ? query.OrderBy(b => b.Author)
                            : query.OrderByDescending(b => b.Author),
                        "price" => ascending
                            ? query.OrderBy(b => b.Price)
                            : query.OrderByDescending(b => b.Price),
                        "year" => ascending
                            ? query.OrderBy(b => b.Year)
                            : query.OrderByDescending(b => b.Year),
                        _ => query
                    };
                }

                var result = await query.ToListAsync();
                return new ObservableCollection<Book>(result);
            }
        }

        // Добавить книгу (синхронно)
        public void AddBook(Book book, int categoryId)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    book.CategoryId = categoryId;
                    uow.Books.Add(book);
                    uow.Save();
                    uow.Commit();
                }
                catch (Exception)
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        // Добавить книгу асинхронно (с транзакцией)
        public async Task AddBookWithTransactionAsync(Book book, int categoryId)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    book.CategoryId = categoryId;
                    uow.Books.Add(book);
                    await uow.SaveAsync();
                    uow.Commit();
                }
                catch (Exception)
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        // Обновить книгу (синхронно)
        public void UpdateBook(Book book, int categoryId)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    var existingBook = uow.Books.Query()
                        .FirstOrDefault(b => b.Id == book.Id);

                    if (existingBook != null)
                    {
                        existingBook.Title = book.Title;
                        existingBook.Author = book.Author;
                        existingBook.Price = book.Price;
                        existingBook.Year = book.Year;
                        existingBook.Pages = book.Pages;
                        existingBook.CategoryId = categoryId;
                        existingBook.Description = book.Description;

                        if (book.ImageBytes != null)
                        {
                            existingBook.ImageBytes = book.ImageBytes;
                        }

                        uow.Books.Update(existingBook);
                        uow.Save();
                        uow.Commit();
                    }
                }
                catch (Exception)
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        // Обновить книгу асинхронно (с транзакцией)
        public async Task UpdateBookWithTransactionAsync(Book book, int categoryId)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    var existingBook = await uow.Books.Query()
                        .FirstOrDefaultAsync(b => b.Id == book.Id);

                    if (existingBook != null)
                    {
                        existingBook.Title = book.Title;
                        existingBook.Author = book.Author;
                        existingBook.Price = book.Price;
                        existingBook.Year = book.Year;
                        existingBook.Pages = book.Pages;
                        existingBook.CategoryId = categoryId;
                        existingBook.Description = book.Description;

                        if (book.ImageBytes != null)
                        {
                            existingBook.ImageBytes = book.ImageBytes;
                        }

                        uow.Books.Update(existingBook);
                        await uow.SaveAsync();
                        uow.Commit();
                    }
                }
                catch (Exception)
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        // Удалить книгу (синхронно)
        public void DeleteBook(int id)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    uow.Books.Delete(id);
                    uow.Save();
                    uow.Commit();
                }
                catch (Exception)
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        // Удалить книгу асинхронно (с транзакцией)
        public async Task DeleteBookWithTransactionAsync(int id)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    uow.Books.Delete(id);
                    await uow.SaveAsync();
                    uow.Commit();
                }
                catch (Exception)
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        // Создать заказ (синхронно)
        public int CreateOrder(ObservableCollection<CartItem> cart)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    // Создаем заказ
                    var order = new Order
                    {
                        OrderDate = DateTime.Now,
                        TotalAmount = cart.Sum(item => item.TotalPrice)
                    };
                    uow.Orders.Add(order);
                    uow.Save();

                    // Добавляем элементы заказа
                    foreach (var item in cart)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            BookId = item.Book.Id,
                            Quantity = item.Quantity,
                            UnitPrice = item.Book.Price
                        };
                        uow.OrderItems.Add(orderItem);
                    }

                    uow.Save();
                    uow.Commit();
                    return order.Id;
                }
                catch (Exception)
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        // Создать заказ асинхронно
        public async Task<int> CreateOrderAsync(ObservableCollection<CartItem> cart)
        {
            using (var uow = new UnitOfWork())
            {
                try
                {
                    uow.BeginTransaction();

                    var order = new Order
                    {
                        OrderDate = DateTime.Now,
                        TotalAmount = cart.Sum(item => item.TotalPrice)
                    };
                    uow.Orders.Add(order);
                    await uow.SaveAsync();

                    foreach (var item in cart)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            BookId = item.Book.Id,
                            Quantity = item.Quantity,
                            UnitPrice = item.Book.Price
                        };
                        uow.OrderItems.Add(orderItem);
                    }

                    await uow.SaveAsync();
                    uow.Commit();
                    return order.Id;
                }
                catch (Exception)
                {
                    uow.Rollback();
                    throw;
                }
            }
        }

        // Получить таблицу категорий
        public DataTable GetCategoriesTable()
        {
            using (var uow = new UnitOfWork())
            {
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Name", typeof(string));

                var categories = uow.Categories.Query().ToList();
                foreach (var c in categories)
                {
                    dt.Rows.Add(c.Id, c.Name);
                }
                return dt;
            }
        }

        // Получить таблицу заказов
        public DataTable GetOrdersTable()
        {
            using (var uow = new UnitOfWork())
            {
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("OrderDate", typeof(DateTime));
                dt.Columns.Add("TotalAmount", typeof(decimal));

                var orders = uow.Orders.Query().ToList();
                foreach (var o in orders)
                {
                    dt.Rows.Add(o.Id, o.OrderDate, o.TotalAmount);
                }
                return dt;
            }
        }

        // Создать БД и заполнить начальными данными
        public void EnsureDatabaseCreated()
        {
            using (var uow = new UnitOfWork())
            {
                if (!uow.Categories.Query().Any())
                {
                    uow.Categories.Add(new Category { Name = "Романы" });
                    uow.Categories.Add(new Category { Name = "Поэзия" });
                    uow.Categories.Add(new Category { Name = "Драма" });
                    uow.Categories.Add(new Category { Name = "Фэнтези" });
                    uow.Categories.Add(new Category { Name = "Классика" });
                    uow.Save();
                }
            }
        }
    }
}