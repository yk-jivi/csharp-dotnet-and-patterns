using System;
using System.Data.Entity;
using System.Linq;

namespace BookStore.DAL.Repository
{
    // Реализация паттерна Repository
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly BookStoreContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(BookStoreContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
    }
}