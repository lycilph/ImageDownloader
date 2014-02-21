using ImageDownloader.Utils;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ImageDownloader.Controls
{
    public partial class AboutDialog
    {
        private Func<Task> close;

        public AboutDialog(MetroWindow parent) : base(parent, parent.MetroDialogOptions)
        {
            InitializeComponent();
        }

        public void Initialize(Func<Task> close)
        {
            this.close = close;
        }

        private async void OkClick(object sender, RoutedEventArgs e)
        {
            await close();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (close == null)
                throw new InvalidOperationException("Initializ was not called!");
        }
    }
}
