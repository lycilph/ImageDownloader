using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ImageDownloader.Modules.Views
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }
        public bool Collapse { get; set; }

        public BooleanToVisibilityConverter()
        {
            Invert = false;
            Collapse = true;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = (bool) value;
            if (Invert)
                result = !result;

            var state_when_false = (Collapse ? Visibility.Collapsed : Visibility.Hidden);

            return (result ? Visibility.Visible : state_when_false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
