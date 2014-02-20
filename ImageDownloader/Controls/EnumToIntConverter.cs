using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImageDownloader.Controls
{
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Enum))
            {
                throw new ArgumentException("Value must be an enum", "value");
            }

            var output = (int)value;
            

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = parameter as Type;
            if (type == null)
                throw new ArgumentException("Value must be a type", "parameter");

            return Enum.Parse(type, value.ToString());
        }
    }
}
