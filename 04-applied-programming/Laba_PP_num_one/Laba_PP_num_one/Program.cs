using System;
using ClassLibrary;

namespace Laba_PP_num_one
{
    //Задание №1
    public class C1
    {
    //Константы

        //Приватное константа
        private const int x = 10;

        //Публичное константа
        public const string phrase_one = "Привет!";

        //Защищённое константа
        protected const string phrase_two = "Привет снова!";

    //Поля

        //Приватное поле
        private string privateFieldString = "Я приватное поле!";

        //Публичное поле
        public string publicFieldString = "Я публичное поле!";

        //Защищённое поле
        protected string protectedFieldString = "Я защищённое поле!";

    //Свойства

        //Приватное свойство
        private int PrivateProperty { get; set; } = 100;
        //Публичное свойства
        public int PublicProperty { get; set; } = 200;
        //Защищённое свойства 
        protected int ProtectedProperty { get; set; } = 300;

    //Конструкторы

        //По умолчанию
        public C1()
        {
            Console.WriteLine("C1: Конструктор по умолчанию");
        }
  
        //Копирующий
        public C1(C1 other)
        {
            this.PrivateProperty = other.PrivateProperty;
            this.PublicProperty = other.PublicProperty;
            this.ProtectedProperty = other.ProtectedProperty;

            this.privateFieldString = other.privateFieldString;
            this.publicFieldString = other.publicFieldString;
            this.protectedFieldString = other.protectedFieldString;

            Console.WriteLine("С1: копирующий конструктор");
        }

        //С параметрами
        public C1(string privateFieldString, string publicFieldString, string protectedFieldString)
        {
            this.privateFieldString = privateFieldString;
            this.publicFieldString = publicFieldString;
            this.protectedFieldString = protectedFieldString;

            Console.WriteLine("С1: конструктор с параметрами");
        }

    //Методы

        //Приватный
        private void PrivateMethod()
        {
            Console.WriteLine("C1: Приватный метод");
        }

        //Публичный
        public void PublicMethod()
        {
            Console.WriteLine("С1: Публичный метод");
        }

        //Защищённый
        protected void ProtectedMethod()
        {
            Console.WriteLine("С1: Защищённый метод");
        }

        public void Show()
        {
            Console.WriteLine($"C1 private const int (x) = {x}" );
            Console.WriteLine($"C1 public const string (phase_one) = {phrase_one}");
            Console.WriteLine($"C1 protected const string (phase_two) = {phrase_two}");
            Console.WriteLine($"C1 private string (privateFieldString) = {privateFieldString}");
            Console.WriteLine($"C1 public string (publicFieldString) = {publicFieldString}");
            Console.WriteLine($"C1 protected string (protectedFieldString) = {protectedFieldString}");
            Console.WriteLine($"C1 private int (PrivatePropetry) = {PrivateProperty}");
            Console.WriteLine($"C1 public int (PublicPropetry) = {PublicProperty}");
            Console.WriteLine($"C1 protected int (ProtectedPropetry) = {ProtectedProperty}");
        }
    }

    //Задание №2

    //Интерфейс
    interface I1
    {
        //Свойство
        int MyPropetry {  get; set; }

        //Метод
        void MyMethod(string message);

        //Событие
        event EventHandler MyEvent;
        
        //Индексатор
        string this[int index] { get; set; }
    }

    public class MyInterfaceClass : I1
    {
        private string[] data = new string[5];
        public int MyPropetry { get; set; }

        public event EventHandler MyEvent;

        public string this[int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }

