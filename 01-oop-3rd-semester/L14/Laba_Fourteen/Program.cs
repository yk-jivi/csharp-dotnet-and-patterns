using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

namespace Laba_Fourteen
{
    public class Program
    {
        static void TaskOne()
        {
            Console.WriteLine("~~~ Задание №1 ~~~");
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    Console.WriteLine(new string('-', 40));
                    Console.WriteLine($"ID: {process.Id}");
                    Console.WriteLine($"Имя: {process.ProcessName}");
                    Console.WriteLine($"Приориет: {process.BasePriority}");
                    Console.WriteLine($"Время запуска: {process.StartTime}");
                    Console.WriteLine($"Статус: {process.Responding}");
                    Console.WriteLine($"Время использования процессора: {process.TotalProcessorTime}");
                    Console.WriteLine(new string('-', 40));
                }
                catch (Exception)
                {
                    Console.WriteLine($"ID: {process.Id}, Имя: {process.ProcessName} — нет доступа к информации");
                    Console.WriteLine(new string('-', 40));
                }
            }
            Console.WriteLine();Console.WriteLine();
        }


        static void TaskTwo()
        {
            Console.WriteLine("~~~ Задание №2 ~~~");

            AppDomain domain = AppDomain.CurrentDomain;
            Console.WriteLine($"Имя домена: {domain.FriendlyName}");

            Console.WriteLine("Сборки в домене: ");
            foreach(var a in domain.GetAssemblies())
                Console.WriteLine(a.FullName);

            //Эта штука устарела

            /*
                AppDomain newDomain = AppDomain.CreateDomain("NewDomain"); 

                newDomain.Load("MyDomain.xml"); 

                AppDomain.Unload(newDomain);
            */

            var loadContext = new AssemblyLoadContext("MyLoadContext", isCollectible: true);

            try
            {
                string path = "Laba_Fourteen.dll";

                Assembly assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(path));
                Console.WriteLine();
                Console.WriteLine($"Сборка {assembly.FullName} успешно загружена");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка загрузки: " + ex.Message);
            }
            loadContext.Unload();
            Console.WriteLine("Контекст выгружен");

            Console.WriteLine(); Console.WriteLine();
        }


        static Thread? thread;
        static bool paused = false;
        static void TaskThree()
        {
            Console.WriteLine("~~~ Задание №3 ~~~");
            Console.WriteLine("Введите n: ");
            int n = int.Parse(Console.ReadLine() ?? "0");
            Console.WriteLine("--- После завершения потока нажмите Enter  ---");

            thread = new Thread(() => WriteNumbers(n));
            thread.Name = "NumberWriter";
            thread.Priority = ThreadPriority.Normal;

            thread.Start();

            while (thread.IsAlive)
            {
                Console.WriteLine("Выберите действие для потока: ");
                Console.WriteLine(" 1 - пауза");
                Console.WriteLine(" 2 - продолжить");
                Console.WriteLine(" 3 - статус");

                string? cmd = Console.ReadLine();

                if (cmd == "1")
                    paused = true;
                if (cmd == "2")
                    paused = false;
                if (cmd == "3")
                {
                    Console.WriteLine($"Статус: {thread.ThreadState}");
                    Console.WriteLine($"Имя: {thread.Name}");
                    Console.WriteLine($"Приоритет: {thread.Priority}");
                    Console.WriteLine($"ID: {thread.ManagedThreadId}");
                }
            }
        }
        static void WriteNumbers(int n)
        {
            using (StreamWriter wn = new StreamWriter("numbers.txt"))
            {
                for(int i = 0; i <= n; i++)
                {
                    while (paused)
                        Thread.Sleep(1000);

                    Console.WriteLine(i);
                    wn.WriteLine(i);

                    Thread.Sleep(1000);
                }
            }
        }


        static object locker = new object();
        static void TaskFour()
        {
            Console.WriteLine("~~~ Задание №4 ~~~");

            while (true)
            {
                Console.WriteLine("\nВыберите вариант:");
                Console.WriteLine("1 — сначала все чётные, потом все нечётные");
                Console.WriteLine("2 — чередование: одно чётное, одно нечётное");
                Console.WriteLine("0 — выход");

                string? choice = Console.ReadLine();

                if (choice == "0")
                {
                    Console.WriteLine("Выход из меню");
                    Console.WriteLine();
                    break;
                }

                Console.Write("Введите n: ");
                int n = int.Parse(Console.ReadLine() ?? "0");

                if (choice == "1")
                {
                    Thread evenThread = new Thread(() => WriteEven(n));
                    Thread oddThread = new Thread(() => WriteOdd(n));

                    evenThread.Name = "EvenThread";
                    oddThread.Name = "OddThread";

                    evenThread.Priority = ThreadPriority.Highest;
                    oddThread.Priority = ThreadPriority.Lowest;

                    evenThread.Start();
                    oddThread.Start();

                    evenThread.Join();
                    oddThread.Join();

                    Console.WriteLine("Вариант 1 завершён: сначала все чётные, потом все нечётные.");
                }
                else if (choice == "2")
                {
                    Thread evenSeq = new Thread(() => WriteEvenSequential(n));
                    Thread oddSeq = new Thread(() => WriteOddSequential(n));

                    evenSeq.Start();
                    oddSeq.Start();

                    evenSeq.Join();
                    oddSeq.Join();

                    Console.WriteLine("Вариант 2 завершён: числа выводились по очереди");
                }
                else
                {
                    Console.WriteLine("Неверный выбор. Попробуйте ещё раз");
                }
            }
        }
        static void WriteEven(int n)
        {
            lock(locker)
            {
                using (StreamWriter we = new StreamWriter("even_numbers.txt"))
                {
                    for (int i = 0; i <= n; i += 2)
                    {
                        Console.WriteLine($"Чётное {i}");
                        we.WriteLine(i);
                        Thread.Sleep(200);
                    }
                }
            }
        }
        static void WriteOdd(int n)
        {
            lock (locker)
            {
                using (StreamWriter wo = new StreamWriter("odd_numbers.txt"))
                {
                    for (int i = 1; i <= n; i += 2)
                    {
                        Console.WriteLine($"Нечётное {i}");
                        wo.WriteLine(i);
                        Thread.Sleep(300);
                    }
                }
            }
        }
        static readonly AutoResetEvent evenTurn = new AutoResetEvent(true);
        static readonly AutoResetEvent oddTurn = new AutoResetEvent(false);
        static void WriteEvenSequential(int n)
        {
            using (StreamWriter w = new StreamWriter("even_seq.txt"))
            {
                for (int i = 0; i <= n; i += 2)
                {
                    evenTurn.WaitOne();

                    Console.WriteLine($"Чётное -> {i}");
                    w.WriteLine(i);

                    Thread.Sleep(200);

                    oddTurn.Set();
                }
            }
        }
        static void WriteOddSequential(int n)
        {
            using (StreamWriter w = new StreamWriter("odd_seq.txt"))
            {
                for (int i = 1; i <= n; i += 2)
                {
                    oddTurn.WaitOne();

                    Console.WriteLine($"Нечётное -> {i}");
                    w.WriteLine(i);

                    Thread.Sleep(300);

                    evenTurn.Set();
                }
            }
        }


        static Timer? timer;
        static void TaskFive()
        {
            Console.WriteLine(" ~~~ Задание №5 ~~~ ");
            timer = new Timer(PrintMessage, null, 0, 2000);
            Console.WriteLine("Нажмите Enter, чтобы остановить таймер...");
            Console.ReadLine();

            timer.Dispose();
            Console.WriteLine("Таймер остановлен.");
        }
        static void PrintMessage(object? state)
        {
            Console.WriteLine($"Повторяющееся действие: {DateTime.Now}");
        }


        static void Main(string[] args)
        {
            TaskOne();
            TaskTwo();
            TaskThree();
            TaskFour();
            TaskFive();
        }
    }
}
