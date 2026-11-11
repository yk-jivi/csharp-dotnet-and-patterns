using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba_Twelve
{
    public class SYSFileInfo
    {
        public static void ShowInfo(string filepath)
        { 
            try
            {
                FileInfo f = new FileInfo(filepath);
                Console.WriteLine($"Полный путь: {f.FullName}");
                Console.WriteLine($"Размер файла: {f.Length} битов");
                Console.WriteLine($"Имя файла: {f.Name}");
                Console.WriteLine($"Дата создания: {f.CreationTime}");
                Console.WriteLine($"Дата изменения: {f.LastWriteTime}");

                SYSLog.WriteStringToFile("Просмотр информации о файле", filepath);
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Ошибка: " +  ex.Message);
            }
        }
    }
}
