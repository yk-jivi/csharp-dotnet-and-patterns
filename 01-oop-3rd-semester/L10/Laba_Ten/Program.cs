namespace Laba_Ten
{
    internal class Program
    {
        public static void TaskOne()
        {
            Console.WriteLine(" ~~~~ Задание №1 ~~~~ ");
            int n = 4;
            List<string> months = new List<string>(["January", "February", "March", "April", "May",
                "June", "July", "August", "September", "October", "November", "December"]);

            var months_n = months.Where(m => m.Length == n).Select(m => m);

            Console.WriteLine($"Месяцы с длиной строки, равной {n} символам: ");
            foreach (var m in months_n)
            {
                Console.Write(m + "  ");
            }
            Console.WriteLine();Console.WriteLine();

            var MonthsSummerAndWinter = months.Where(m => new[]{ "December", "January", "February", "June", "July", "August" }.Contains(m));

            Console.WriteLine($"Только летние и зимние месяцы: ");
            foreach (var m in MonthsSummerAndWinter)
            {
                Console.Write(m + "  ");
            }
            Console.WriteLine(); Console.WriteLine();

            Console.WriteLine($"Месяцы в алфавитном порядке: ");
            var sortMonths = months.OrderBy(m => m);
            foreach (var m in sortMonths)
            {
                Console.Write(m + "  ");
            }
            Console.WriteLine(); Console.WriteLine();

            Console.WriteLine($"Месяцы, содержащие букву u и длинной не менее 4-х символов: ");
            var letterUInMonths = months.Where(m => (m.Length >= 4) & (m.Contains("u")));
            foreach (var m in letterUInMonths)
            {
                Console.Write(m + "  ");
            }
            Console.WriteLine(); Console.WriteLine();
        }
        public static void TaskTwoThree()
        {
            Console.WriteLine(" ~~~~ Задание №2-3 ~~~~ ");
            List<Set> sets = new List<Set>();
            sets.Add(new Set());

            sets.Add(new Set(1, 2));
            sets.Add(new Set(5, 8));
            sets.Add(new Set(18, 13));

            sets.Add(new Set(7, -14, 21));
            sets.Add(new Set(2, 4, 6));
            sets.Add(new Set(10, 20, 30));

            sets.Add(new Set(15));
            sets.Add(new Set(30, 35));
            sets.Add(new Set(200, 100, 300));

            Console.WriteLine($"Создано элементов: {sets.Count}");

            Console.WriteLine("Множества с наименьшей/наибольшей суммой элементов: ");
            var setsSort = sets.OrderBy(m => m.Sum()).ToList();
            Console.WriteLine($"Наименьшая сумма: {sets[0]}");
            Console.WriteLine($"Наибольшая сумма: {sets[9]}");
            Console.WriteLine(); Console.WriteLine();

            Console.WriteLine("Множества с отрицательными элементами: ");
            var setsNegative = sets.Where(m => m.HasNegative() == true).ToList();
            foreach (var s in setsNegative)
            {
                Console.WriteLine(s + " ");
            }
            Console.WriteLine();Console.WriteLine();

            int i = 30;
            Console.Write($"Количество, множеств содержащих заданное значение ({i}): ");
            var setsWithI = sets.Where( m => m.Contains(i) == true);
            Console.WriteLine(setsWithI.Count());
            foreach (var s in setsWithI)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine(); Console.WriteLine();

            Console.Write($"Максимальное множество: ");
            Console.WriteLine(setsSort[9]);
            Console.WriteLine(); Console.WriteLine();

            Console.WriteLine($"Первое множество с заданным элементом: ");
            var fistWithI = sets.Where(m => m.ContainsFirst(i)).FirstOrDefault();
            if( fistWithI != null )
            {
                Console.WriteLine(fistWithI);
            }
            else Console.WriteLine("Множество не найдено");
            Console.WriteLine(); Console.WriteLine();

            Console.WriteLine("Упорядоченный массив множеств по первому элементу: ");
            var setsSortByFirstElement = sets.OrderBy(m => m.FirstElement()).ToList();
            foreach (var s in setsSortByFirstElement)
            { 
                Console.WriteLine(s);
            }
            Console.WriteLine(); Console.WriteLine();
        }
        public static void TaskFour()
        {
            Console.WriteLine(" ~~~~ Задание №4 ~~~~ ");

            var prod = products

                .Where(p => p.Category != "Молочные продукты" && p.Description.Length > 5)

                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.Category,
                    DescLenght = p.Description.Length
                })

                .OrderByDescending(p => p.DescLenght)

                .ThenBy(p => p.Name)

                .GroupBy(p => p.Category)

                .Select(g => new
                {
                    Category = g.Key,
                    ProductCount = g.Count(),
                    TotalSumDesc = g.Sum(p => p.DescLenght),
                    AvgDesc = g.Average(p => p.DescLenght),
                    Products = g
                        .Take(10)
                        .Select(p => $"{p.Name} ({p.Description})")
                        .ToList()
                })

                .Where(g => g.ProductCount > 0)
                .ToList();

            foreach (var p in prod)
            {
                Console.WriteLine($"\nКатегория: {p.Category}");
                Console.WriteLine($"Количество продуктов: {p.ProductCount}");
                Console.WriteLine($"Средняя длина описания: {p.AvgDesc:F1}");
                Console.WriteLine($"Продукты: {string.Join(", ", p.Products)}");
            }
            Console.WriteLine(); Console.WriteLine();
        }
        public static void TaskFive()
        {
            Console.WriteLine(" ~~~~ Задание №5 ~~~~ ");
            var productsWithCosts = products
                .Join(costProducts,
                      product => product.Name,
                      costProduct => costProduct.NameProduct,
                      (product, costProduct) => new
                      {
                          ProductName = product.Name,
                          Description = product.Description,
                          Category = product.Category,
                          TotalValue = costProduct.CostProd * 10
                      })
                .OrderByDescending(x => x.TotalValue)
                .ThenBy(x => x.ProductName)
                .ToList();
            Console.WriteLine("Продукты с ценами: ");
            Console.WriteLine();
            foreach (var p in productsWithCosts)
            {
                Console.WriteLine($"{p.ProductName,-10} | {p.Description,-20} | {p.Category,-25} | Итого: ${p.TotalValue}");
            }
            Console.WriteLine(); Console.WriteLine();
        }


        static List<Product> products = new List<Product>()
        {
            new Product("Молоко", "Вкусное", "Молочные продукты"),
            new Product("Сок", "Апельсиновый", "Напитки"),
            new Product("Вода", "Минеральная", "Напитки"),
            new Product("Багет", "Пшеничный", "Хлебобулочные изделия"),
            new Product("Йогурт", "Натуральный", "Молочные продукты")
        };
        static List<CostProduct> costProducts = new List<CostProduct>()
        {
            new CostProduct("Молоко", 2),
            new CostProduct("Сок", 4),
            new CostProduct("Вода", 1),
            new CostProduct("Багет", 2),
            new CostProduct("Йогурт", 1)
        };
        public class Product
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }

            public Product(string name, string desc, string cat) 
            {
                Name = name;
                Description = desc;
                Category = cat;
            }
        }
        public class CostProduct
        {
            public string NameProduct { get; set; }
            public int CostProd { get; set; }

            public CostProduct(string nameProd, int costProd)
            {
                NameProduct = nameProd;
                CostProd = costProd;
            }
        }


        static void Main(string[] args)
        {
            try
            {
                TaskOne();
                TaskTwoThree();
                TaskFour();
                TaskFive();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally 
            { 
                Console.WriteLine("---Исключений в программе не обнаружено---"); 
            }
        }
    }
}
