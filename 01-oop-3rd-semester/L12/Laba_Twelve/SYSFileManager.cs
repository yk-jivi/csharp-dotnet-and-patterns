using System;
using System.IO;
using System.IO.Compression;

namespace Laba_Twelve
{
    public class SYSFileManager
    {
        public static void InspectDrive(string drivepath)
        {
            try
            {
                string nameDir = "SYSInspect";
                Directory.CreateDirectory(nameDir);

                string infoFile = Path.Combine(nameDir, "sysdirinfo.txt");

                using (StreamWriter sw = new StreamWriter(infoFile))
                {
                    foreach (var dir in Directory.GetDirectories(drivepath))
                        sw.WriteLine("DIR: " + dir);
                    foreach (var file in Directory.GetFiles(drivepath))
                        sw.WriteLine("FILE: " + file);
                }

                string copyFile = Path.Combine(nameDir, "sysdirinfo_copy.txt");
                File.Copy(infoFile, copyFile, true);

                File.Delete(infoFile);

                SYSLog.WriteStringToFile("InspectDrive", drivepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка InspectDrive: " + ex.Message);
            }
        }

        public static void CopyByExtension(string sourceDir, string ext)
        {
            try
            {
                string filesDir = "SYSFiles";

                if (Directory.Exists(filesDir))
                    Directory.Delete(filesDir, true);

                Directory.CreateDirectory(filesDir);

                foreach (string file in Directory.GetFiles(sourceDir, $"*.{ext}"))
                {
                    string name = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(filesDir, name), true);
                }

                string target = Path.Combine("SYSInspect", filesDir);

                if (Directory.Exists(target))
                    Directory.Delete(target, true);

                Directory.Move(filesDir, target);

                SYSLog.WriteStringToFile("CopyByExtension", sourceDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка CopyByExtension: " + ex.Message);
            }
        }

        public static void ArchiveFiles()
        {
            try
            {
                string src = Path.Combine("SYSInspect", "SYSFiles");
                string zip = Path.Combine(Environment.CurrentDirectory, "sysarchive.zip");
                string extract = Path.Combine(Environment.CurrentDirectory, "SYSExtract");

                if (!Directory.Exists(src))
                {
                    Console.WriteLine(@"Папка SYSInspect\\SYSFiles отсутствует.");
                    return;
                }

                // ❗ Удаляем старый архив точно по абсолютному пути
                if (File.Exists(zip))
                    File.Delete(zip);

                ZipFile.CreateFromDirectory(src, zip);

                // ❗ Полностью пересоздаём папку распаковки
                if (Directory.Exists(extract))
                    Directory.Delete(extract, true);

                Directory.CreateDirectory(extract);

                ZipFile.ExtractToDirectory(zip, extract);

                SYSLog.WriteStringToFile("ArchiveFiles");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка архивирования: " + ex.Message);
            }
        }

    }
}
