using ReactiveUI;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace ImageDownloader.Models
{
    [Export(typeof(Settings))]
    public class Settings : ReactiveObject
    {
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

        public Settings()
        {
            _OutputFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
