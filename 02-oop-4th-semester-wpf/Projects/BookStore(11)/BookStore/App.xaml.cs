using System.Windows;

namespace BookStore
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var viewModel = new MainViewModel();
            var mainWindow = new MainWindow { DataContext = viewModel };

            viewModel.LoadBooksCommand.Execute(null);

            mainWindow.Show();
        }
    }
}