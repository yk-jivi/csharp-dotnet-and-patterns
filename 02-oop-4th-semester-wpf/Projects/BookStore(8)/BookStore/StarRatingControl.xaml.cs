using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookStore
{
    public partial class StarRatingControl : UserControl
    {
        // DependencyProperty с валидацией
        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register("Rating", typeof(int), typeof(StarRatingControl),
                new FrameworkPropertyMetadata(0, OnRatingChanged), ValidateRating);

        // Событие Bubbling (всплывающее)
        public static readonly RoutedEvent RatingChangedEvent =
            EventManager.RegisterRoutedEvent("RatingChanged", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(StarRatingControl));

        public event RoutedEventHandler RatingChanged
        {
            add { AddHandler(RatingChangedEvent, value); }
            remove { RemoveHandler(RatingChangedEvent, value); }
        }

        public int Rating
        {
            get { return (int)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        public StarRatingControl()
        {
            InitializeComponent();
        }

        // Валидация: рейтинг от 0 до 5
        private static bool ValidateRating(object value)
        {
            int rating = (int)value;
            return rating >= 0 && rating <= 5;
        }

        private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (StarRatingControl)d;
            int newRating = (int)e.NewValue;
            control.UpdateStars();
        }

        private void UpdateStars()
        {
            var stars = new[] { Star1, Star2, Star3, Star4, Star5 };
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].Content = i < Rating ? "★" : "☆";
            }
        }

        private void Star_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                Rating = int.Parse(btn.Tag.ToString());

                System.Diagnostics.Debug.WriteLine($"Рейтинг изменен на: {Rating}");

                RaiseEvent(new RoutedEventArgs(RatingChangedEvent, e.OriginalSource));
            }
        }
    }
}