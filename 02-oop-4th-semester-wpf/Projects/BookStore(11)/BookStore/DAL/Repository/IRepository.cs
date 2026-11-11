using System.Linq;

namespace BookStore.DAL.Repository
{
    // Обобщенный интерфейс репозитория — паттерн Repository
    public interface IRepository<T> where T : class
    {
        // Основной метод для LINQ-запросов
        IQueryable<T> Query();

        // Добавить запись
        void Add(T entity);

        // Обновить запись
        void Update(T entity);

        // Удалить запись по ID
        void Delete(int id);
    }
}