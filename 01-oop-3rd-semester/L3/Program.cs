using System.Reflection;

namespace Laba_Three;

//Класс вектор
public class Vector
{
    public List<int> Elements; //Элементы вектора
    public Production Prod = new Production(1, "Юлия"); //Вложенный объект
    public Developer Dev; //Объект вложенного класс 
    
    //Конструкторы
    public Vector()
    {
        Elements = new List<int>();
    }

    public Vector(List<int> eles)
    {
        Elements = eles;
    }
    
    //Индексатор
    public int this[int index]
    {
        get { return Elements[index];}
        set { Elements[index] = value; }
    }
    
    //Перегрузка +
    public static Vector operator +(Vector a, Vector b)
    {
        List<int> res = new List<int>();
        int n = Math.Min(a.Elements.Count, b.Elements.Count);
        for (int i = 0; i < n; i++)
        {
            res.Add(a.Elements[i] + b.Elements[i]);
        }
        int m = Math.Max(a.Elements.Count, b.Elements.Count);
        if (a.Elements.Count > b.Elements.Count)
        {
            for (int i = n; i < m; i++)
            {
                res.Add(a.Elements[i]);
            }
        }
        else if (b.Elements.Count > a.Elements.Count)
        {
            for (int i = n; i < m; i++)
            {
                res.Add(b.Elements[i]);
            }
        }
        return new Vector(res);
    }
    
    //Перегрузка >
    public static bool operator >(Vector a, Vector b)
    {
        int sum1 = 0;
        int sum2 = 0;
        foreach (int x in a.Elements)
        {
            sum1 += x;
        }
        foreach (int x in b.Elements)
        {
            sum2 += x;
        }
        return sum1 > sum2;
    }
    
    //Перегрузка <
    public static bool operator <(Vector a, Vector b)
    {
        int sum1 = 0;
        int sum2 = 0;
        foreach (int x in a.Elements)
        {
            sum1 += x;
        }
        foreach (int x in b.Elements)
        {
            sum2 += x;
        }
        return sum1 < sum2;
    }

    //Копирование ==
    public static Vector operator ==(Vector a, Vector b)
    {
        List<int> copy = new List<int>();
        foreach (int x in b.Elements )
        {
            copy.Add(x);
        }
        return new Vector(copy);
    }
    
    public static Vector operator !=(Vector a, Vector b)
    {
        return a;
    }
    
    //Проверка пустоты
    public static bool operator true(Vector a)
    {
        return a.Elements.Count > 0;
    }
    public static bool operator false(Vector a)
    {
        return a.Elements.Count == 0;
    }
    
    //Показать вектор
    public void ShowVector()
    {
        Console.Write("Вектор: ");
        foreach (int i in Elements)
        {
            Console.Write(i + " ");
        }
        Console.WriteLine();
    }

    //Вложенный класс Developer
    public class Developer
    {
        public string Fio;
        public int Id;
        public string Department;
        public Developer(string fio, int id, string dep)
        {
            Fio = fio;
            Id = id;
            Department = dep;
        }
        public void Show()
        {
            Console.WriteLine("ФИО: " + Fio + ", Id: " + Id + ", Отдел: " + Department);
        }
    }
}

//Класс, чтобы создать вложенный объект Production
public class Production
{
    public int Id;
    public string Name;

    public Production(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public void Show()
    {
        Console.WriteLine("Id: " + Id + ", Название организации: " + Name);
    }
}

//Статический класс для статистики
public static class StatisticOperation
{
    //Сумма элементов вектора
    public static int Sum(Vector v)
    {
        int sum = 0;
        foreach (int x in v.Elements)
            sum += x;
        return sum;
    }

    //Разница между максимальным и минимальным элементами
    public static int Difference(Vector v)
    {
        if (v.Elements.Count == 0) return 0;
        int max = v.Elements[0];
        int min = v.Elements[0];
        foreach (int x in v.Elements )
        {
            if (x > max) max = x;
            if (x < min) min = x;
        }
        return max - min;
    }

    //Количество элементов
    public static int CountElements(Vector v)
    {
        return v.Elements.Count;
    }

    //Усечение строки с начала на опред. количество символов
    public static string StringTruncation(string s, int n)
    {
        if (s.Length <= n) return "";
        return s.Substring(n);
    }
    
    //Удаление положительных элементов вектора
    public static void RemovePos(Vector v)
    {
        for (int i = v.Elements.Count - 1; i >= 0; i--)
        {
            if(v.Elements[i] > 0) v.Elements.RemoveAt(i);
        }
    }
    
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Laba_Num_Three");
        Vector v1 = new Vector(new List<int> { 1, 2, 3 });
        Vector v2 = new Vector(new List<int> { 4, 5, 6, 4 });
        
        //Сложение
        Vector v3 = v1 + v2;
        v3.ShowVector();
        
        //Сравнение
        if (v1 > v2)
        {
            Console.WriteLine("v1 больше v2");
        }
        else
        {
            Console.WriteLine("v1 меньше v2");
        }
        
        //Копирование
        Vector v4 = v2 == v1;
        v4.ShowVector();
        
        //Проверка пустоты
        Vector v5 = new Vector();
        if (v5) 
            Console.WriteLine("v5 не пустой");
        else 
            Console.WriteLine("Пустой");

        //Вложенный объект
        Console.WriteLine(v1.Prod.Id);
        Console.WriteLine(v2.Prod.Name);
        
        //Вложенный класс 
        v1.Dev = new Vector.Developer("Иванов Иван Иванович", 1, "IT");
        v1.Dev.Show();
        
        //Статистический класс
        Console.WriteLine("Сумма элементов вектора v1: " + StatisticOperation.Sum(v1));
        Console.WriteLine("Разница элементов вектора v1: " + StatisticOperation.Difference(v1));
        Console.WriteLine("Количество элементов вектора v1: " + StatisticOperation.CountElements(v1));
        
        //Усечение строки с начала
        string s = "Привет";
        Console.WriteLine(StatisticOperation.StringTruncation(s, 3));
        
        //Удаление положительных элементов вектора
        Vector v7 = new Vector(new List<int> { 1, 2, 3, -5, -8});
        StatisticOperation.RemovePos(v7);
        Console.WriteLine("Вектор v7 после удаление положительных элементов: ");
        v7.ShowVector();
        
    }
}