using System;
using System.Threading.Tasks;
using BookStore.DAL.Repository;

namespace BookStore.DAL
{
    // Интерфейс паттерна Unit of Work
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Book> Books { get; }
        IRepository<Category> Categories { get; }
        IRepository<Order> Orders { get; }
        IRepository<User> Users { get; }
        IRepository<OrderItem> OrderItems { get; }

        int Save();
        Task<int> SaveAsync();

        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}