using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BookStore
{
    public class User : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Language { get; set; } = "ru";
        public string Theme { get; set; } = "DefaultTheme";
        public string Role { get; set; } = "User";

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}