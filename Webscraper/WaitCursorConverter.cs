using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Webscraper
{
    [ValueConversion(typeof(bool), typeof(Cursor))]
    public class WaitCursorConverter : MarkupExtension, IValueConverter
    {
        private static WaitCursorConverter instance = new WaitCursorConverter();

        public WaitCursorConverter() {}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var busy = (bool)value;
            if (busy)
                return Cursors.Wait;
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance;
        }
    }
}
