using System;
using System.Collections.Generic;
using DP008_ICelebrity.lib;
using DP008_sql.lib;

namespace DP008_sql_test
{
    class Program
    {
        static void Main(string[] args)
        {

            using (ICelebrity<Celebrity> repo = Repository.Create())
            {
                Console.WriteLine("--------- GetAllCelebrities ---------------------------");
                {
                    List<Celebrity> celebrities = repo.GetAllCelebrities();
                    celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath} "));
                }

                Console.WriteLine("--------- AddCelebrity --------------------------------");
                {
                    // В SQL Id генерируется автоматически, передаем 0
                    repo.AddCelebrity(new Celebrity { Id = 0, Firstname = "Marlin", Surname = "Minsky", PhotoPath = "/Photo/Minsky.jpg" });
                    List<Celebrity> celebrities = repo.GetAllCelebrities();
                    celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath} "));
                }

                Console.WriteLine("--------- GetCelebrityIdByName, UpdCelebrity ----------");
                {
                    int id = repo.GetCelebrityIdByName("Marlin");
                    if (id >= 0) repo.UpdCelebrity(id, new Celebrity { Id = 0, Firstname = "Marvin", Surname = "Minsky", PhotoPath = "/Photo/Minsky.jpg" });
                    List<Celebrity> celebrities = repo.GetAllCelebrities();
                    celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath} "));
                }

                Console.WriteLine("--------- GetCelebrityIdByName, DelCelebrity ----------");
                {
                    int id = repo.GetCelebrityIdByName("Minsky");
                    if (id >= 0) repo.DelCelebrity(id);
                    List<Celebrity> celebrities = repo.GetAllCelebrities();
                    celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath} "));
                }

                Console.WriteLine("--------- AddCelebrityAndGetId ------------------------");
                {
                    int id = repo.AddCelebrityAndGetId(new Celebrity { Id = 0, Firstname = "Marvin", Surname = "Minsky", PhotoPath = "/Photo/Minsky.jpg" });
                    Console.WriteLine($"Minsky Id = {id}");
                    List<Celebrity> celebrities = repo.GetAllCelebrities();
                    celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath} "));
                }

                Console.WriteLine($"SaveChanges = {repo.SaveChanges()}");
            }

            // 4. Вторая проверка — убеждаемся, что данные сохранились в БД
            Console.WriteLine("--------- Final Verification from DB ------------------");
            using (ICelebrity<Celebrity> repo = Repository.Create())
            {
                List<Celebrity> celebrities = repo.GetAllCelebrities();
                celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath} "));
            }

            Console.ReadKey();
        }
    }
}