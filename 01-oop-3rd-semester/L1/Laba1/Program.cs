namespace Laba1
{
    using System.Text;

    internal class Program
    {
        static void Main(string[] args)
        {
                Console.WriteLine("---Task №1---");
                Console.Write("Введите значение для bool (true/false): ");
                bool one = bool.Parse(Console.ReadLine());
                Console.WriteLine("bool: " + one);

                Console.Write("Введите значение для byte: ");
                byte two = byte.Parse(Console.ReadLine());
                Console.WriteLine("byte: " + two);

                Console.Write("Введите значение для sbyte: ");
                sbyte three = sbyte.Parse(Console.ReadLine());
                Console.WriteLine("sbyte: " + three);

                Console.Write("Введите символ (char): ");
                char four = char.Parse(Console.ReadLine());
                Console.WriteLine("char: " + four);

                Console.Write("Введите значение для decimal: ");
                decimal five = decimal.Parse(Console.ReadLine());
                Console.WriteLine("decimal: " + five);

                Console.Write("Введите значение для double: ");
                double six = double.Parse(Console.ReadLine());
                Console.WriteLine("double: " + six);

                Console.Write("Введите значение для float: ");
                float seven = float.Parse(Console.ReadLine());
                Console.WriteLine("float: " + seven);

                Console.Write("Введите значение для int: ");
                int eight = int.Parse(Console.ReadLine());
                Console.WriteLine("int: " + eight);

                Console.Write("Введите значение для uint: ");
                uint nine = uint.Parse(Console.ReadLine());
                Console.WriteLine("uint: " + nine);

                Console.Write("Введите значение для nint: ");
                nint ten = nint.Parse(Console.ReadLine());
                Console.WriteLine("nint: " + ten);

                Console.Write("Введите значение для nuint: ");
                nuint a = nuint.Parse(Console.ReadLine());
                Console.WriteLine("nuint: " + a);

                Console.Write("Введите значение для long: ");
                long b = long.Parse(Console.ReadLine());
                Console.WriteLine("long: " + b);

                Console.Write("Введите значение для ulong: ");
                ulong c = ulong.Parse(Console.ReadLine());
                Console.WriteLine("ulong: " + c);

                Console.Write("Введите значение для short: ");
                short d = short.Parse(Console.ReadLine());
                Console.WriteLine("short: " + d);

                Console.Write("Введите значение для ushort: ");
                ushort h = ushort.Parse(Console.ReadLine());
                Console.WriteLine("ushort: " + h);

                //Неявное приведение 
                int aa = 100;
                long bb = aa; //int в long
                float cc = aa; //int в float
                double dd = aa; //int в double
                char ch = 'H';
                int cha = ch; //char в int
                              //Явное приведение
                double x = 88.88;
                int y = (int)x; //double в int, потеря дробной части
                Console.WriteLine("Потеря дробной части: " + y);
                long l = 10000000000000;
                int z = (int)l; //long в int, возможно потерять данные
                object obj = 88;
                int i = (int)obj; //если бы obj содержал, например, string, то возникло бы исключение времени выполнения (InvalidCastException)
                float f = 8.88f;
                byte by = (byte)f; //преобразование от числа с плавающей точкой к целому 8-битному типу
                decimal dec = 188.5m;
                int n = (int)dec; //дробная часть отбрасывается
                //Класс Convert - он представляет собой методы для безопасного преобразования между типами
                string str = "888";
                int num = Convert.ToInt32(str); // string в int
                bool flag = Convert.ToBoolean("true");  // string в bool
                double dVal = Convert.ToDouble(str); // string в double
                char charr = Convert.ToChar(65);    // int в char
                string s = Convert.ToString(100);   // int в string

                //Упаковка и распаковка значимых типов
                int val = 88;
                object box = val; //Упаковка

                int unbox = (int)box; // Распаковка
                Console.WriteLine(unbox);

                //Неявно типизированная переменная (var)
                var mess = "Hi!";
                Console.WriteLine(mess);

                //Работа с Nullable переменной
                int? op = null;
                if (op.HasValue)
                    Console.WriteLine($"Значение: {op.Value}");
                else
                    Console.WriteLine("Значение отсутствует");
                Console.WriteLine(op);
                op = 18;
                //Выводим присвоенное значение
                Console.WriteLine(op.GetValueOrDefault());

                Console.WriteLine();
                Console.WriteLine("---Task №2---");
                //Объявление и сравнение строковых литералов
                string str1 = "Hello";
                string str2 = "Hi";
                string str3 = "Hello";
                Console.WriteLine(str1 == str2); //выведется false, т.к. по значению они не совпадают
                Console.WriteLine(str3 == str1); //выведется true

                //операции со строками
                string str_one = "Привет,";
                string str_two = "второй";
                string str_three = "курс!";

                //Сцепление строк
                string cluth = string.Concat(str_one + " " + str_two + " " + str_three);
                Console.WriteLine(cluth);

                //Копирование
                string copy_str = string.Copy(str_two);
                Console.WriteLine(copy_str);

                //Выделение подстроки
                string substring = cluth.Substring(4, 9);
                Console.WriteLine(substring);

                //Разделение строки на слова
                string[] words = cluth.Split(' ');
                foreach (var word in words)
                    Console.WriteLine(word);

                //Вставка
                string insert = cluth.Insert(6, ", БГТУ");
                Console.WriteLine(insert);

                //Удаление
                string remove = insert.Remove(8, 6);
                Console.WriteLine(remove);

                //Интерполяция
                string title = "БГТУ";
                int year = 2025;
                string info = $"{title} {year}";
                Console.WriteLine(info);

                //Пустая и null строки
                string empty_str = "";
                string null_str = null;

                Console.WriteLine(string.IsNullOrEmpty(empty_str)); // Выведется true
                Console.WriteLine(string.IsNullOrEmpty(null_str)); //Так же

                //Можно узнать длину пустой строки
                Console.WriteLine(empty_str.Length); //0
                                                     //Console.WriteLine(null_str.Length); выдаст ошибку, т.к. null_str не имеет значения

                StringBuilder string_bild = new StringBuilder("Объект");
                string_bild.Remove(0, 3);
                string_bild.Insert(0, "--- ");
                string_bild.Append(" ---");
                Console.WriteLine(string_bild.ToString());

                Console.WriteLine();
                Console.WriteLine("---Task №3---");

                //Двумерный массив
                int[,] matrix =
                {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 888, 9 }
                };
                Console.WriteLine("Матрица: ");
                for (int ii = 0; ii < matrix.GetLength(0); ii++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        Console.Write($"{matrix[ii, j],4}");
                    }
                    Console.WriteLine();
                }

                //Одномерный массив строк
                string[] words_two = { "Я", "учусь", "в", "БГТУ" };
                Console.WriteLine("Массив строк: ");
                foreach (var word in words_two)
                {
                    Console.WriteLine(word);
                }
                Console.WriteLine($"Длина массива: {words_two.Length}");
                Console.WriteLine("Введите позицию для замены (0-3): ");
                int index = int.Parse(Console.ReadLine());

                Console.WriteLine("Введите новое значение: ");
                string new_value = Console.ReadLine();

                words_two[index] = new_value;

                Console.WriteLine("Обновлённый массив: ");
                foreach (var word in words_two)
                Console.Write(word + " ");
                Console.WriteLine();

                //Ступенчатый массив
                int[][] stepwise = new int[3][];
                stepwise[0] = new int[2];
                stepwise[1] = new int[3];
                stepwise[2] = new int[4];

                for (int ii = 0; ii < stepwise.Length; ii++)
                {
                    Console.WriteLine($"Введите {stepwise[ii].Length} элементов для строки {ii}: ");
                    for (int jj = 0; jj < stepwise[ii].Length; jj++)
                    {
                        stepwise[ii][jj] = int.Parse(Console.ReadLine());
                    }
                }

                //Вывод полученного массива
                for (int ii = 0; ii < stepwise.Length; ii++)
                {
                    Console.Write($"Строка {ii}: ");
                    foreach (var value in stepwise[ii])
                    {
                        Console.Write(value + " ");
                    }
                    Console.WriteLine();
                }

                //Неявно типизированные переменные (var)
                var numbers = new[] { 1, 2, 3, };
                var name = "Yuliya";
                Console.Write("Массив: ");
                foreach (var numm in numbers)
                {
                    Console.Write(numm + " ");
                }
                Console.WriteLine();

                Console.WriteLine($"Строка: {name}");

                Console.WriteLine();
                Console.WriteLine("---Task №4---");

                //Задание кортежа из пяти элементов
                var tuple = (88, "BSTU", 'Y', "World", 70000000UL);
                //Вывод целиком
                Console.WriteLine($"Кортеж целиком: {tuple}");
                //Выборочно
                Console.WriteLine($"Элементы 1, 3, 4: {tuple.Item1}, {tuple.Item3}, {tuple.Item4}");

                //Полная распаковка кортежа
                (int numberr, string strr, char letter, string worldd, ulong population) = tuple;

                //Частичная распаковка
                (_, _, char symb, string ww, _) = tuple;
                Console.WriteLine($"Символ: {symb}, Слово: {ww}");

                //Сравнение кортежей
                var tuple1 = (1, "Hello", 'y');
                var tuple2 = (1, "Hello", 'y');

                Console.WriteLine(tuple1 == tuple2);
                Console.WriteLine(tuple1 != tuple2);

                //Локальная функция
                int[] numberss = { 88, 1, 4 };
                string text = "Текст";

                (int max, int min, int sum, char first_symb) localFunction(int[] arr, string text)
                {
                    int max = arr.Max();
                    int min = arr.Min();
                    int sum = arr.Sum();
                    char first_symb = string.IsNullOrEmpty(text) ? '-' : text[0];
                    return (max, min, sum, first_symb);
                }

                var res = localFunction(numberss, text);
                Console.WriteLine($"Maкс.: {res.max}, Мин: {res.min}, Сумма: {res.sum}, Первая буква: {res.first_symb}");

                //Checked и unchecked

                checkedFunc();
                uncheckedFunc();

                //Функция checked
                void checkedFunc()
                {
                    try
                    {
                        checked
                        {
                            int maxx = int.MaxValue;
                            int over = maxx + 1; // произойдёт переполнение, покажет исключение при выводе
                            Console.WriteLine($"Checked: {over}");

                        }
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Переполнение в checked");
                    }
                }

                //Функция unchecked
                void uncheckedFunc()
                {
                    unchecked
                    {
                        int maxx = int.MaxValue;
                        int over = maxx + 1; //произойдёт переполнение, но оно проигнорируется, число станет отрицательным, перевернётся
                        Console.WriteLine($"Unchecked: {over}");
                    }
                }
            }
        }
    }