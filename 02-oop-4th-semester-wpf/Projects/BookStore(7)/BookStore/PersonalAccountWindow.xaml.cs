using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace BookStore
{
    public partial class PersonalAccountWindow : Window
    {
        private MainWindow _mainWindow;

        public PersonalAccountWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadUserData();
        }

        private void LoadUserData()
        {
            UsernameInput.Text = _mainWindow.CurrentUser.Username;
            EmailInput.Text = _mainWindow.CurrentUser.Email;
            PhoneInput.Text = _mainWindow.CurrentUser.Phone;

            foreach (ComboBoxItem item in LanguageCombo.Items)
            {
                if (item.Tag.ToString() == _mainWindow.CurrentUser.Language)
                {
                    LanguageCombo.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in ThemeCombo.Items)
            {
                if (item.Tag.ToString() == _mainWindow.CurrentUser.Theme)
                {
                    ThemeCombo.SelectedItem = item;
                    break;
                }
            }
        }

        private bool ValidateData()
        {
            // Проверка имени пользователя
            if (string.IsNullOrWhiteSpace(UsernameInput.Text))
            {
                MessageBox.Show("Введите имя пользователя!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameInput.Focus();
                return false;
            }

            if (UsernameInput.Text.Length < 2)
            {
                MessageBox.Show("Имя пользователя должно содержать минимум 2 символа!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameInput.Focus();
                return false;
            }

            // Проверка email
            if (string.IsNullOrWhiteSpace(EmailInput.Text))
            {
                MessageBox.Show("Введите email!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailInput.Focus();
                return false;
            }

            if (!EmailInput.Text.Contains("@") || !EmailInput.Text.Contains("."))
            {
                MessageBox.Show("Введите корректный email (пример: name@mail.com)!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailInput.Focus();
                return false;
            }

            // Проверка телефона
            if (string.IsNullOrWhiteSpace(PhoneInput.Text))
            {
                MessageBox.Show("Введите номер телефона!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneInput.Focus();
                return false;
            }

            // Простая проверка телефона (должен содержать цифры)
            string digits = Regex.Replace(PhoneInput.Text, @"[^\d]", "");
            if (digits.Length < 10 || digits.Length > 11)
            {
                MessageBox.Show("Введите корректный номер телефона (10-11 цифр)!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneInput.Focus();
                return false;
            }

            return true;
        }

        public void UpdateTheme()
        {
            this.Background = (System.Windows.Media.Brush)Application.Current.Resources["BackgroundBrush"];
            this.Resources = null;
            this.Resources = Application.Current.Resources;
        }

        private void LanguageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageCombo.SelectedItem is ComboBoxItem selected)
            {
                string lang = selected.Tag.ToString();
                _mainWindow.ChangeLanguage(lang);
                _mainWindow.CurrentUser.Language = lang;
            }
        }

        private void ThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeCombo.SelectedItem is ComboBoxItem selected)
            {
                string theme = selected.Tag.ToString();
                _mainWindow.ChangeTheme(theme);
                _mainWindow.CurrentUser.Theme = theme;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем данные перед сохранением
            if (!ValidateData())
            {
                return;
            }

            // Сохраняем данные
            _mainWindow.CurrentUser.Username = UsernameInput.Text;
            _mainWindow.CurrentUser.Email = EmailInput.Text;
            _mainWindow.CurrentUser.Phone = PhoneInput.Text;

            MessageBox.Show("Данные успешно сохранены!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}