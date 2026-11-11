using System;
using System.Data.Entity;
using System.Threading.Tasks;
using BookStore.DAL.Repository;

namespace BookStore.DAL
{
    // Реализация паттерна Unit of Work
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookStoreContext _context;
        private bool _disposed = false;

        private IRepository<Book> _booksRepository;
        private IRepository<Category> _categoriesRepository;
        private IRepository<Order> _ordersRepository;
        private IRepository<User> _usersRepository;
        private IRepository<OrderItem> _orderItemsRepository;

        public UnitOfWork()
        {
            _context = new BookStoreContext();
        }

        public UnitOfWork(BookStoreContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<Book> Books
        {
            get
            {
                if (_booksRepository == null)
                    _booksRepository = new Repository<Book>(_context);
                return _booksRepository;
            }
        }

        public IRepository<Category> Categories
        {
            get
            {
                if (_categoriesRepository == null)
                    _categoriesRepository = new Repository<Category>(_context);
                return _categoriesRepository;
            }
        }

        public IRepository<Order> Orders
        {
            get
            {
                if (_ordersRepository == null)
                    _ordersRepository = new Repository<Order>(_context);
                return _ordersRepository;
            }
        }

        public IRepository<User> Users
        {
            get
            {
                if (_usersRepository == null)
                    _usersRepository = new Repository<User>(_context);
                return _usersRepository;
            }
        }

        public IRepository<OrderItem> OrderItems
        {
            get
            {
                if (_orderItemsRepository == null)
                    _orderItemsRepository = new Repository<OrderItem>(_context);
                return _orderItemsRepository;
            }
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                _context.Database.CurrentTransaction.Commit();
            }
        }

        public void Rollback()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                _context.Database.CurrentTransaction.Rollback();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}