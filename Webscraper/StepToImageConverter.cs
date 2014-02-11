using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Webscraper
{
    [ValueConversion(typeof(Step), typeof(object))]
    public class StepToImageConverter : IValueConverter
    {
        public Image NextImage { get; set; }
        public Image DownloadImage { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var step = value as Step;
            if (step == null)
                return DependencyProperty.UnsetValue;

            return (step is ImagesStep ? DownloadImage : NextImage);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