        public void MyMethod(string message)
        {
            Console.WriteLine("MyInterfaceClass: " + message);
            MyEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    //Задание №3
    public class C2 : I1
    {
        //Константы

        //Приватное константа
        private const int x = 10;

        //Публичное константа
        public const string phrase_one = "Привет из C2!";

        //Защищённое константа
        protected const string phrase_two = "Привет снова из С2!";

        //Поля

        //Приватное поле
        private string privateFieldString = "Я приватное поле C2!";

        //Публичное поле
        public string publicFieldString = "Я публичное поле C2!";

        //Защищённое поле
        protected string protectedFieldString = "Я защищённое поле C2!";

        //Свойства

        //Приватное свойство
        private int PrivateProperty { get; set; } = 400;
        //Публичное свойства
        public int PublicProperty { get; set; } = 100;
        //Защищённое свойства 
        protected int ProtectedProperty { get; set; } = 500;

        //Конструкторы

        //По умолчанию
        public C2()
        {
            Console.WriteLine("C2: Конструктор по умолчанию");
        }

        //Копирующий
        public C2(C2 other)
        {
            this.PrivateProperty = other.PrivateProperty;
            this.PublicProperty = other.PublicProperty;
            this.ProtectedProperty = other.ProtectedProperty;

            this.privateFieldString = other.privateFieldString;
            this.publicFieldString = other.publicFieldString;
            this.protectedFieldString = other.protectedFieldString;

            Console.WriteLine("С2: копирующий конструктор");
        }

        //С параметрами
        public C2(string privateFieldString, string publicFieldString, string protectedFieldString)
        {
            this.privateFieldString = privateFieldString;
            this.publicFieldString = publicFieldString;
            this.protectedFieldString = protectedFieldString;

            Console.WriteLine("С2: конструктор с параметрами");
        }

        //Методы

        //Приватный
        private void PrivateMethod()
        {
            Console.WriteLine("C2: Приватный метод");
        }

        //Публичный
        public void PublicMethod()
        {
            Console.WriteLine("С2: Публичный метод");
        }

        //Защищённый
        protected void ProtectedMethod()
        {
            Console.WriteLine("С2: Защищённый метод");
        }

        public void Show()
        {
            Console.WriteLine($"C2 private const int (x) = {x}");
            Console.WriteLine($"C2 public const string (phase_one) = {phrase_one}");
            Console.WriteLine($"C2 protected const string (phase_two) = {phrase_two}");
            Console.WriteLine($"C2 private string (privateFieldString) = {privateFieldString}");
            Console.WriteLine($"C2 public string (publicFieldString) = {publicFieldString}");
            Console.WriteLine($"C2 protected string (protectedFieldString) = {protectedFieldString}");
            Console.WriteLine($"C2 private int (PrivatePropetry) = {PrivateProperty}");
            Console.WriteLine($"C2 public int (PublicPropetry) = {PublicProperty}");
            Console.WriteLine($"C2 protected int (ProtectedPropetry) = {ProtectedProperty}");
        }

        //Реализация интерфейса
        private string[] data = new string[3];
        public int MyPropetry { get; set; }
        public event EventHandler MyEvent;
        public string this[int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }
        public void MyMethod(string message)
        {
            Console.WriteLine("C2 (интерфейс): " + message);
            MyEvent?.Invoke(this, EventArgs.Empty);
        }

    }

    public class C3
    {
        private int privateField = 1;
        protected int protectedField = 2;
        public int publicField = 3;

        private int PrivateProperty { get; set; } = 10;
        protected int ProtectedProperty { get; set; } = 20;
        public int PublicProperty { get; set; } = 30;

        public C3()
        {
            Console.WriteLine("C3: Конструктор");
        }

        private void PrivateMethod()
        {
            Console.WriteLine("C3: Приватный метод");
        }

        protected void ProtectedMethod()
        {
            Console.WriteLine("C3: Защищенный метод");
        }

        public void PublicMethod()
        {
            Console.WriteLine("C3: Публичный метод");
            PrivateMethod();
        }

        public void ShowInfo()
        {
            Console.WriteLine($"C3 protectedField = {protectedField}");
            Console.WriteLine($"C3 publicField = {publicField}");
            Console.WriteLine($"C3 ProtectedProperty = {ProtectedProperty}");
            Console.WriteLine($"C3 PublicProperty = {PublicProperty}");
        }
    }

    public class C4 : C3
    {
        public string NewField = "C4 Field";
        public int NewProperty { get; set; }

        public C4()
        {
          
            Console.WriteLine("C4: Конструктор");
        }

        public void NewMethod()
        {
            Console.WriteLine("C4: Новый метод");
            ProtectedMethod();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ////Задание №1
            //Console.WriteLine("\n----- Задание №1 -----");
            //C1 obj1 = new C1();
            //obj1.PublicMethod();
            //obj1.Show();

            //Console.WriteLine();

            //C1 obj2 = new C1("Я учусь", "в", "БГТУ");
            //obj2.PublicProperty = 123;
            //obj2.PublicMethod();
            //obj2.Show();

            //Console.WriteLine();
            //C1 obj3 = new C1(obj2);
            //obj3.Show();
            //Console.WriteLine();

            ////Задание №2
            //Console.WriteLine("\n----- Задание №2 -----");
            //MyInterfaceClass interfaceObj = new MyInterfaceClass();
            //interfaceObj.MyEvent += (s, e) => Console.WriteLine("MyEvent сработал из MyInterfaceClass");
            //interfaceObj.MyPropetry = 100;
            //interfaceObj[0] = "Тест из MyInterfaceClass";
            //interfaceObj.MyMethod("Привет из MyInterfaceClass");

            ////Задание №3
            //Console.WriteLine("\n----- Задание №3 -----");
            //C2 c2 = new C2();
            //c2.MyEvent += (s, e) => Console.WriteLine("MyEvent сработал из C2");
            //c2.MyPropetry = 18;
            //c2[0] = "Тест из C2";
            //c2.MyMethod("Привет из C2");
            //c2.PublicMethod();
            //c2.Show();

            ////Задание №4
            //Console.WriteLine("\n----- Задание №4 -----");
            //C4 c4 = new C4();
            //c4.PublicMethod();      // унаследованный метод
            //c4.NewMethod();         // собственный метод
            //c4.NewProperty = 999;
            //c4.ShowInfo();
            //Console.WriteLine($"C4 NewField = {c4.NewField}");
            //Console.WriteLine($"C4 NewProperty = {c4.NewProperty}");

            int a = 6;
            int b = 7;
            Hello hello = new Hello();
            hello.Sum(a, b);

            int res = hello.CompositionFromInternalClass(a, b);
            Console.WriteLine("Произведение: " + res);

        }
    }
}
