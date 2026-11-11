using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


namespace Laba_Nnine
{
    class InternetResource
    {
        public string url;
        public string name;
        public int id;

        public InternetResource(string url, string name, int id)
        {
           this.url = url;
           this.name = name;
           this.id = id;
        }

        public override string ToString()
        {
           return "URL: " + url + "\nName: " + name + "\nId: " + id;
        }
    }

    class InternetResourceManager : IList<InternetResource>
    {
        ConcurrentDictionary<int, InternetResource> collection = new ConcurrentDictionary<int, InternetResource>();

        private int elements_count = 0;

        public InternetResource this[int index]
        {
            get
            {
                if (collection.ContainsKey(index))
                {
                    return collection[index];
                }
                else
                    throw new ArgumentOutOfRangeException("Нет элемента с таким индексом");
            }
            set
            {
                collection[index] = value;
            }
        }

        public int Count 
        {
            get 
            {  
                return collection.Count; 
            }
        }

        public bool IsReadOnly 
        { 
            get 
            { 
                return false; 
            } 
        }

        public void Add(InternetResource item)
        {
            collection[elements_count] = item;
            elements_count++;
        }

        public void Clear()
        {
            collection.Clear();
            elements_count = 0;
        }

        public bool Contains(InternetResource item) 
        {
            return collection.Values.Contains(item);
        }

        public void CopyTo(InternetResource[] array, int arrayIndex)
        {
            collection.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<InternetResource> GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }


        public int IndexOf(InternetResource item)
        {
            foreach(var pair in collection)
            {
                if(pair.Value.Equals(item))
                {
                    return pair.Key;
                }
            }
            return -1;
        }

        public void Insert(int index, InternetResource item)
        {
            collection[index] = item;
        }

        public bool Remove(InternetResource item)
        {
            int index = IndexOf(item);
            if(index != -1)
            {
                return collection.TryRemove(index, out _);
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            collection.TryRemove(index, out _);
        }
        //Мои методы
        public void RemoveElement(int id)
        {
            if (collection.ContainsKey(id))
            {
                collection.TryRemove(id, out _);
            }
            else
                Console.WriteLine("Элемента по такому ID не найдено");
        }
        public InternetResource? SearchElement(int id)
        {
            if (collection.ContainsKey(id))
            {
                return collection[id];
            }
            return null;
        }
        public void OutputElements()
        {
            for (int i = 0; i < collection.Count; i++)
            {
                Console.WriteLine($"\n{collection[i]}");
            }
        }
    }

    internal class Program
    {
        static void TaskOne()
        {
            Console.WriteLine("~~~~~~~~ Задание №1 ~~~~~~~~");
            InternetResourceManager manager = new InternetResourceManager();

            manager.Add(new InternetResource("//youtube", "YouTube", 1));
            manager.Add(new InternetResource("//github", "GitHub", 2));
            manager.Add(new InternetResource("//habr", "Habr", 3));

            manager.OutputElements();

            Console.WriteLine("--- Удаление элементов с Id = 1 ---");
            manager.RemoveElement(2);

            manager.OutputElements();

            Console.WriteLine("--- Поиск элемента с Id = 2 ---");
            Console.WriteLine(manager.SearchElement(2));
        }
        
        //Создание универсальной коллекции
        static void TaskTwo()
        {
            Console.WriteLine("~~~~~~~~ Задание №2 ~~~~~~~~");
            List<int> numbers = new List<int>() { 18, 2024, 0, 13};

            Console.WriteLine("Исходный List: ");
            foreach (int i in numbers)
                Console.Write(i + " ");

            //Удаление элементов
            int n = 3;
            for (int i = 0; i < n && numbers.Count > 0; i++)
            {
                numbers.RemoveAt(0);
            }

            Console.WriteLine("\nList после удаления элементов: ");
            foreach (int i in numbers)
                Console.Write(i + " ");

            //Добавление элементов
            numbers.Add(13);
            numbers.AddRange(new int[] { 2024, 14 });
            numbers.Insert(1, 18);

            Console.WriteLine("\nList после добавления элементов: ");
            foreach (int i in numbers)
                Console.Write(i + " ");

            //Создание второй коллекции
            Dictionary<int, int> dict = new Dictionary<int, int>();

            int index = 0;

            foreach (int i in numbers)
            {
                dict.Add(index, i);
                index++;
            }

            Console.WriteLine("\nВторая коллекция (Dictionary): ");
            foreach(var i in dict)
            {
                Console.WriteLine($"Ключ: {i.Key}, значение: {i.Value}");
            }

            //Поиск элемента
            if (dict.ContainsKey(3))
                Console.WriteLine($"Элемент с ключом 3: {dict[3]}");
            else
                Console.WriteLine("Элемент не найден");
        }

        static void TaskThree()
        {
            ObservableCollection<InternetResource> internetResources = new ObservableCollection<InternetResource>();

            internetResources.CollectionChanged += OnCollectionChanged;

            internetResources.Add(new InternetResource("//pinterest", "Pinterest", 1));
            internetResources.Add(new InternetResource("//behance", "Behance", 2));
            

            Console.WriteLine("\nИсходная коллекция: ");
            foreach (var i in internetResources)
                Console.Write(i + " ");

            Console.WriteLine("\nВставка элемента: ");
            internetResources.Insert(2, new InternetResource("//adobecolor", "AdobeColor", 3));

            Console.WriteLine("\nУдаление элемента: ");
            internetResources.RemoveAt(0);
            internetResources.Remove(internetResources[1]);

            Console.WriteLine("\nЗамена элемента: ");
            internetResources[0] = new InternetResource("//figma", "Figma", 1);

            Console.WriteLine("\nОчистка коллекции: ");
            internetResources.Clear();
        }

        private static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Console.WriteLine($"Добавлено {e.NewItems.Count} элемент(ов):");
                    foreach (InternetResource item in e.NewItems)
                        Console.WriteLine($"  + {item}");
                    break;

                case NotifyCollectionChangedAction.Remove:
                    Console.WriteLine($"Удалено {e.OldItems.Count} элемент(ов):");
                    foreach (InternetResource item in e.OldItems)
                        Console.WriteLine($"  - {item}");
                    break;

                case NotifyCollectionChangedAction.Replace:
                    Console.WriteLine("Элемент заменён:");
                    Console.WriteLine($"  Было: {e.OldItems[0]}");
                    Console.WriteLine($"  Стало: {e.NewItems[0]}");
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Console.WriteLine("Коллекция очищена");
                    break;
            }

            //Вывод текущего количества элементов
            var coll = sender as ObservableCollection<InternetResource>;
            Console.WriteLine($"Текущее количество элементов: {coll?.Count ?? 0}\n");
        }

        static void Main(string[] args)
        {
            TaskOne();
            TaskTwo();
            TaskThree();
        }
    }
}
