using System;
using System.Collections.Generic;

namespace DP008_ICelebrity.lib
{
    public interface ICelebrity<T> : IDisposable
    {
        List<T> GetAllCelebrities(); //получить всех знаменитостей
        T? GetCelebrityById(int Id); //получить знаменитость по ID
        bool DelCelebrity(int id); //удалить знаменитость
        bool AddCelebrity(T celebrity); //добавить знаменитость
        int AddCelebrityAndGetId(T celebrity); //добавить знаменитость и получить её ID
        bool UpdCelebrity(int id, T celebrity); //обновить знаменитость
        int GetCelebrityIdByName(string name); //получить знаменитость по вводу имени
        int SaveChanges(); //сохранить изменения = количество изменений
    }
}