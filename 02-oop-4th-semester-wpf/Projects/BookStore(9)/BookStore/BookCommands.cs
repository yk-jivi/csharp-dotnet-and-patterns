using System.Windows.Input;

namespace BookStore
{
    public static class BookCommands
    {
        public static readonly RoutedUICommand ClearCartCommand =
            new RoutedUICommand("Очистить корзину", "ClearCart", typeof(BookCommands),
                new InputGestureCollection { new KeyGesture(Key.Delete, ModifierKeys.Control) });
    }
}