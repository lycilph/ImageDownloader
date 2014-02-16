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

        private bool _DebugEnabled = false;
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

            _OutputFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public void Handle(SystemMessage message)
        {
            DebugEnabled = !DebugEnabled;
        }
    }
}
