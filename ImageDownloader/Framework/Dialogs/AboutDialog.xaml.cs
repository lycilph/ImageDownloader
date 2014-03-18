using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ImageDownloader.Framework.Dialogs
{
    public partial class AboutDialog
    {
        private Func<Task> close;

        public AboutDialog(MetroWindow parent, MetroDialogSettings settings) : base(parent, settings)
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
    }
}
