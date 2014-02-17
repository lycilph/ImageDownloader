using Caliburn.Micro;
using ImageDownloader.Messages;
using ReactiveUI;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace ImageDownloader.Models
{
    [Export(typeof(Settings))]
    public class Settings : ReactiveObject, IHandle<SystemMessage>
    {
        private IEventAggregator event_aggregator;

        private string _OutputFolder;
        public string OutputFolder
        {
            get { return _OutputFolder; }
            set { this.RaiseAndSetIfChanged(ref _OutputFolder, value); }
        }

        private string _CacheFolder;
        public string CacheFolder
        {
            get { return _CacheFolder; }
            set { this.RaiseAndSetIfChanged(ref _CacheFolder, value); }
        }

        private bool _DebugEnabled = true;
        public bool DebugEnabled
        {
            get { return _DebugEnabled; }
            set { this.RaiseAndSetIfChanged(ref _DebugEnabled, value); }
        }

        [ImportingConstructor]
        public Settings(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;
            event_aggregator.Subscribe(this);

            var root_folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _OutputFolder = Path.Combine(root_folder, "Images");
            _CacheFolder = Path.Combine(root_folder, "Cache");

            if (!Directory.Exists(_CacheFolder))
                Directory.CreateDirectory(_CacheFolder);
        }

        public void Handle(SystemMessage message)
        {
            DebugEnabled = !DebugEnabled;
        }
    }
}
