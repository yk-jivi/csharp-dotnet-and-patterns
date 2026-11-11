using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore
{
    public class EFService
    {
        //Асинхронные методы

        public async Task<ObservableCollection<Book>> GetAllBooksAsync()
        {
            using (var db = new BookStoreContext())
            {
                var books = await db.Books
                    .Include(b => b.CategoryEntity)
                    .ToListAsync();
                return new ObservableCollection<Book>(books);
            }
        }

        public async Task<ObservableCollection<Book>> GetFilteredBooksAsync(
            string searchText = null,
            string category = null,
            string sortBy = null,
            bool ascending = true)
        {
            using (var db = new BookStoreContext())
            {
                IQueryable<Book> query = db.Books.Include(b => b.CategoryEntity);

                //Поиск по нескольким полям
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    searchText = searchText.ToLower();
                    query = query.Where(b =>
                        b.Title.ToLower().Contains(searchText) ||
                        b.Author.ToLower().Contains(searchText) ||
                        b.Description.ToLower().Contains(searchText));
                }

                //Фильтрация по категориям
                if (!string.IsNullOrWhiteSpace(category) && category != "Все категории")
                {
                    query = query.Where(b => b.CategoryEntity.Name == category);
                }

                //Сортировка
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

        //CRUD с транзакциями

        public async Task AddBookWithTransactionAsync(Book book, int categoryId)
        {
            using (var db = new BookStoreContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        book.CategoryId = categoryId;
                        db.Books.Add(book);

                        await db.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task UpdateBookWithTransactionAsync(Book book, int categoryId)
        {
            using (var db = new BookStoreContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var existingBook = await db.Books.FindAsync(book.Id);
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

                            await db.SaveChangesAsync();
                            transaction.Commit();
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task DeleteBookWithTransactionAsync(int id)
        {
            using (var db = new BookStoreContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var book = await db.Books.FindAsync(id);
                        if (book != null)
                        {
                            db.Books.Remove(book);
                            await db.SaveChangesAsync();
                            transaction.Commit();
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        //Синхкронные методы

        public ObservableCollection<Book> GetAllBooks()
        {
            using (var db = new BookStoreContext())
            {
                var books = db.Books.Include(b => b.CategoryEntity).ToList();
                return new ObservableCollection<Book>(books);
            }
        }

        public void AddBook(Book book, int categoryId)
        {
            using (var db = new BookStoreContext())
            {
                book.CategoryId = categoryId;
                db.Books.Add(book);
                db.SaveChanges();
            }
        }

        public void UpdateBook(Book book, int categoryId)
        {
            using (var db = new BookStoreContext())
            {
                var existingBook = db.Books.Find(book.Id);
                if (existingBook != null)
                {
                    existingBook.Title = book.Title;
                    existingBook.Author = book.Author;
                    existingBook.Price = book.Price;
                    existingBook.Year = book.Year;
                    existingBook.Pages = book.Pages;
                    existingBook.CategoryId = categoryId;

                    if (book.ImageBytes != null)
                    {
                        existingBook.ImageBytes = book.ImageBytes;
                    }

                    db.SaveChanges();
                }
            }
        }

        public void DeleteBook(int id)
        {
            using (var db = new BookStoreContext())
            {
                var book = db.Books.Find(id);
                if (book != null)
                {
                    db.Books.Remove(book);
                    db.SaveChanges();
                }
            }
        }

        public int CreateOrder(ObservableCollection<CartItem> cart)
        {
            using (var db = new BookStoreContext())
            {
                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.Sum(item => item.TotalPrice)
                };

                db.Orders.Add(order);
                db.SaveChanges();

                return order.Id;
            }
        }

        //Вспомогательные методы

        public DataTable GetCategoriesTable()
        {
            using (var db = new BookStoreContext())
            {
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Name", typeof(string));

                var categories = db.Categories.ToList();
                foreach (var c in categories)
                {
                    dt.Rows.Add(c.Id, c.Name);
                }
                return dt;
            }
        }

        public DataTable GetOrdersTable()
        {
            using (var db = new BookStoreContext())
            {
                var dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("OrderDate", typeof(DateTime));
                dt.Columns.Add("TotalAmount", typeof(decimal));

                var orders = db.Orders.ToList();
                foreach (var o in orders)
                {
                    dt.Rows.Add(o.Id, o.OrderDate, o.TotalAmount);
                }
                return dt;
            }
        }

        //Автоматическое создание БД

        public void EnsureDatabaseCreated()
        {
            using (var db = new BookStoreContext())
            {
                if (!db.Categories.Any())
                {
                    db.Categories.Add(new Category { Name = "Романы" });
                    db.Categories.Add(new Category { Name = "Поэзия" });
                    db.Categories.Add(new Category { Name = "Драма" });
                    db.Categories.Add(new Category { Name = "Фэнтези" });
                    db.Categories.Add(new Category { Name = "Классика" });
                    db.SaveChanges();
                }
            }
        }
    }
}