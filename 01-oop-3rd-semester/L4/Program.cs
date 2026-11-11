using System;

namespace Laba_Four;

// EarthPart — базовый абстрактный класс;
// Water и Land наследуются от EarthPart;
// Sea наследуется от Water и помечен sealed;
// Continent, State, Island наследуются от Land;

interface ICloneable
{
    bool DoClone();
}
//Абстрактный класс
abstract class EarthPart
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
    
    //Переопределение ToString()
    public override string ToString()
    {
        return "Тип: " + GetType().Name + "Название: " + Name;
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


class Program
{
    static void Main(string[] args)
    {
        //Создание объектов
        State Belarus  = new State("Belarus");
        State China =  new State("China");
        Continent eurasia = new Continent("Евразия", new State[] {Belarus, China});
        
        EarthPart sea = new Sea("Чёрное море");
        EarthPart isLand = new Island("Сахалин");
        
        //Создание объекта через ссылку интерфейса ICloneable
        ICloneable lake =  new Water("Озеро");
        lake.DoClone(); 
        
        EarthPart river =  new Land("Река");
        river.DoClone();
        
        //Проверка типов is/as
        if (sea is Sea) Console.WriteLine("Sea - это море!");
        State mbState = eurasia.States[0] as State;
        if (mbState != null) Console.WriteLine("Первое государство на континенте: " + mbState.Name); 
        
        Printer printer = new Printer();
        EarthPart[] arr = { sea, eurasia, isLand, river };

        for (int i = 0; i < arr.Length; i++)
        {
            printer.IAmPrinting(arr[i]);
            Console.WriteLine();
        }
    }
}