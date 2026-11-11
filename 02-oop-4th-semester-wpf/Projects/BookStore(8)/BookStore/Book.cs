using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookStore
{
    public class Book : INotifyPropertyChanged
    {
        private int _id;
        private string? _title;
        private string? _author;
        private string? _category;
        private int _year;
        private int _pages;
        private decimal _price;
        private string? _description;
        private string? _imagePath;
        public byte[]? ImageBytes { get; set; }

        //---К лабе №7
        private int _rating;
        private int _selectedQuantity = 1;
        private int _ratingCount;
        public int Rating
        {
            get { return _rating; }
            set { _rating = value; OnPropertyChanged(); }
        }
        public int SelectedQuantity
        {
            get { return _selectedQuantity; }
            set { _selectedQuantity = value; OnPropertyChanged(); }
        }
        public int RatingCount
        {
            get { return _ratingCount; }
            set { _ratingCount = value; OnPropertyChanged(); }
        }
        //---------

        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get { return _title!; }
            set { _title = value; OnPropertyChanged(); }
        }

        public string Author
        {
            get { return _author!; }
            set { _author = value; OnPropertyChanged(); }
        }

        public string Category
        {
            get { return _category!; }
            set { _category = value; OnPropertyChanged(); }
        }

        public int Year
        {
            get { return _year; }
            set { _year = value; OnPropertyChanged(); }
        }

        public int Pages
        {
            get { return _pages; }
            set { _pages = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get { return _price; }
            set { _price = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get { return _description!; }
            set { _description = value; OnPropertyChanged(); }
        }

        public string ImagePath
        {
            get { return _imagePath!; }
            set { _imagePath = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}