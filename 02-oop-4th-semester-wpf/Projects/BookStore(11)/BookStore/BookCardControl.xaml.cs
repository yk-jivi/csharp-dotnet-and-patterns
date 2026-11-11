using System;
using System.Windows;
using System.Windows.Controls;

namespace BookStore
{
    public partial class BookCardControl : UserControl
    {
        public BookCardControl()
        {
            InitializeComponent();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Book book)
            {
                // Получаем NumberBox из ControlTemplate или ищем по имени
                var nb = FindName("QuantityBox") as NumberBox;
                if (nb != null)
                    book.SelectedQuantity = nb.Value;
                else
                    book.SelectedQuantity = 1;

                // Находим MainWindow и вызываем команду
                var window = Application.Current.MainWindow as MainWindow;
                if (window?.DataContext is MainViewModel vm)
                {
                    vm.AddToCartCommand.Execute(book);
                }
            }
        }

        private void StarRating_RatingChanged(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== Bubbling ===");
            System.Diagnostics.Debug.WriteLine($"OriginalSource: {e.OriginalSource} ↑ Sender: {sender}");
            System.Diagnostics.Debug.WriteLine("=================");
        }

        private void NumberBox_ValueChanged(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== Tunneling ===");
            System.Diagnostics.Debug.WriteLine($"Sender: {sender} ↓ OriginalSource: {e.OriginalSource}");
            System.Diagnostics.Debug.WriteLine("=================");
        }
    }
}