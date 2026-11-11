using System;
using System.Globalization;
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
                    string uri = $"D:/University_2nd_year/4th_semester/Object-oriented_programming/Projects/BookStore/BookStore/Images/{imageName}";
                    return new BitmapImage(new Uri(uri));
                }
                catch
                {
                }
            }
            try
            {
                return new BitmapImage(new Uri($"D:/University_2nd_year/4th_semester/Object-oriented_programming/Projects/BookStore/BookStore/Images/default-book.jpg"));
            }
            catch
            {
                return null!;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}