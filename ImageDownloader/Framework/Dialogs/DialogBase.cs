using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageDownloader.Framework.Dialogs
{
    public class DialogBase : BaseMetroDialog
    {
        private Func<Task> close;

        public DialogBase(MetroWindow parent, MetroDialogSettings settings) : base(parent, settings)
        {
            close = () => parent.HideMetroDialogAsync(this);
        }

        protected async void OkClick(object sender, RoutedEventArgs e)
        {
            await close();
        }
    }
}
