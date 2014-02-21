using ImageDownloader.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ImageDownloader.Utils
{
    public static class DialogService
    {
        public static Task<MessageDialogResult> ShowMetroMessageBox(string title, string message)
        {
            var window = GetMainWindow();
            return window.ShowMessageAsync(title, message);
        }

        public static Task ShowMetroDialog(BaseMetroDialog dialog)
        {
            var window = GetMainWindow();
            return window.ShowMetroDialogAsync(dialog);
        }

        public static Task ShowAboutDialog()
        {
            var window = GetMainWindow();
            var dialog = new AboutDialog(window);
            dialog.Initialize(() => window.HideMetroDialogAsync(dialog));
            return window.ShowMetroDialogAsync(dialog);
        }

        private static MetroWindow GetMainWindow()
        {
            var window = Application.Current.MainWindow as MetroWindow;
            if (window == null)
                throw new InvalidOperationException("The main window must be MetroWindow");
            return window;
        }
    }
}
