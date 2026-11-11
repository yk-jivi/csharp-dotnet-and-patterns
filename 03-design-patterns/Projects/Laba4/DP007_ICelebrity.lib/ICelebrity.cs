using System;
using System.Collections.Generic;

namespace ServiceLocator.Interfaces
{
    public interface ICelebrity<T> : IDisposable
    {
        List<T> GetAllCelebrities();
        T? GetCelebrityById(int Id);
        bool DelCelebrity(int id);
        bool AddCelebrity(T celebrity);
        int AddCelebrityAndGetId(T celebrity);
        bool UpdCelebrity(int id, T celebrity);
        int GetCelebrityIdByName(string name);
        int SaveChanges();
    }
    public interface ILifeevent : IDisposable { }
    public interface IRepository : ICelebrity<Celebrity>, ILifeevent { }
}