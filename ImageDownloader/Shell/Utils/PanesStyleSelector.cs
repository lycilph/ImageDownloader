using ImageDownloader.Test.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ImageDownloader.Shell.Utils
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle { get; set; }

        public Style ContentStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ToolViewModel)
                return ToolStyle;

            if (item is ContentViewModel)
                return ContentStyle;

            return base.SelectStyle(item, container);
        }
    }
}
