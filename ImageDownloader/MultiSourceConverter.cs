using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ImageDownloader
{
    public class MultiSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type target_type, object parameter, CultureInfo culture)
        {
            var index = (int)values[values.Length - 1];

            var node = values[index] as Node;
            if (node == null || node.Image == null)
                return null;

            return new BitmapImage(new Uri(node.Image));
        }

        public object[] ConvertBack(object value, Type[] target_types, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
