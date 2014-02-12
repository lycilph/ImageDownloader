using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ImageDownloader.Models;
using System.ComponentModel;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(FlyoutBase))]
    public class SettingsFlyoutViewModel : FlyoutBase
    {
        public Settings Settings { get; set; }

        [ImportingConstructor]
        public SettingsFlyoutViewModel(Settings settings) : base("Settings", Position.Right, true)
        {
            Settings = settings;

            // List for property changed events on the settings model and forward them (using a weak event handler to avoid memory leaks)
            PropertyChangedEventManager.AddHandler(settings, ForwardPropertyNotifications, string.Empty);
        }
        
        private void ForwardPropertyNotifications(object sender, PropertyChangedEventArgs e)
        {
            raisePropertyChanged(e.PropertyName);
        }
    }
}
