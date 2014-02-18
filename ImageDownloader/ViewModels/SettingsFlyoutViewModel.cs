using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
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
        private Settings settings;

        private string _OutputFolder;
        public string OutputFolder
        {
            get { return _OutputFolder; }
            set { this.RaiseAndSetIfChanged(ref _OutputFolder, value); }
        }

        private ObservableAsPropertyHelper<bool> _DebugEnabled;
        public bool DebugEnabled
        {
            get { return _DebugEnabled.Value; }
        }

        private bool _CachingEnabled;
        public bool CachingEnabled
        {
            get { return _CachingEnabled; }
            set { this.RaiseAndSetIfChanged(ref _CachingEnabled, value); }
        }

        [ImportingConstructor]
        public SettingsFlyoutViewModel(Settings settings) : base("Settings", Position.Right, true)
        {
            this.settings = settings;

            this.ObservableForProperty(x => x.IsOpen)
                .Where(x => x.Value)
                .Subscribe(x => Opening());

            _DebugEnabled = settings.WhenAnyValue(x => x.DebugEnabled, x => x)
                                    .ToProperty(this, x => x.DebugEnabled);
        }

        private void Opening()
        {
            OutputFolder = settings.OutputFolder;
            CachingEnabled = settings.CachingEnabled;
        }

        public void Accept()
        {
            settings.OutputFolder = OutputFolder;
            settings.CachingEnabled = CachingEnabled;

            IsOpen = false;
        }
    }
}
