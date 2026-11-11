using System.Reflection;
using System.Text;
using static Laba_Eleven.Program;

namespace Laba_Eleven
{
    internal class Program
    {
        public static class Reflector
        {
            public static void FindNameAssembling(string classname)
            {
                Console.WriteLine(" ~~~ a. Определение имени сборки, в которой  определен класс ~~~ ");
                Type? type = Type.GetType(classname);
                if (type == null)
                {
                    Console.WriteLine("Класс не найден");
                    return;
                }
                string? assemblyName = type.Assembly.FullName;
                Console.WriteLine($"Сборка: {assemblyName}");
                Console.WriteLine(); Console.WriteLine();
            }

            public static void GetPublicConstructors(string classname)
            {
                Console.WriteLine(" ~~~ b. Есть ли публичные конструкторы ~~~ ");
                Type? type = Type.GetType(classname);
                if (type == null)
                {
                    Console.WriteLine("Класс не найден");
                    return;
                }
                ConstructorInfo[] constructors = type.GetConstructors();

                foreach (MemberInfo constructor in constructors)
                {
                    Console.WriteLine($"{constructor.ToString()}");
                }
                Console.WriteLine(); Console.WriteLine();
            }

            public static IEnumerable<string> AllPublicMethods(string classname)
            {
                Type? type = Type.GetType(classname);
                if (type == null)
                {
                    Console.WriteLine("Класс не найден");
                    yield break;
                }

                MethodInfo[] methods = type.GetMethods();

                foreach (var method in methods)
                {
                    yield return method.Name;
                }
                Console.WriteLine(); Console.WriteLine();
            }

            public static IEnumerable<string> AllInterfacesRealizedClass(string classname)
            {
                Type? type = Type.GetType(classname);
                if(type == null)
                {
                    Console.WriteLine("Класс не найден");
                    yield break;
                }

                Type[] interfaces = type.GetInterfaces();
                if (interfaces.Length == 0)
                {
                    Console.WriteLine("Интерфейсы не найдены");
                    yield break;
                }
                else
                {
                    foreach (var iface in interfaces)
                    {
                        yield return iface.Name;
                    }
                }
            }

            public static IEnumerable<string> MethodsWithParametrType(string classname, string parametrType)
            {
                Type? type = Type.GetType(classname);
                if (type == null)
                {
                    Console.WriteLine("Класс не найден");
                    yield break;
                }

                MethodInfo[] methods = type.GetMethods();

                foreach (MethodInfo method in methods)
                {
                    foreach (ParameterInfo parametr in method.GetParameters())
                    {
                        if (parametr.ParameterType.Name == parametrType)
                        {
                            yield return method.Name;
                            break;
                        }
                    }
                }
            }

            public static object? Invoke(object obj, string methodName, object[] parametrs)
            {
                Type type = obj.GetType();

                MethodInfo? method = type.GetMethod(methodName);
                if (method == null)
                {
                    Console.WriteLine("Метод не найден");
                    return null;
                }

                return method.Invoke(obj, parametrs);
            }

            public static object[] ReadFromFile(string filePath, MethodInfo method)
            {
                string[] lines = File.ReadAllLines(filePath);
                ParameterInfo[] parametrs = method.GetParameters();

                object[] result = new object[parametrs.Length];

                for (int i = 0; i < parametrs.Length; i++)
                {
                    Type targetType = parametrs[i].ParameterType;
                    result[i] = Convert.ChangeType(lines[i], targetType);
                }
                return result;
            }

            public static object[] GenerateParams(MethodInfo method)
            {
                ParameterInfo[] parameters = method.GetParameters();
                object[] result = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    result[i] = GenerateValue(parameters[i].ParameterType);
                }

                return result;
            }

            public static object GenerateValue(Type t)
            {
                if (t == typeof(int)) 
                    return 42;
                if (t == typeof(string)) 
                    return "AutoString";
                if (t == typeof(double)) 
                    return 3.14;
                if (t == typeof(bool)) 
                    return true;

                //если тип класс, то создаётся экземпляр
                return Activator.CreateInstance(t)!;
            }

