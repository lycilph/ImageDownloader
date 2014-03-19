using ImageDownloader.Core;
using MahApps.Metro.Controls;
using System.ComponentModel.Composition;

namespace ImageDownloader.Flyouts.Settings.ViewModels
{
    [Export(typeof(IFlyout))]
    [Export(typeof(ISettings))]
    public class SettingsFlyoutViewModel : FlyoutBase, ISettings
    {
        public SettingsFlyoutViewModel() : base("Settings", Position.Right) { }
    }
}
