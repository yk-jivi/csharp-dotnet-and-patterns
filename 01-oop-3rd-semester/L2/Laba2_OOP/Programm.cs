using System;
using System.Collections.Generic;
using System.Linq;

namespace Laba2_OOP
{
    //1
    //1.8 partial class - разделение класса на части
    public partial class Set
    {
        private int x, y, z;
        private List<int> elements = new List<int>();

        //№1.8
        private static int objCount = 0;
        public static void Info()
        {
            Console.WriteLine($"Класс Set: создано объектов — {objCount}");
        }

        //№1.4
        //Создание поля только для чтения
        public readonly int ID;
        private int GenerateID()
        {
            return (x * y * z);
        }

        //№1.1
        //Создание констукторов
        //C параметрами
        public Set()
        {
            x = 3; y = 4; z = 5;
            elements.AddRange(new[] { x, y, z });
            ID = GenerateID();
            objCount++;
        }

        public Set(int a, int b)
        {
            x = a; y = b; z = 6;
            elements.AddRange(new[] { x, y, z });
            ID = GenerateID();
            objCount++;
        }

        //С параметрами по умолчанию
        public Set(int a = 10, int b = 40, int c = 100)
        {
            x = a; y = b; z = c;
            elements.AddRange(new[] { x, y, z });
            ID = GenerateID();
            objCount++;
        }

        //№1.2
        //Статический конструктор
        public static int d;
        static Set()
        {
            d = 100;
            Console.WriteLine("Задание №1.2 выполнено!");
        }

        //№1.3
        //Закрытый конструктор
        private Set(string message)
        {
            Console.WriteLine($"Закрытый конструктор: {message}");
            x = 0; y = 0; z = 0;
            elements.AddRange(new[] { x, y, z });
            ID = GenerateID();
            objCount++;
        }
        //Вызывать/создавать можно только через Create()
        public static Set Create()
        {
            return new Set("Создан через Create()");
        }

        //№1.5
        //Поле-константа
        public const int age = 18;
        public void Print()
        {
            Console.WriteLine($"Set: x={x}, y={y}, z={z}, ID={ID}, age={age}");
            Console.WriteLine($"Элементы: {{ {string.Join(", ", elements)} }}");
        }

        //№1.6
        //Свойства
        public int X 
        { 
            get 
            { 
                return x; 
            } 
            set 
            { 
                x = value; 
            } 
        }
        public int Y 
        { 
            get 
            { 
                return y; 
            } 
            set 
            { 
                y = value; 
            } 
        }
        public int Z 
        { 
            get 
            { 
                return z; 
            } 
            private set 
            { 
                z = value; 
            } 
        }

        //№1.7
        //ref и out параметры
        public void Calc(ref int a, out int result)
        {
            int sum = x + y + z;
            a += 1;
            result = sum * a;
        }

        //Методы множества
        public void Add(int value)
        {
            if (!elements.Contains(value))
                elements.Add(value);
        }
        public void Remove(int value)
        {
            elements.Remove(value);
        }
        public int Count()
        {
            return elements.Count;
        }
        public int Sum()
        {
            return elements.Sum();
        }
    }

    public partial class Set
    {
        public bool HasNegative()
        {
            return elements.Any(element => element < 0);
        }

        //Переопределение Object
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Set other = (Set)obj;
            return x == other.x && y == other.y && z == other.z && ID == other.ID;
        }
        public override int GetHashCode()
        {
            int hash = 18;
            hash = hash * 13 + x.GetHashCode();
            hash = hash * 13 + y.GetHashCode();
            hash = hash * 13 + z.GetHashCode();
            hash = hash * 13 + ID.GetHashCode();
            return hash;
        }
        public override string ToString()
        {
            return $"Set: x={x}, y={y}, z={z}, ID={ID}, Count={Count()}, Sum={Sum()}";
        }

    }
    internal class Programm
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            //Создание объектов
            Set s1 = new Set();
            Set s2 = new Set(5, -3);
            Set s3 = Set.Create();

            s1.Add(10);
            s2.Add(-7);
            s3.Add(0);

            s1.Print();
            s2.Print();
            s3.Print();

            //Сравнение и тип
            Console.WriteLine($"s1.Equals(s2): {s1.Equals(s2)}");
            Console.WriteLine($"Тип s1: {s1.GetType()}");
            Console.WriteLine($"ToString s1: {s1}");

            //ref и out
            int a = 2;
            int result;
            s1.Calc(ref a, out result);
            Console.WriteLine($"Calc: a={a}, result={result}");

            //Статическая информация
            Set.Info();

            //Массив объектов
            Set[] sets = new Set[5];
            sets[0] = new Set(1, 2);
            sets[1] = new Set(-5, 3);
            sets[2] = new Set(0, 0, -1);
            sets[3] = new Set(7, 8);
            sets[4] = new Set(2, -2, 4);

            foreach (var s in sets)
            {
                s.Add(s.X);
                s.Add(s.Y);
                s.Add(s.Z);
            }

            //a)Множества с наименьшей и наибольшей суммой
            var minSet = sets.OrderBy(s => s.Sum()).First();
            var maxSet = sets.OrderByDescending(s => s.Sum()).First();

            Console.WriteLine("\nМножество с наименьшей суммой:");
            minSet.Print();

            Console.WriteLine("\nМножество с наибольшей суммой:");
            maxSet.Print();

            //b)Множества с отрицательными элементами
            Console.WriteLine("\nМножества с отрицательными элементами:");
            foreach (var s in sets.Where(s => s.HasNegative()))
                s.Print();

            //Анонимный тип
            var anonym = new { X = s1.X, Y = s1.Y, Z = s1.Z, Sum = s1.Sum() };
            Console.WriteLine($"\nАнонимный тип: X={anonym.X}, Y={anonym.Y}, Z={anonym.Z}, Sum={anonym.Sum}");
            Class1 onb = new Class1();
            onb.Hell();
        }
    }
}
