using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(FlyoutBase))]
    public class SettingsFlyoutViewModel : FlyoutBase
    {
        public SettingsFlyoutViewModel() : base("Settings", Position.Right, true) {}
    }
}
