using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookStore
{
    public class CartItem : INotifyPropertyChanged
    {
        private Book? _book;
        private int _quantity = 1;

        public Book Book
        {
            get { return _book!; }
            set { _book = value; OnPropertyChanged(); }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public decimal TotalPrice
        {
            get { return (Book?.Price ?? 0) * Quantity; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}