            public static T Create<T> (params object[] args)
            {
                Type type = typeof(T);
                ConstructorInfo[] constructors = type.GetConstructors();
                if (constructors.Length == 0)
                {
                    throw new InvalidOperationException($"У типа {type.Name} нет публичных конструкторов.");
                }
                foreach (var constr in constructors)
                {
                    ParameterInfo[] parametrs = constr.GetParameters();
                    if (parametrs.Length != args.Length)
                        continue;
                    bool match = true;
                    for (int i = 0; i < parametrs.Length; i++)
                    {
                        if (!parametrs[i].ParameterType.IsAssignableFrom(args[i].GetType()))
                        {
                            match = false;
                            break;
                        }
                    }

                    if(match)
                    {
                        return (T)constr.Invoke(args);
                    }

                }
                throw new ArgumentException("Не найден подходящий конструктор для указанных аргументов.");
            }
            public static void WriteStringToFile(string text, string filePath)
            {
                try
                {
                    File.WriteAllText(filePath, text);
                    Console.WriteLine($"Данные успешно записаны в файл: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при записи в файл: {ex.Message}");
                }
            }
            public static void SaveClassInfo(string className, string filePath)
            {
                Type? type = Type.GetType(className);
                if (type == null)
                {
                    WriteStringToFile("Класс не найден", filePath);
                    return;
                }

                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"~~~ Информация о классе {className} ~~~");

                // 1. Имя сборки
                sb.AppendLine("Имя сборки:");
                sb.AppendLine(type.Assembly.FullName);
                sb.AppendLine();

                // 2. Публичные конструкторы
                sb.AppendLine("Публичные конструкторы:");
                foreach (var ctor in type.GetConstructors())
                    sb.AppendLine("   " + ctor);
                sb.AppendLine();

                // 3. Публичные методы
                sb.AppendLine("Публичные методы:");
                foreach (var method in type.GetMethods())
                    sb.AppendLine("   " + method.Name);
                sb.AppendLine();

                // 4. Интерфейсы
                sb.AppendLine("Реализованные интерфейсы:");
                var interfaces = type.GetInterfaces();
                if (interfaces.Length == 0)
                    sb.AppendLine("Нет интерфейсов");
                else
                    foreach (var iface in interfaces)
                        sb.AppendLine("   " + iface.Name);
                sb.AppendLine();

