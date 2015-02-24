using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ImageDownloader.Screens.Browser
{
    public class MultiBooleanToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type target_type, object parameter, CultureInfo culture)
        {
            return values.Cast<bool>().Any(v => v) ? Visibility.Visible : Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] target_types, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
