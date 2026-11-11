using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BookStore
{
    public partial class PersonalAccountWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public PersonalAccountWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            LoadUserData();
        }

        private void LoadUserData()
        {
            UsernameInput.Text = _viewModel.CurrentUser.Username;
            EmailInput.Text = _viewModel.CurrentUser.Email;
            PhoneInput.Text = _viewModel.CurrentUser.Phone;

            foreach (ComboBoxItem item in LanguageCombo.Items)
            {
                if (item.Tag?.ToString() == _viewModel.CurrentUser.Language)
                {
                    LanguageCombo.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in ThemeCombo.Items)
            {
                if (item.Tag?.ToString() == _viewModel.CurrentUser.Theme)
                {
                    ThemeCombo.SelectedItem = item;
                    break;
                }
            }
        }

        private void LanguageCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LanguageCombo.SelectedItem is ComboBoxItem item && item.Tag is string lang)
            {
                _viewModel.ChangeLanguage(lang);
            }
        }

        private void ThemeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ThemeCombo.SelectedItem is ComboBoxItem item && item.Tag is string theme)
            {
                _viewModel.ChangeTheme(theme);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CurrentUser.Username = UsernameInput.Text;
            _viewModel.CurrentUser.Email = EmailInput.Text;
            _viewModel.CurrentUser.Phone = PhoneInput.Text;
            _viewModel.OnPropertyChanged(nameof(_viewModel.CurrentUser));
            MessageBox.Show("Данные сохранены!", "Успех");
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void UpdateTheme()
        {
            Background = (Brush)Application.Current.Resources["BackgroundBrush"];
        }
    }
}