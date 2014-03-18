using Caliburn.Micro;
using ImageDownloader.Framework.Dialogs;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace ImageDownloader.Framework.Services
{
    public static class WindowManagerExtensions
    {
        public static Task ShowAboutDialog(this IWindowManager window_manager)
        {
            var window = Application.Current.Windows.OfType<MetroWindow>().First();
            var settings = new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Theme };
            var dialog = new AboutDialog(window, settings);

            dialog.Initialize(() => window.HideMetroDialogAsync(dialog));
            return window.ShowMetroDialogAsync(dialog);
        }
    }
}
