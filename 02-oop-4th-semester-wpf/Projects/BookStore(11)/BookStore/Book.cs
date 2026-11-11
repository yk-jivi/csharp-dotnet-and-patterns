using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace BookStore
{
    public class Book : INotifyPropertyChanged
    {
        private int _id;
        private string _title = "";
        private string _author = "";
        private int _year;
        private int _pages;
        private decimal _price;
        private string _description = "";
        private string _imagePath = "";
        private int _rating;
        private int _selectedQuantity = 1;

        [Key]
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public string Author
        {
            get => _author;
            set { _author = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public int Year
        {
            get => _year;
            set { _year = value; OnPropertyChanged(); }
        }

        public int Pages
        {
            get => _pages;
            set { _pages = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        [Column("ImageData")]
        public byte[]? ImageBytes { get; set; }

        // Связь с категорией
        public int? CategoryId { get; set; }

        public virtual Category? CategoryEntity { get; set; }

        // Для отображения в UI
        [NotMapped]
        public string Category
        {
            get => CategoryEntity?.Name ?? "Без категории";
            set { }
        }

        [NotMapped]
        public int Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public int SelectedQuantity
        {
            get => _selectedQuantity;
            set { _selectedQuantity = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}