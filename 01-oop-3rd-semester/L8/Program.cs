using System;
using System.Text.RegularExpressions;

namespace Laba_Eight;

//Задание №1
//Делегат для событий
public delegate void UserEvent();

//Класс Пользователь
public class User
{
    public string Name { get; set; }
    public int Version { get; set; }
    
    //События
    public event UserEvent Upgrade;
    public event UserEvent Work;

    public User(string name, int version)
    {
        Name = name;
        Version = version;
    }

    public void DoUpgrade()
    {
        Console.WriteLine($"Пользователь {Name}: вызов события Upgrade");
        Upgrade?.Invoke();
    }
    
    public void DoWork()
    {
        Console.WriteLine($"Пользователь {Name}: вызов события Work");
        Work?.Invoke();
    }

    public void UpdateVersion()
    {
        Version++;
        Console.WriteLine($"Пользователь {Name}: обновил версию до {Version}");
    }
    
    public void ShowWorkMessage()
    {
        Console.WriteLine($"Пользователь {Name} выполняет работу");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("---Задание №1---");
        User u1 = new User("Джон", 1);
        User u2 = new User("Саймон", 2);
        User u3 = new User("Моно", 3);

        u1.Upgrade += u1.UpdateVersion;
        u1.Work += u1.ShowWorkMessage;
        
        u2.Upgrade += u2.UpdateVersion;
        u2.Upgrade += () => Console.WriteLine("Доп. действие при апгрейде у Саймона");
        u2.Work += u2.ShowWorkMessage;
        
        u3.Work += () => Console.WriteLine("Моно работает сверхурочно");
        
        //Вызов событий
        u1.DoUpgrade();
        u1.DoWork();
        
        u2.DoUpgrade();
        u2.DoWork();

        u3.DoWork();
        
        Console.WriteLine($"\nТекущие версии пользователей:");
        Console.WriteLine($"{u1.Name}: {u1.Version}");
        Console.WriteLine($"{u2.Name}: {u2.Version}");
        Console.WriteLine($"{u3.Name}: {u3.Version}");
        
        Console.WriteLine("---Задание №2---");
        string text = "Привет, мир";
        Console.WriteLine($"Исходный текст: {text}");
        
        //Func — удалить знаки препинания
        Func<string, string> removePunct = s => Regex.Replace(s, @"[^\w\s]", "");

        //Func — удалить лишние пробелы
        Func<string, string> trimSpaces = s => Regex.Replace(s, @"\s+", " ").Trim();

        //Func — перевести в верхний регистр
        Func<string, string> toUpper = s => s.ToUpper();

        //Func — добавить восклицательный знак
        Func<string, string> addSign = s => s + "!";

        //Predicate — проверить наличие слова HELLO
        Predicate<string> containsHello = s => s.Contains("HELLO");
        
        Action<string> printHello = s => Console.WriteLine($"HELLO");    

        //Последовательная обработка строки
        text = removePunct(text);
        text = trimSpaces(text);
        text = toUpper(text);
        text = addSign(text);

        Console.WriteLine($"Обработанный текст: {text}");

        //Проверка через Predicate
        if (containsHello(text))
            Console.WriteLine("В тексте есть слово HELLO");
        else
            Console.WriteLine("Слова HELLO нет");
    }
}