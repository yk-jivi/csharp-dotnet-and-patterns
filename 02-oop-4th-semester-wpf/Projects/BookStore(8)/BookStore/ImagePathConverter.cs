using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BookStore
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imageName && !string.IsNullOrEmpty(imageName))
            {
                try
                {
                    // Вариант 1: Относительный путь к папке Images
                    string relativePath = $"Images/{imageName}";
                    if (File.Exists(relativePath))
                    {
                        return new BitmapImage(new Uri(relativePath, UriKind.Relative));
                    }

                    // Вариант 2: Абсолютный путь к папке Images в директории проекта
                    string absolutePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", imageName);
                    if (File.Exists(absolutePath))
                    {
                        return new BitmapImage(new Uri(absolutePath));
                    }

                    // Вариант 3: Ищем в папке проекта
                    string projectPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Images", imageName);
                    if (File.Exists(projectPath))
                    {
                        return new BitmapImage(new Uri(projectPath));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения {imageName}: {ex.Message}");
                }
            }

            // Загружаем изображение по умолчанию
            try
            {
                string defaultImagePath = "Images/default-book.jpg";
                if (File.Exists(defaultImagePath))
                {
                    return new BitmapImage(new Uri(defaultImagePath, UriKind.Relative));
                }

                string absoluteDefaultPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "default-book.jpg");
                if (File.Exists(absoluteDefaultPath))
                {
                    return new BitmapImage(new Uri(absoluteDefaultPath));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения по умолчанию: {ex.Message}");
            }

            return null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}