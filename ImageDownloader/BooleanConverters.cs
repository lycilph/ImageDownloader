using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ImageDownloader
{
    public abstract class BooleanConverter<T> : IValueConverter
    {
        public T TrueValue { get; set; }

        public T FalseValue { get; set; }

        protected BooleanConverter(T true_value, T false_value)
        {
            TrueValue = true_value;
            FalseValue = false_value;
        }

        public object Convert(object value, Type target_type, object parameter, CultureInfo culture)
        {
            return Equals(value, true) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type target_type, object parameter, CultureInfo culture)
        {
            return Equals(value, TrueValue);
        }
    }

    public class BooleanToImageConverter : BooleanConverter<BitmapImage>
    {
        public BooleanToImageConverter() : base(null, null) { }
    }

    public class NegatedBooleanConverter : BooleanConverter<bool>
    {
        public NegatedBooleanConverter() : base(false, true) { }
    }
}
