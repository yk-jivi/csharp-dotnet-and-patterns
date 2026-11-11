using System;
using System.Collections.Generic;
using System.Linq;
using DP008_ICelebrity.lib;
using Microsoft.EntityFrameworkCore;

namespace DP008_sql.lib
{
    // Репозиторий для работы с данными знаменитостей в SQL базе данных
    // Реализует интерфейс ICelebrity<Celebrity>
    public class Repository : ICelebrity<Celebrity>
    {
        private readonly CelebrityContext _context; // Контекст базы данных

        // Приватный конструктор репозитория
        private Repository()
        {
            _context = new CelebrityContext();
            // EnsureCreated создаст базу и таблицы, если они отсутствуют
            _context.Database.EnsureCreated();
        }

        // Метод для создания экземпляра репозитория
        public static ICelebrity<Celebrity> Create()
        {
            return new Repository();
        }

        // Получить всех знаменитостей из БД
        public List<Celebrity> GetAllCelebrities() => _context.Celebrities.ToList();

        // Получить знаменитость по ID
        public Celebrity? GetCelebrityById(int Id) => _context.Celebrities.Find(Id);

        // Добавить знаменитость и сразу сохранить в БД
        public bool AddCelebrity(Celebrity celebrity)
        {
            _context.Celebrities.Add(celebrity);
            _context.SaveChanges(); // Синхронизируем с базой немедленно
            return true;
        }

        // Добавить знаменитость и получить её ID из базы
        public int AddCelebrityAndGetId(Celebrity celebrity)
        {
            _context.Celebrities.Add(celebrity);
            _context.SaveChanges(); // Сохраняем, чтобы БД сгенерировала Id
            return celebrity.Id;
        }

        // Удалить знаменитость
        public bool DelCelebrity(int id)
        {
            var item = GetCelebrityById(id);
            if (item != null)
            {
                _context.Celebrities.Remove(item);
                _context.SaveChanges(); // Удаляем из БД
                return true;
            }
            return false;
        }

        // Обновить данные знаменитости
        public bool UpdCelebrity(int id, Celebrity celebrity)
        {
            var item = GetCelebrityById(id);
            if (item != null)
            {
                // Обновляем поля существующего объекта, за которым следит EF
                item.Firstname = celebrity.Firstname;
                item.Surname = celebrity.Surname;
                item.PhotoPath = celebrity.PhotoPath;
                _context.SaveChanges(); // Фиксируем изменения в БД
                return true;
            }
            return false;
        }

        // Поиск ID по имени или фамилии
        public int GetCelebrityIdByName(string name)
        {
            var celebrity = _context.Celebrities.FirstOrDefault(c =>
                c.Firstname.Contains(name) || c.Surname.Contains(name));
            return celebrity?.Id ?? -1;
        }

        // Сохранить все накопленные изменения (для совместимости с интерфейсом)
        public int SaveChanges()
        {
            // Если мы сохраняем в каждом методе, здесь обычно будет 0
            return _context.SaveChanges();
        }

        // Освобождение контекста БД
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}