using System.Collections.Concurrent;
using System.Diagnostics;

namespace Laba_Fifteen
{
    internal class Program
    {
        static void MissionOne()
        {
            Console.WriteLine(" ~~~ Задание №1 ~~~ ");

            Task task = Task.Run(() =>
            {
                Console.WriteLine($"Task ID: {Task.CurrentId} начался");
                Stopwatch sw = Stopwatch.StartNew();
                var numbers = FindNumbers(100000);
                sw.Stop();
                Console.WriteLine($"Task ID: {Task.CurrentId} завершился. Status: {Task.CurrentId}");
                Console.WriteLine($"Найдено простых чисел: {numbers.Count}, Время: {sw.ElapsedMilliseconds} ms");
            });

            task.Wait();
        }
        static List<int> FindNumbers(int n)
        {
            bool[] isPrime = new bool[n + 1];
            for (int i = 2; i <= n; i++) isPrime[i] = true;

            for (int i = 2; i * i <= n; i++)
            {
                if (isPrime[i])
                {
                    for (int j = i * i; j <= n; j += i)
                        isPrime[j] = false;
                }
            }

            List<int> primes = new List<int>();
            for (int i = 2; i <= n; i++)
                if (isPrime[i]) primes.Add(i);
            return primes;
        }

        static void MissionTwo()
        {
            Console.WriteLine("\n ~~~ Задание №2 ~~~ ");

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            Task task = Task.Run(() =>
            {
                Console.WriteLine("Отменяемая задача началась");
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(50);
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Задача была отменена!");
                        return;
                    }
                }
                Console.WriteLine("Отменяемая задача завершилась");
            }, token);

            Thread.Sleep(300);
            cts.Cancel();

            task.Wait();
        }

        static void MissionThree()
        {
            Console.WriteLine("\n ~~~ Задание №3 ~~~ ");

            Task<int> t1 = Task.Run(() => 5 + 3);
            Task<int> t2 = Task.Run(() => 10 * 2);
            Task<int> t3 = Task.Run(() => 50 - 7);

            Task<int> finalTask = Task.WhenAll(t1, t2, t3)
                .ContinueWith(results =>
                {
                    int sum = results.Result.Sum();
                    Console.WriteLine($"Сумма результатов трех задач: {sum}");
                    return sum;
                });

            finalTask.Wait();
        }
        static void MissionFour()
        {
            Console.WriteLine("\n ~~~ Задание №4 ~~~ ");

            // ContinueWith
            Task task1 = Task.Run(() => Console.WriteLine("Первая задача завершена"))
                .ContinueWith(t => Console.WriteLine("Задача продолжения через ContinueWith"));
            task1.Wait();

            // GetAwaiter
            Task<string> task2 = Task.Run(() => "Привет из GetAwaiter");
            string result = task2.GetAwaiter().GetResult();
            Console.WriteLine($"Continuation через GetAwaiter(): {result}");
        }

        static void MissionFive()
        {
            Console.WriteLine("\n ~~~ Задание №5 ~~~ ");

            int[] numbers = Enumerable.Range(1, 1_000_000).ToArray();

            Stopwatch swParallel = Stopwatch.StartNew();
            Parallel.For(0, numbers.Length, i => numbers[i] *= 2);
            swParallel.Stop();
            Console.WriteLine($"Параллельный For: {swParallel.ElapsedMilliseconds} ms");

            for (int i = 0; i < numbers.Length; i++) numbers[i] = i + 1;

            Stopwatch swFor = Stopwatch.StartNew();
            for (int i = 0; i < numbers.Length; i++) numbers[i] *= 2;
            swFor.Stop();
            Console.WriteLine($"Обычный For: {swFor.ElapsedMilliseconds} ms");
        }
        static void MissionSix()
        {
            Console.WriteLine("\n ~~~ Задание №6 ~~~ ");

            Parallel.Invoke(
                () => Console.WriteLine("Блок 1 начал работу"),
                () => Console.WriteLine("Блок 2 начал работу"),
                () =>
                {
                    Console.WriteLine("Блок 3 начал работу");
                    var arr = Enumerable.Range(0, 10_000).Reverse().ToArray();
                    Array.Sort(arr);
                    Console.WriteLine("Блок 3 закончил работу");
                }
            );
        }

        static void MissionSeven()
        {
            Console.WriteLine("\n ~~~ Задание №7 ~~~ ");

            BlockingCollection<string> warehouse = new BlockingCollection<string>(10);
            string[] suppliers = { "Samsung", "LG", "Bosch", "Sony", "Panasonic" };
            string[] buyers = { "Покупатель1", "Покупатель2", "Покупатель3", "Покупатель4", "Покупатель5",
                                "Покупатель6", "Покупатель7", "Покупатель8", "Покупатель9", "Покупатель10" };

            Task.Run(() => Suppliers(warehouse, suppliers));
            Task.Run(() => Buyers(warehouse, buyers));

            Thread.Sleep(5000);
        }

        static void Suppliers(BlockingCollection<string> warehouse, string[] suppliers)
        {
            Random rnd = new Random();
            foreach (var s in suppliers)
            {
                Thread.Sleep(rnd.Next(100, 500));
                warehouse.Add(s);
                Console.WriteLine($"Поставщик привез: {s}. На складе: {string.Join(", ", warehouse)}");
            }
        }

        static void Buyers(BlockingCollection<string> warehouse, string[] buyers)
        {
            Random rnd = new Random();
            foreach (var b in buyers)
            {
                Thread.Sleep(rnd.Next(150, 400));
                if (warehouse.TryTake(out string item))
                    Console.WriteLine($"{b} купил: {item}. На складе: {string.Join(", ", warehouse)}");
                else
                    Console.WriteLine($"{b} ничего не купил, склад пуст.");
            }
        }

        static async Task MissionEight()
        {
            Console.WriteLine("\n ~~~ Задание №8 ~~~ ");

            string data = await DownloadDataFakeAsync();
            Console.WriteLine($"Асинхронные данные получены: {data}");
        }

        static async Task<string> DownloadDataFakeAsync()
        {
            await Task.Delay(1000);
            return "Содержимое файла или сайта";
        }

        static void Main(string[] args)
        {
            MissionOne();
            MissionTwo();
            MissionThree();
            MissionFour();
            MissionFive();
            MissionSix();
            MissionSeven();
            MissionEight().Wait();
        }
    }
}
