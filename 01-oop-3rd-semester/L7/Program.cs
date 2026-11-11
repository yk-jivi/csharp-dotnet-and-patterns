using System;
using System.Collections.Generic;
using System.IO;

//Задание №1
//Простой обобщённый интерфейс
public interface ISimpleCollection<T>
{
    void Add(T item);
    bool Remove(T item);
    void ShowAll();
}

//Задание №4
//Пользовательский класс
public abstract class EarthPart
{
    public string Name;

    public EarthPart(string name)
    {
        Name = name;
    }
    public abstract void Info();
    public override string ToString()
    {
        return Name;
    }
}

public class Land : EarthPart, IComparable
{
    public Land (string name) : base(name) { }

    public override void Info()
    {
        Console.WriteLine("Это место: " + Name);
    }
    
    public int CompareTo(object obj)
    {
        if (obj == null)
        {
            return 1;
        }
        if (obj is Land other)
        {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        return 0;
    }
}

public class State : Land
{
    public State (string name) : base(name) { }

    public override void Info()
    {
        Console.WriteLine("Это государство: " + Name);
    }
}

//Задание №2
//Обобщённый класс CollectionType<T> (реализует ISimpleCollection<T>
public class Collection<T> : ISimpleCollection<T> where T : IComparable
{
    private List<T> items;

    public Collection()
    {
        items = new List<T>();
    }

    public void Add(T item)
    {
        try
        {
            items.Add(item);
            Console.WriteLine("Элемент добавлен: " + item);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при добавлении: " + ex.Message);
        }
        finally
        {
            Console.WriteLine("Add завершён (finally)");
        }
    }

    public bool Remove(T item)
    {
        bool removed = false;
        try
        {
            removed = items.Remove(item);
            Console.WriteLine(removed ? "Элемент удалён: " + item : "Элемент для удаления не найден: " + item);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при удалении: " + ex.Message);
        }
        finally
        {
            Console.WriteLine("Remove завершён (finally)");
        }
        return removed;
    }
    
    //Просмотр всех элементов
    public void ShowAll()
    {
        try
        {
            Console.WriteLine("Содержимое коллекции: ");
            if (items.Count == 0)
            {
                Console.WriteLine("Пусто");
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Console.WriteLine($"[{i}] {items[i]}");

                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при просмотре: " + ex.Message);
        }
        finally
        {
            Console.WriteLine("Show завершён (finally)");
        }
    }
    
    // Поиск первого элемента по предикату (возвращает default(T) если не найден)
    public T Find(Predicate<T> predicate)
    {
        try
        {
            foreach (var it in items)
            {
                if (predicate(it))
                {
                    Console.WriteLine("Элемент найден: " + it);
                    return it;
                }
            }

            Console.WriteLine("Элемент по условию не найден");
            return default;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при поиске: " + ex.Message);
            return default;
        }
        finally
        {
            Console.WriteLine("Find завершён (finally)");
        }
    }
    
    //Задание №5
    //Текстовые файлы
    
    //Сохранение в текстовый файл
    public void SaveToTextFile(string path)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (var it in items)
                {
                    sw.WriteLine(it.ToString());
                }
            }

            Console.WriteLine("Коллекция сохранена в текстовый файл: " + path);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при сохранении в текстовый файл: " + ex.Message);
            throw;
        }
        finally
        {
            Console.WriteLine("SaveToTextFile завершён");
        }
    }
    
    //Загрузка из текстового файла
    public static Collection<T> LoadFromTextFile(string path)
    {
        var coll =  new Collection<T>();
        try
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Файл не найден", path);
            
            string[] lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                object value = null;

                if (typeof(T) == typeof(State))
                {
                    value = new State(line);  // State можно создать
                }
                else
                {
                    value = Convert.ChangeType(line, typeof(T));
                }
                coll.items.Add((T)value);
            }

            Console.WriteLine("R: " + path);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при загрузке из файла: " + ex.Message);
        }
        finally
        {
            Console.WriteLine("LoadFromTextFile завершён (finally)");
        }

        return coll;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Демонстрация Collection<T>");
        Console.WriteLine();
        
        //Задание №3
        //Стандартные типы
        Console.WriteLine("---Тест с int---");
        var intCol = new Collection<int>();
        intCol.Add(18);
        intCol.Add(9);
        intCol.Add(6);
        intCol.ShowAll();
        
        var foundInt = intCol.Find(x => x > 6);
        Console.WriteLine("Find вернул: " + (foundInt == default(int) ? "нет" :  foundInt.ToString()));
        intCol.Remove(6);
        intCol.ShowAll();
        string intFile = "ints.txt";
        intCol.SaveToTextFile(intFile);
        var loadedInts = Collection<int>.LoadFromTextFile(intFile);
        Console.WriteLine("После загрузки (int): ");
        loadedInts.ShowAll();
        
        Console.WriteLine();
        Console.WriteLine("---Тест с double---");
        var doubleCol = new Collection<double>();
        doubleCol.Add(3.14);
        doubleCol.Add(2.71);
        doubleCol.ShowAll();
        string doubleFile = "doubles.txt";
        doubleCol.SaveToTextFile(doubleFile);
        var loadedDoubles = Collection<double>.LoadFromTextFile(doubleFile);
        Console.WriteLine("После загрузки (double): ");
        loadedDoubles.ShowAll();
        
        Console.WriteLine();
        //Задание №4
        Console.WriteLine("---Текст с пользовательским типом Land---");
        var stateCol = new Collection<State>();
        stateCol.Add(new State("Belarus"));
        stateCol.Add(new State("China"));
        stateCol.Add(new State("USA"));
        stateCol.ShowAll();
        
        //Поиск по имени
        var foundState = stateCol.Find(x => x.Name == "Belarus");
        if (foundState != null) 
        {
            Console.WriteLine("Найдено государство: " + foundState.Name);
        }
        
        //Сохранение и загрузка в текстовый файл
        string stateFile = "states.txt";
        stateCol.SaveToTextFile(stateFile);
        var loadedStates = Collection<State>.LoadFromTextFile(stateFile);
        Console.WriteLine("После загрузки (State): ");
        loadedStates.ShowAll();
        
        Console.WriteLine();
        Console.WriteLine("Конец");
        Console.WriteLine();
    }
}

