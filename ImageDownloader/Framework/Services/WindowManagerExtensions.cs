using Caliburn.Micro;
using ImageDownloader.Framework.Dialogs;
using ImageDownloader.Framework.Dialogs.About;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ImageDownloader.Framework.Services
{
    public static class WindowManagerExtensions
    {
        public static Task ShowAboutDialog(this IWindowManager window_manager)
        {
            var window = Application.Current.Windows.OfType<MetroWindow>().First();
            var settings = new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Theme };
            var dialog = new AboutDialog(window, settings);
            return window.ShowMetroDialogAsync(dialog);
        }
    }
}
