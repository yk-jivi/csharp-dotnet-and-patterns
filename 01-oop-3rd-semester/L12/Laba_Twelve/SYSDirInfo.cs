using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laba_Twelve
{
    public class SYSDirInfo
    {
        public static void ShowInfo(string dirpath)
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo(dirpath);
                Console.WriteLine($"Количество файлов: {d.GetFiles().Length}");
                Console.WriteLine($"Время создания: {d.CreationTime}");
                Console.WriteLine($"Количество поддиректориев: {d.GetDirectories().Length}");
                Console.WriteLine("Список родительских директориев: ");
                DirectoryInfo? temp = d.Parent;
                while (temp != null)
                {
                    Console.WriteLine(" -> " + temp.FullName);
                    temp = temp.Parent;
                }
                SYSLog.WriteStringToFile("Просмотр информации о конкретном директории", dirpath);
                Console.WriteLine(); Console.WriteLine();
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
    }
}
