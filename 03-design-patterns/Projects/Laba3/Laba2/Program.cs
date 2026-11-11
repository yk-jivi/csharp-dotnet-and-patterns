using System;
using System.Collections.Generic;
using DP007_ICelebrity.lib;
using DP007_json.lib;

namespace DP007_json_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Repository.JSONFileName = "Celebrities.json";

            using (ICelebrity<Celebrity> repo = Repository.Create("Celebrities"))
            {
                Console.WriteLine("--------- GetAllCelebrities ---------------------------");
                {
                    List<Celebrity> celebrities = repo.GetAllCelebrities();
                    celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath} "));
                }

                Console.WriteLine("--------- AddCelebrity --------------------------------");
                {
                    repo.AddCelebrity(new Celebrity(0, "Marlin", "Minsky", "/Photo/Minsky.jpg"));
                    List<Celebrity> celebrities = repo.GetAllCelebrities();
                    celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath} "));
                }

                Console.WriteLine("--------- GetCelebrityIdByName, UpdCelebrity ----------");
                {
                    int id = repo.GetCelebrityIdByName("Marlin");
                    if (id >= 0) repo.UpdCelebrity(id, new Celebrity(0, "Marvin", "Minsky", "/Photo/Minsky.jpg"));
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
                    int id = repo.AddCelebrityAndGetId(new Celebrity(0, "Marvin", "Minsky", "/Photo/Minsky.jpg"));
                    Console.WriteLine($"Minsky Id = {id}");
                    List<Celebrity> celebrities = repo.GetAllCelebrities();
                    celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath} "));
                }

                Console.WriteLine($"SaveChanges = {repo.SaveChanges()}");
            }

            using (ICelebrity<Celebrity> repo = Repository.Create("Celebrities"))
            {
                List<Celebrity> celebrities = repo.GetAllCelebrities();
                celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath} "));
            }

            Console.ReadKey();
        }
    }
}