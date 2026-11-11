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

        private void AddToCartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Book book)
            {
                AddToCartClicked?.Invoke(this, book);
            }
        }
    }
}