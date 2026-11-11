using System;

namespace Laba_Four;

//Перечисление
enum EarthType
{
    Continent,
    State,
    Island,
    Sea,
    Water,
    Land
}

//Структура
struct Coordinates
{
    public double X;
    public double Y;

    public Coordinates(double x, double y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"Координаты: ({X},{Y})";
    }
}

interface ICloneable
{
    bool DoClone();
}

//Абстрактный класс
abstract partial class EarthPart
{
    public string Name;
    
    //Конструктор, который принимает строку name и записывает в поле Name
    public EarthPart(string name)
    {
        Name = name;
    }

    //Метод для переопределения его другими классами-наследниками
    public abstract void Info();
    
    //-||-
    public abstract bool DoClone();
    
    //Виртуальный метод
    public virtual void Display()
    {
        Console.WriteLine("Display" + ToString());
    }
}

// Вода(реализует интерфейс и наследует абстрактный метод)
class Water : EarthPart, ICloneable
{
    public Water(string name) : base(name) { }
    
    //Реализация абстрактного метода Info()
    public override void Info()
    {
        Console.WriteLine("Это вода: " + Name);
    }

    //Реализация абстрактного метода DoClone() через класс EarthPart
    public override bool DoClone()
    {
        Console.WriteLine("Клон воды (через абстрактный класс)");
        return true;
    }
    
    //Явная реализация интерфейса
    bool ICloneable.DoClone()
    {
        Console.WriteLine("Клон воды (через интерфейс)");
        return false;
    }
    
    public override string ToString()
    {
        return "Вода (Water), название: " + Name;
    }
}

//Sealed означает, что класс нельзя наследовать дальше
//Море
sealed class Sea : Water
{
    public Sea(string name) :  base(name) { }

    public override void Info()
    {
        Console.WriteLine("Это море: " + Name);
    }
    public override string ToString()
    {
        return "Море (Sea), название: " + Name;
    }
}

// Суша - базовый тип для континента, государства и острова

class Land : EarthPart
{
    public Land (string name) : base(name) { }
    public override void Info()
    {
        Console.WriteLine("Это суша: " + Name);
    }

    public override bool DoClone()
    {
        Console.WriteLine("Клон суши");
        return true;
    }
    
    //Переопределение методов Object
    public override string ToString()
    {
        return "Суша (Land), название: " + Name;
    }

    public override bool Equals(object obj)
    {
        if (obj is Land other)
        {
            return this.Name == other.Name;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Name != null ? Name.GetHashCode() : 0;
    }
}


//Государство
class State : Land
{
    public State (string name) : base(name) { }

    public override void Info()
    {
        Console.WriteLine("Это государство: " + Name);
    }

    public override bool DoClone()
    {
        Console.WriteLine("Клон государства");
        return true;
    }
    
    //Переопределение виртуального метода
    public override void Display()
    {
        Console.WriteLine("Display (State): " + ToString());
    }
    public override string ToString()
    {
        return "Государство (State), Название: " + Name;
    }
}
//Континент
class Continent : Land
{
    public State[] States;

    public Continent(string name, State[] states = null) : base(name)
    {
        States = states;
    }

    public override void Info()
    {
        Console.WriteLine("Это континент: " + Name);
        if (States != null && States.Length > 0)
        {
            Console.WriteLine("Содержит государства: ");
            for (int i = 0; i < States.Length; i++)
            {
                Console.WriteLine(" " + States[i].Name);
            }
        }
    }

    public override bool DoClone()
    {
        Console.WriteLine("Клон континента");
        return true;
    }

    public override string ToString()
    {
        string StatesCount = (States != null && States.Length > 0) ? $" Количество государств:  {States.Length}" : "";
        return "Континент (Continent), название: " + Name + StatesCount;
    }
}

//Остров
class Island : Land
{
    public Island (string name) : base(name) { }
    public override void Info()
    {
        Console.WriteLine("Это остров: " + Name);
    }

    public override bool DoClone()
    {
        Console.WriteLine("Клон острова");
        return true;
    }

    public override string ToString()
    {
        return "Остров (Island), Название: " + Name;
    }
}

//Printer - полиморфный метод принимает ссылку на абстрактный класс

class Printer
{
    public void IAmPrinting(EarthPart obj)
    {
        if (obj is Sea) Console.WriteLine("Определено: Море");
        else if (obj is State) Console.WriteLine("Определено: Государство");
        else if (obj is Continent) Console.WriteLine("Определено: Континент");
        else if (obj is Island) Console.WriteLine("Определено: Остров");
        else if (obj is Water) Console.WriteLine("Определено: Вода");
        else if (obj is Land) Console.WriteLine("Определено: Суша");
        else Console.WriteLine("Определено: Неизвестный тип");

        // Вывод информации через ToString()
        Console.WriteLine(obj.ToString());
    }
}


//Контейнер
class EarthContainer
{
    private List<EarthPart> earthParts = new List<EarthPart>();
    
    // метод get (вернуть список)
    public List<EarthPart> GetEarthParts()
    {
        return earthParts;
    }
    
    // метод set (заменить список)
    public void SetEarthParts(List<EarthPart> parts)
    {
        earthParts = parts;
    }
    
    //Добавление элементов
    public void AddEarthPart(EarthPart part)
    {
        earthParts.Add(part);
    }

    //Удаление элементов
    public void RemoveEarthPart(EarthPart part)
    {
        earthParts.Remove(part);
    }
    
