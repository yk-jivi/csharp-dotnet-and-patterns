using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DP007_ICelebrity.lib;

namespace DP007_json.lib
{
    //Репозиторий для работы с данными знаменитостей в JSON-файле
    //Реализует интерфейс ICelebrity<Celebrity>
    public class Repository : ICelebrity<Celebrity>
    {
        public static string JSONFileName { get; set; } = "Celebrities.json"; //имя JSON-файла с данными
        private List<Celebrity> _celebrities; //список знаменитостей в памяти
        private string _fullPath; //полный путь к JSON-файлу
        private int _changeCount = 0; //счетчик изменений для сохранения

        //Приватный конструктор репозитория
        private Repository(string directory)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory); //создать директорию если не существует
            _fullPath = Path.Combine(directory, JSONFileName); //формируем полный путь к файлу

            if (File.Exists(_fullPath)) //если файл существует
            {
                string json = File.ReadAllText(_fullPath); //читаем JSON из файла
                _celebrities = JsonSerializer.Deserialize<List<Celebrity>>(json) ?? new List<Celebrity>(); //десериализуем список
            }
            else
            {
                _celebrities = new List<Celebrity>(); //создаем пустой список
            }
        }
        
        //Метод для создания экземпляра репозитория
        public static ICelebrity<Celebrity> Create(string directory)
        {
            return new Repository(directory);
        }

        //Получить всех знаменитостей
        public List<Celebrity> GetAllCelebrities() => _celebrities.ToList(); //возвращаем копию списка

        //Получить знаменитость по ID
        public Celebrity? GetCelebrityById(int Id) => _celebrities.FirstOrDefault(c => c.Id == Id); //поиск по ID

        //Добавить знаменитость
        public bool AddCelebrity(Celebrity celebrity)
        {
            celebrity.Id = _celebrities.Any() ? _celebrities.Max(c => c.Id) + 1 : 1; //автоматически назначаем новый ID
            _celebrities.Add(celebrity); //добавляем в список
            _changeCount++; //увеличиваем счетчик изменений
            return true; //возвращаем успех
        }

        //Добавить знаменитость и получить её ID
        public int AddCelebrityAndGetId(Celebrity celebrity)
        {
            AddCelebrity(celebrity); //вызываем метод добавления
            return celebrity.Id; //возвращаем ID добавленной знаменитости
        }

        //Удалить знаменитость
        public bool DelCelebrity(int id)
        {
            var item = GetCelebrityById(id); //находим знаменитость по ID
            if (item != null) //если найдена
            {
                _celebrities.Remove(item); //удаляем из списка
                _changeCount++; //увеличиваем счетчик изменений
                return true; //возвращаем успех
            }
            return false; //не найдена
        }

        //Обновить знаменитость
        public bool UpdCelebrity(int id, Celebrity celebrity)
        {
            var index = _celebrities.FindIndex(c => c.Id == id); //находим индекс по ID
            if (index != -1) //если найдена
            {
                celebrity.Id = id; //сохраняем оригинальный ID
                _celebrities[index] = celebrity; //обновляем элемент в списке
                _changeCount++; //увеличиваем счетчик изменений
                return true; //возвращаем успех
            }
            return false; //не найдена
        }

        //Получить знаменитость по вводу имени
        public int GetCelebrityIdByName(string name)
        {
            var celebrity = _celebrities.FirstOrDefault(c =>
                c.Firstname.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                c.Surname.Contains(name, StringComparison.OrdinalIgnoreCase)); //поиск по имени или фамилии
            return celebrity?.Id ?? -1; //возвращаем ID или -1 если не найдена
        }

        //Сохранить изменения = количество изменений
        public int SaveChanges()
        {
            string json = JsonSerializer.Serialize(_celebrities, new JsonSerializerOptions { WriteIndented = true }); //сериализуем в JSON с отступами
            File.WriteAllText(_fullPath, json); //записываем в файл
            int count = _changeCount; //сохраняем количество изменений
            _changeCount = 0; //сбрасываем счетчик
            return count; //возвращаем количество сохраненных изменений
        }

        //Освобождение ресурсов
        public void Dispose() { } //пустая реализация для интерфейса IDisposable
    }
}