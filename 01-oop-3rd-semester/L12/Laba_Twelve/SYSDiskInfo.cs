using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Laba_Twelve
{
    public class SYSDiskInfo
    {
        public static void ShowFreePlace(string drivename)
        {
            try
            {
                DriveInfo d = new DriveInfo(drivename);
                Console.WriteLine($"Свободно: {d.AvailableFreeSpace / 1024 / 1024 } МБ");
                SYSLog.WriteStringToFile("Проверка свободного места", drivename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        public static void ShowFileSysten(string drivename)
        {
            try
            {
                DriveInfo d = new DriveInfo(drivename);
                Console.WriteLine($"Файловая система: {d.DriveFormat}");
                SYSLog.WriteStringToFile("Проверка файловой системы", drivename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        public static void ShowAllDrives()
        {
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                Console.WriteLine(
                    $"Имя: {d.Name}, Объём {d.TotalSize / 1024 / 1024 } МБ, " +
                    $"Доступно: {d.TotalFreeSpace / 1024 / 1024 } МБ, Метка: {d.VolumeLabel}"
                    );
            }
            SYSLog.WriteStringToFile("Просмотр всей информации о дисках");
        }
    }
}
