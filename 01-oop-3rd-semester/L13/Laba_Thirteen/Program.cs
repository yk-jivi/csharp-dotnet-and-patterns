using System;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Laba_Thirteen
{
    [DataContract]
    [Serializable]
    public abstract class EarthPart
    {
        [DataMember]
        public string Name { get; set; }

        public EarthPart() { }
        public EarthPart(string name) => Name = name;

        public abstract void Info();

        public virtual void Display() =>
            Console.WriteLine("Display: " + ToString());

        public override string ToString() =>
            $"Тип: {GetType().Name}, Название: {Name}";
    }

    [DataContract]
    [Serializable]
    public class Water : EarthPart
    {
        public Water() { }
        public Water(string name) : base(name) { }

        public override void Info() =>
            Console.WriteLine("Это вода: " + Name);

        public override string ToString() =>
            "Вода (Water), название: " + Name;
    }

    [DataContract]
    [Serializable]
    public class Land : EarthPart
    {
        public Land() { }
        public Land(string name) : base(name) { }

        public override void Info() =>
            Console.WriteLine("Это суша: " + Name);

        public override string ToString() =>
            "Суша (Land), название: " + Name;
    }

    [DataContract]
    [Serializable]
    public class State : Land
    {
        public State() { }
        public State(string name) : base(name) { }

        public override void Info() =>
            Console.WriteLine("Это государство: " + Name);

        public override string ToString() =>
            "Государство (State), Название: " + Name;
    }

    [DataContract]
    [Serializable]
    public class Continent : Land
    {
        [DataMember]
        public State[] States { get; set; }

        public Continent() { }
        public Continent(string name, State[] states) : base(name) =>
            States = states;

        public override void Info()
        {
            Console.WriteLine("Это континент: " + Name);
            if (States != null)
            {
                Console.WriteLine("Содержит государства:");
                foreach (var s in States)
                    Console.WriteLine(" " + s.Name);
            }
        }

        public override string ToString() =>
            $"Континент: {Name}, Кол-во государств: {States?.Length ?? 0}";
    }

    public class Program
    {
        static void Test(SerializationType type, Continent cont, string file)
        {
            Console.WriteLine($"\n~~~ {type} ~~~");
            var ser = new CustomSerializer(type);
            ser.Save(cont, file);
            Continent newCont = ser.Load<Continent>(file);
            Console.WriteLine("Загружено: " + newCont.Name);
        }
        static void TaskOne()
        {
            State s1 = new State("Франция");
            State s2 = new State("Германия");
            Continent europe = new Continent("Европа", new[] { s1, s2 });

            Test(SerializationType.Binary, europe, "continent.bin");
            Test(SerializationType.Soap, europe, "continent.soap.xml");
            Test(SerializationType.Xml, europe, "continent.xml");
            Test(SerializationType.Json, europe, "continent.json");

            Console.WriteLine("\nВсе сериализации выполнились успешно");
            Console.WriteLine(); Console.WriteLine();
        }
        static void TaskTwo()
        {
            State[] states = new State[]
            {
                new State("Испания"),
                new State("Италия"),
                new State("Франция")
            };

            string jsonFile = "states.json";
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(states, jsonOptions);
            File.WriteAllText(jsonFile, json);

            var loadedStatesJson = JsonSerializer.Deserialize<State[]>(File.ReadAllText(jsonFile));
            Console.WriteLine("JSON: ");
            foreach (var s in loadedStatesJson)
                Console.WriteLine(s.Name);

            string xmlFile = "states.xml";
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(State[]));
            using (FileStream fs = new FileStream(xmlFile, FileMode.Create))
                xmlSerializer.Serialize(fs, states);

            using (FileStream fs = new FileStream(xmlFile, FileMode.Open))
            {
                var loadedStatesXml = (State[])xmlSerializer.Deserialize(fs);
                Console.WriteLine("XML: ");
                foreach (var s in loadedStatesXml)
                    Console.WriteLine(s.Name);
            }
            Console.WriteLine();Console.WriteLine();
        }
        static void TaskThree()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("states.xml");

            XmlNodeList? nodes1 = doc.SelectNodes("/ArrayOfState/State");

            Console.WriteLine("Все государства:");
            foreach (XmlNode node in nodes1)
            {
                Console.WriteLine(node["Name"].InnerText);
            }

            XmlNode? node2 = doc.SelectSingleNode("/ArrayOfState/State[Name='Испания']");
            if (node2 != null)
            {
                Console.WriteLine("\nНайдено государство с Name = 'Испания':");
                Console.WriteLine(node2["Name"].InnerText);
            }
            else
            {
                Console.WriteLine("\nГосударство с Name = 'Испания' не найдено");
            }
            Console.WriteLine(); Console.WriteLine();
        }
        static void TaskFour()
        {
            XDocument doc = new XDocument(
            new XElement("States",
                new XElement("State", new XElement("Name", "Нидерладны")),
                new XElement("State", new XElement("Name", "Норвегия")),
                new XElement("State", new XElement("Name", "Австрия"))
                )
            );

            doc.Save("states_linq.xml");

            var names = doc.Descendants("State")
                           .Select(s => s.Element("Name")?.Value);

            var state = doc.Descendants("State")
                             .Where(s => s.Element("Name")?.Value == "Норвегия")
                             .Select(s => s.Element("Name")?.Value)
                             .FirstOrDefault();
            foreach ( var st in names)
            {
                Console.WriteLine(st);
            }
            Console.WriteLine();
            Console.WriteLine(state);
        }
        static void Main()
        {
            TaskOne();
            TaskTwo();
            TaskThree();
            TaskFour();
        }

        
    }
}
