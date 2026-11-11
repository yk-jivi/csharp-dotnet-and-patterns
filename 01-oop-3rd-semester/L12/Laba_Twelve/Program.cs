namespace Laba_Twelve
{
    internal class Program
    {
        public static void ClassOne()
        {
            SYSLog.WriteStringToFile("Открытие программы", "Program.cs запущен");
            SYSLog.WriteStringToFile("Записано значение", "13");
            SYSLog.WriteStringToFile("Записано значение", "18");

            var found_one = SYSLog.Find("Открытие");
            foreach (var line in found_one)
                Console.WriteLine(line);
            var found_two = SYSLog.Find("Чтение");
            foreach (var line in found_two)
                Console.WriteLine(line);
            var found_three = SYSLog.Find("Записано");
            foreach (var line in found_three)
                Console.WriteLine(line);
            Console.WriteLine(); Console.WriteLine();

        }
        public static void ClassTwo()
        {
            SYSDiskInfo.ShowAllDrives();
            Console.WriteLine(); Console.WriteLine();
        }
        public static void ClassThree()
        {
            SYSFileInfo.ShowInfo(@"D:\\Lenovo\\Документы\\Конспект по физике 10 класс.docx");
            Console.WriteLine(); Console.WriteLine();
        }
        public static void ClassFour()
        {
            SYSDirInfo.ShowInfo(".");
            Console.WriteLine(); Console.WriteLine();
        }
        public static void ClassFive()
        {
            SYSFileManager.InspectDrive("C:\\");
            SYSFileManager.CopyByExtension(".", "cs");
            SYSFileManager.ArchiveFiles();
        }

        static void Main(string[] args)
        {
            ClassOne();
            ClassTwo();
            ClassThree();
            ClassFour();
            ClassFive();
            Console.WriteLine("\nПоиск по слову 'Drive':");
            foreach (var l in SYSLog.Find("Drive"))
                Console.WriteLine(l);

            Console.WriteLine($"\nВсего записей в логе: {SYSLog.Read().Length}");

            SYSLog.KeepOnlyCurrentHour();
        }
    }
}
