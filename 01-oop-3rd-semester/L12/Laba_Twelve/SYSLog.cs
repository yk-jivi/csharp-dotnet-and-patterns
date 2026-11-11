using System;
using System.IO;
using System.Linq;

namespace Laba_Twelve
{
    public static class SYSLog
    {
        private static readonly string logFile = Path.Combine(Environment.CurrentDirectory, "SYSlogfile.txt");

        public static void WriteStringToFile(string action, string details = "")
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(logFile, true))
                {
                    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {action} | {details}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи в лог: {ex.Message}");
            }
        }

        public static string[] Read()
        {
            try
            {
                if (!File.Exists(logFile))
                    return Array.Empty<string>();

                return File.ReadAllLines(logFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения лога: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        public static string[] Find(string word)
        {
            try
            {
                return Read().Where(line => line.Contains(word)).ToArray();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        public static void KeepOnlyCurrentHour()
        {
            try
            {
                var now = DateTime.Now;
                var result = Read()
                    .Where(l => l.StartsWith(now.ToString("yyyy-MM-dd HH")))
                    .ToArray();

                File.WriteAllLines(logFile, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка очистки лога: " + ex.Message);
            }
        }
    }
}
