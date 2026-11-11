using System;
using System.Collections.Generic;
using System.Linq;
using ServiceLocator.Library;
using ServiceLocator.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ServiceLocator.Library
{
    // Реализуем IRepository. Обрати внимание, что IRepository 
    // должен быть связан с DP008_sql.lib.Celebrity в самом интерфейсе.
    public class Repository : IRepository
    {
        private readonly CelebrityContext _context;

        public Repository()
        {
            _context = new CelebrityContext();
            _context.Database.EnsureCreated();
        }

        // Возвращаем список именно тех Celebrity, которые определены в этом проекте
        public List<ServiceLocator.Interfaces.Celebrity> GetAllCelebrities()
        {
            return _context.Celebrities.ToList();
        }

        public ServiceLocator.Interfaces.Celebrity? GetCelebrityById(int Id)
        {
            return _context.Celebrities.Find(Id);
        }

        public bool AddCelebrity(ServiceLocator.Interfaces.Celebrity celebrity)
        {
            _context.Celebrities.Add(celebrity);
            _context.SaveChanges();
            return true;
        }

        public int AddCelebrityAndGetId(ServiceLocator.Interfaces.Celebrity celebrity)
        {
            _context.Celebrities.Add(celebrity);
            _context.SaveChanges();
            return celebrity.Id;
        }

        public bool DelCelebrity(int id)
        {
            var item = GetCelebrityById(id);
            if (item != null)
            {
                _context.Celebrities.Remove(item);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdCelebrity(int id, ServiceLocator.Interfaces.Celebrity celebrity)
        {
            var item = GetCelebrityById(id);
            if (item != null)
            {
                item.Firstname = celebrity.Firstname;
                item.Surname = celebrity.Surname;
                item.PhotoPath = celebrity.PhotoPath;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public int GetCelebrityIdByName(string name)
        {
            var celebrity = _context.Celebrities.FirstOrDefault(c =>
                c.Firstname.Contains(name) || c.Surname.Contains(name));
            return celebrity?.Id ?? -1;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}