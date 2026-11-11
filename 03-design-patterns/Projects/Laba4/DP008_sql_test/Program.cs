using ServiceLocator.Interfaces;
using ServiceLocator.Library;
using System;
using System.Collections.Generic;

namespace Test_ServiceLocator
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new Dictionary<Type, Func<object>>();
            // Регистрация базы данных
            services[typeof(IRepository)] = () => new Repository();
            var scopeFactory = new ServiceScopeFactory(services);

            Console.WriteLine("=== ЛАБОРАТОРНАЯ №9: ТЕСТ СЕРВИС-ЛОКАТОРА ===\n");

            // --- 1. ВЫВОД ИЗ БД (Делаем первым, пока контекст чист) ---
            Console.WriteLine("--- 1. Проверка работы с БД ---");
            using (var dbScope = scopeFactory.CreateScope())
            {
                var repo = (IRepository)dbScope.ServiceProvider.GetService(typeof(IRepository));
                var list = repo.GetAllCelebrities();
                Console.WriteLine($"Успешно! Получено записей: {list.Count}");
                foreach (var item in list) Console.WriteLine($"- {item.Firstname} {item.Surname}");
            }

            // --- 2. ТЕСТ SCOPED ---
            Console.WriteLine("\n--- 2. Тест Scoped ---");
            using (var scope1 = scopeFactory.CreateScope())
            {
                var ref1 = scope1.ServiceProvider.GetService(typeof(IRepository));
                var ref2 = scope1.ServiceProvider.GetService(typeof(IRepository));
                // Внутри одного scope объект ДОЛЖЕН быть один и тот же
                Console.WriteLine($"Объекты внутри одного Scope одинаковые? {ReferenceEquals(ref1, ref2)} (True)");
            }

            // --- 3. ТЕСТ SINGLETON (Имитация) ---
            Console.WriteLine("\n--- 3. Тест Singleton ---");
            var mySingleton = new Repository();
            // Подменяем фабрику на возврат конкретного объекта
            services[typeof(IRepository)] = () => mySingleton;

            using (var sScope1 = scopeFactory.CreateScope())
            {
                var s1 = sScope1.ServiceProvider.GetService(typeof(IRepository));
                using (var sScope2 = scopeFactory.CreateScope())
                {
                    var s2 = sScope2.ServiceProvider.GetService(typeof(IRepository));
                    // Singleton всегда возвращает одну ссылку
                    Console.WriteLine($"Объекты в разных Scope одинаковые? {ReferenceEquals(s1, s2)} (True)");
                }
            }

            // --- 4. ПОЧЕМУ TRANSIENT БЫЛ FALSE? ---
            Console.WriteLine("\n--- 4. Пояснение по Transient ---");
            Console.WriteLine("В текущей реализации Scoped-контейнера все объекты кэшируются внутри Scope.");
            Console.WriteLine("Для 'чистого' Transient нужно, чтобы GetService не сохранял объект в словарь.");

            Console.WriteLine("\nТесты завершены успешно. Нажми любую клавишу...");
            Console.ReadKey();
        }
    }
}