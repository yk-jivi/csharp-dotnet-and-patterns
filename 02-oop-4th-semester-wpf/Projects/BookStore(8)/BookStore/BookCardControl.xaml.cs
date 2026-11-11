using System;
using System.Windows;
using System.Windows.Controls;

namespace BookStore
{
    public partial class BookCardControl : UserControl
    {
        public event EventHandler<Book> AddToCartClicked;

        public BookCardControl()
        {
            InitializeComponent();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Book book)
            {
                book.SelectedQuantity = QuantityBox.Value;
                AddToCartClicked?.Invoke(this, book);
            }
        }
        private void StarRating_RatingChanged(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== Bubbling ===");

            // Стрелки вверх показывают направление всплытия
            System.Diagnostics.Debug.WriteLine($"OriginalSource: {e.OriginalSource} ↑ Sender: {sender}");

            System.Diagnostics.Debug.WriteLine("=================");
        }
        private void NumberBox_ValueChanged(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== Tunneling ===");

            // Стрелки вниз показывают направление туннелинга
            System.Diagnostics.Debug.WriteLine($"Sender: {sender} ↓ OriginalSource: {e.OriginalSource}");

            System.Diagnostics.Debug.WriteLine("=================");
        }
    }
}