    //Вывод в консоль
    public void Print()
    {
        Console.WriteLine("Содержимое контейнера:");
        for (int i = 0; i < earthParts.Count; i++)
        {
            Console.WriteLine(earthParts[i].Name + " ");
        }
    }
}

//Контроллер

class EarthController
{
    private EarthContainer container;

    public EarthController(EarthContainer container)
    {
        this.container = container;
    }

    //Поиск всех государств на континенте
    public List<State> GetEarthParts(string name)
    {
        List<State> states = new List<State>();
        for (int i = 0; i < container.GetEarthParts().Count; i++)
        {
            EarthPart part = container.GetEarthParts()[i];

            if (part is Continent)
            {
                Continent cont = (Continent)part;
                if (cont.Name == name)
                {
                    for (int j = 0; j < cont.States.Length; j++)
                    {
                        states.Add(cont.States[j]);
                    }
                }
            }
        }
        return states;
    }
    
    //Подсчёт всех морей
    public int CountSeas()
    {
        int count = 0;
        for (int i = 0; i < container.GetEarthParts().Count; i++)
        {
            if (container.GetEarthParts()[i] is Sea) 
                count++;
        }
        return count;
    }
    
    //Острова по алфавиту
    public List<Island> GetIslandParts()
    {
        List<Island> islands = new List<Island>();
        for (int i = 0; i < container.GetEarthParts().Count; i++)
        {
            EarthPart part = container.GetEarthParts()[i];
            if (part is Island)
            {
                islands.Add((Island)part);
            }
        }
        //Сортировка пузырьком
        for (int i = 0; i < islands.Count; i++)
        {
            for (int j = i + 1; j < islands.Count; j++)
            {
                if (string.Compare(islands[i].Name, islands[j].Name) > 0)
                {
                    Island temp = islands[i];
                    islands[i] = islands[j];
                    islands[j] = temp;
                }
            }
        }
        return islands;
    }
}


class Program
{
    static void Main(string[] args)
    {
        EarthContainer container = new EarthContainer();

        // Создаём государства с типом и координатами
        State belarus = new State("Беларусь");
        Coordinates coordsBelarus = new Coordinates(53.9, 27.5667);
        EarthType typeBelarus = EarthType.State;

        State china = new State("Китай");
        Coordinates coordsChina = new Coordinates(35.8617, 104.1954);
        EarthType typeChina = EarthType.State;

        State france = new State("Франция");
        Coordinates coordsFrance = new Coordinates(46.2276, 2.2137);
        EarthType typeFrance = EarthType.State;

        // Континенты
        Continent europe = new Continent("Европа", new State[] { belarus, france });
        Coordinates coordsEurope = new Coordinates(54.5260, 15.2551);
        EarthType typeEurope = EarthType.Continent;

        Continent asia = new Continent("Азия", new State[] { china });
        Coordinates coordsAsia = new Coordinates(34.0479, 100.6197);
        EarthType typeAsia = EarthType.Continent;

        // Моря
        Sea blackSea = new Sea("Чёрное море");
        Coordinates coordsBlackSea = new Coordinates(43.0, 35.0);
        EarthType typeBlackSea = EarthType.Sea;

        Sea balticSea = new Sea("Балтийское море");
        Coordinates coordsBalticSea = new Coordinates(58.0, 20.0);
        EarthType typeBalticSea = EarthType.Sea;

        // Острова
        Island sakhalin = new Island("Сахалин");
        Coordinates coordsSakhalin = new Coordinates(50.0, 142.0);
        EarthType typeSakhalin = EarthType.Island;

        Island madagascar = new Island("Мадагаскар");
        Coordinates coordsMadagascar = new Coordinates(-18.7669, 46.8691);
        EarthType typeMadagascar = EarthType.Island;

        Island greenland = new Island("Гренландия");
        Coordinates coordsGreenland = new Coordinates(71.7069, -42.6043);
        EarthType typeGreenland = EarthType.Island;

        // Добавляем все объекты в контейнер
        container.AddEarthPart(belarus);
        container.AddEarthPart(china);
        container.AddEarthPart(france);
        container.AddEarthPart(europe);
        container.AddEarthPart(asia);
        container.AddEarthPart(blackSea);
        container.AddEarthPart(balticSea);
        container.AddEarthPart(sakhalin);
        container.AddEarthPart(madagascar);
        container.AddEarthPart(greenland);

        EarthController controller = new EarthController(container);

        //Поиск всех государств на континенте Европа
        Console.WriteLine("Государства на континенте Европа:");
        List<State> europeanStates = controller.GetEarthParts("Европа");
        foreach (State st in europeanStates)
        {
            Console.WriteLine($"{st.Name} (тип: {EarthType.State})");
        }
        Console.WriteLine();

        //Подсчёт количества морей
        int seasCount = controller.CountSeas();
        Console.WriteLine("Количество морей в контейнере: " + seasCount);
        Console.WriteLine();

        //Вывод островов по алфавиту
        Console.WriteLine("Острова по алфавиту:");
        List<Island> islandsSorted = controller.GetIslandParts();
        foreach (Island isl in islandsSorted)
        {
            Console.WriteLine($"{isl.Name} (тип: {EarthType.Island})");
        }
        Console.WriteLine();

        //Работа с координатами
        Console.WriteLine("Координаты некоторых объектов:");
        Console.WriteLine($"{belarus.Name}: {coordsBelarus}");
        Console.WriteLine($"{china.Name}: {coordsChina}");
        Console.WriteLine($"{sakhalin.Name}: {coordsSakhalin}");
        Console.WriteLine();

        //Вывод всего содержимого контейнера
        container.Print();
    }
}