                // 5. Методы по параметрам
                sb.AppendLine("Методы с параметрами int, string, bool, double:");
                foreach (var method in type.GetMethods())
                {
                    foreach (var p in method.GetParameters())
                    {
                        if (p.ParameterType == typeof(int) ||
                            p.ParameterType == typeof(string) ||
                            p.ParameterType == typeof(bool) ||
                            p.ParameterType == typeof(double))
                        {
                            sb.AppendLine($"   {method.Name} ({p.ParameterType.Name})");
                        }
                    }
                }
                WriteStringToFile(sb.ToString(), filePath);
            }
        }
        public class Student
        {
            public int Id;
            public string Name;
            public Student(int id, string name) 
            {
                Name = name;
                Id = id;
            }
            public void MyAge(int age)
            {
                Console.WriteLine($"Мне {age} лет");
            }
            public void MyName(string name)
            {
                Console.WriteLine($"Меня зовут {name}");
            }
        }
        public class Product
        {
            public int Id;
            public string NameProd;
            public Product(int id, string nameProd)
            {
                Id = id;
                NameProd = nameProd;
            }

            public void MyCondition(int id)
            {
                Console.WriteLine($"Я просроченный товар под номером {id}");
            }
            public void MyName(string name)
            {
                Console.WriteLine($"Я товар под именем {name}");
            }
        }
        public static void TaskOne()
        {
            Console.WriteLine("------Задание №1------");
            Type t = typeof(Student);
            Type p = typeof(Product);
            //a
            Reflector.FindNameAssembling(t.FullName!);
            Reflector.FindNameAssembling(p.FullName!);
            //b
            Reflector.GetPublicConstructors(t.FullName!);
            Reflector.GetPublicConstructors(p.FullName!);
            //c
            Console.WriteLine(" ~~~ c. Извлекает все общедоступные публичные методы класса (возвращает IEnumerable<string>) ~~~ ");
            foreach (string method in Reflector.AllPublicMethods(t.FullName!))
            {
                Console.WriteLine(method);
            }
            //d
            Console.WriteLine(" ~~~ c. Извлекает все общедоступные публичные методы класса (возвращает IEnumerable<string>) ~~~ ");
            foreach (string method in Reflector.AllPublicMethods(t.FullName!))
            {
                Console.WriteLine(method);
            }
            foreach (string method in Reflector.AllPublicMethods(p.FullName!))
            {
                Console.WriteLine(method);
            }
            //e
            Console.WriteLine(" ~~~ e. получает все реализованные классом интерфейсы (возвращает IEnumerable<string>) ~~~ ");
            foreach (string method in Reflector.AllInterfacesRealizedClass(t.FullName!))
            {
                Console.WriteLine(method);
            }
            foreach (string method in Reflector.AllInterfacesRealizedClass(p.FullName!))
            {
                Console.WriteLine(method);
            }
            Console.WriteLine("(Статический класс не может реализовывать интерфейсы)");
            Console.WriteLine(); Console.WriteLine();
            //f
            Console.WriteLine(" ~~~ f. выводит по имени класса имена методов, которые содержат заданный (пользователем) тип параметра  ~~~ ");
            Console.WriteLine("Тип: Int32");
            foreach (string method in Reflector.MethodsWithParametrType(t.FullName!, "Int32"))
            {
                Console.Write(method);
            }
            Console.WriteLine();
            Console.WriteLine("Тип: String");
            foreach (string method in Reflector.MethodsWithParametrType(t.FullName!, "String"))
            {
                Console.Write(method);
            }
            Console.WriteLine(); Console.WriteLine();
            Console.WriteLine("Тип: Int32");
            foreach (string method in Reflector.MethodsWithParametrType(p.FullName!, "Int32"))
            {
                Console.Write(method);
            }
            Console.WriteLine();
            Console.WriteLine("Тип: String");
            foreach (string method in Reflector.MethodsWithParametrType(p.FullName!, "String"))
            {
                Console.Write(method);
            }
            Console.WriteLine(); Console.WriteLine();
            //g
            Console.WriteLine(" ~~~ g. метод Invoke, который вызывает метод класса, при этом значения для его параметров необходимо:   ~~~ ");
            Console.WriteLine("1) прочитать из текстового файла (имя класса и имя метода передаются в качестве аргументов)");

            //Для Student
            Student st = new Student(1, "Anton");
            MethodInfo m = typeof(Student).GetMethod("MyAge")!;

            object[] parametrs = Reflector.ReadFromFile("parametrs.txt", m);
            Reflector.Invoke(st, "MyAge", parametrs);
            Console.WriteLine();

            //Для Product
            Product prod = new Product(101, "Молоко");
            MethodInfo mProduct = typeof(Product).GetMethod("MyCondition")!;
            object[] parametersFromFile = Reflector.ReadFromFile("parametrss.txt", mProduct);
            Reflector.Invoke(prod, "MyCondition", parametersFromFile);
            Console.WriteLine();

            //Для Student
            Console.WriteLine("2) сгенерировать, используя генератор значений для каждого типа");
            MethodInfo mm = typeof(Student).GetMethod("MyName")!;
            object[] parametrss = Reflector.GenerateParams(mm);
            Reflector.Invoke(st, "MyName", parametrss);
            Console.WriteLine();

            //Для Product
            MethodInfo mmProduct = typeof(Product).GetMethod("MyName")!;
            object[] parametersGenerated = Reflector.GenerateParams(mmProduct);
            Reflector.Invoke(prod, "MyName", parametersGenerated);
            Console.WriteLine();
        }
        public static void TaskTwo()
        {
            Console.WriteLine("------Задание №2------");
            Student s = Reflector.Create<Student>(1, "Anton");
            Console.WriteLine(s.Name);

            Product p = Reflector.Create<Product>(101, "Молоко");
            Console.WriteLine(p.NameProd);
        }
        static void Main(string[] args)
        {
            TaskOne();
            TaskTwo();
            Reflector.SaveClassInfo(typeof(Student).FullName!, "StudentInfo.txt");
            Reflector.SaveClassInfo(typeof(Product).FullName!, "ProductInfo.txt");

        }
    }
}
