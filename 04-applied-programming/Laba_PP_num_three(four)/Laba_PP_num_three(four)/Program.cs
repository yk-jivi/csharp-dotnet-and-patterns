using Lec03LibN;
using PP03;
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Лабораторная работа № 3");

            // фабрика 1
            IFactory l1 = Lec03BLib.getL1();
            float hours = 4.0f; // количество отработанных часов

            // 1-a
            Employee employee1 = new Employee(l1.getA(25));
            Console.WriteLine(string.Format("Bonus-L1-A = {0}", employee1.calcBonus(hours)));

            // 1-b
            Employee employee2 = new Employee(l1.getB(25, 1.1f));
            Console.WriteLine(string.Format("Bonus-L1-B = {0}", employee2.calcBonus(hours)));

            // 1-c
            Employee employee3 = new Employee(l1.getC(25, 1.1f, 5.0f));
            Console.WriteLine(string.Format("Bonus-L1-C = {0}", employee3.calcBonus(hours)));

            // фабрика 2
            IFactory l2 = Lec03BLib.getL2(1.0f);

            // 2-a
            Employee employee4 = new Employee(l2.getA(25));
            Console.WriteLine(string.Format("Bonus-L2-A = {0}", employee4.calcBonus(hours)));

            // 2-b
            Employee employee5 = new Employee(l2.getB(25, 1.1f));
            Console.WriteLine(string.Format("Bonus-L2-B = {0}", employee5.calcBonus(hours)));

            // 2-c
            Employee employee6 = new Employee(l2.getC(25, 1.1f, 5.0f));
            Console.WriteLine(string.Format("Bonus-L2-C = {0}", employee6.calcBonus(hours)));

            // фабрика 3
            IFactory l3 = Lec03BLib.getL3(1.0f, 0.5f);

            // 3-a
            Employee employee7 = new Employee(l3.getA(25));
            Console.WriteLine(string.Format("Bonus-L3-A = {0}", employee7.calcBonus(hours)));

            // 3-b
            Employee employee8 = new Employee(l3.getB(25, 1.1f));
            Console.WriteLine(string.Format("Bonus-L3-B = {0}", employee8.calcBonus(hours)));

            // 3-c
            Employee employee9 = new Employee(l3.getC(25, 1.1f, 0.5f));
            Console.WriteLine(string.Format("Bonus-L3-C = {0}", employee9.calcBonus(hours)));
        }
    }