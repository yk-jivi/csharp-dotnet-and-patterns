using System.Windows;
using System.Windows.Controls;

namespace BookStore
{
    public partial class NumberBox : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(NumberBox),
                new FrameworkPropertyMetadata(1, OnValueChanged, CoerceValue), ValidateValue);

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(int), typeof(NumberBox),
                new PropertyMetadata(1, null, CoerceMin));

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(NumberBox),
                new PropertyMetadata(10, null, CoerceMax));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Tunnel,
                typeof(RoutedEventHandler), typeof(NumberBox));

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public NumberBox()
        {
            InitializeComponent();
        }

        private static bool ValidateValue(object value)
        {
            int val = (int)value;
            return val >= 0;
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var control = (NumberBox)d;
            int val = (int)baseValue;

            if (val < control.Min) return control.Min;
            if (val > control.Max) return control.Max;
            return val;
        }

        private static object CoerceMin(DependencyObject d, object baseValue)
        {
            var control = (NumberBox)d;
            int min = (int)baseValue;
            if (min > control.Max) return control.Max;
            if (min < 0) return 0;
            return min;
        }

        private static object CoerceMax(DependencyObject d, object baseValue)
        {
            var control = (NumberBox)d;
            int max = (int)baseValue;
            if (max < control.Min) return control.Min;
            return max;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumberBox)d;
            control.RaiseEvent(new RoutedEventArgs(ValueChangedEvent, control));
        }

        private void Increase_Click(object sender, RoutedEventArgs e)
        {
            if (Value < Max) Value++;

            RaiseEvent(new RoutedEventArgs(ValueChangedEvent, sender));
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            if (Value > Min) Value--;

            RaiseEvent(new RoutedEventArgs(ValueChangedEvent, sender));
        }
    }
}