using System;
using System.Collections.Generic;
using System.Linq;

namespace Laba_Ten
{
    public class Set
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
        public bool Contains(int value)
        {
            return elements.Contains(value);
        }
        public bool ContainsFirst(int value)
        {
            if (elements.Contains(value)) 
                return true;
            else
                return false;
        }
        public int FirstElement()
        {
            return elements[0];
        }
    }
}
