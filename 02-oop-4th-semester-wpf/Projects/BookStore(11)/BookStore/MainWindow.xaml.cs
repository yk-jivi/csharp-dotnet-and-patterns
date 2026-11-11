using System.Windows;
using System.Windows.Input;

namespace BookStore
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Оставлено для совместимости с BookCommands.ClearCartCommand
        private void ClearCart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                vm.ClearCartCommand.Execute(null);
        }

        private void ClearCart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                e.CanExecute = vm.Cart.Count > 0;
            else
                e.CanExecute = false;
        }
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            string searchPlaceholder = (string)Application.Current.Resources["SearchPlaceholder"];
            if (SearchBox.Text == searchPlaceholder)
                SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
                SearchBox.Text = (string)Application.Current.Resources["SearchPlaceholder"];
        }
    }
}