using Caliburn.Micro;
using ImageDownloader.Messages;
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

        private string _CacheFolder;
        public string CacheFolder
        {
            get { return _CacheFolder; }
            set { this.RaiseAndSetIfChanged(ref _CacheFolder, value); }
        }

        private bool _CachingEnabled;
        public bool CachingEnabled
        {
            get { return _CachingEnabled; }
            set { this.RaiseAndSetIfChanged(ref _CachingEnabled, value); }
        }

        private bool _DebugEnabled;
        public bool DebugEnabled
        {
            get { return _DebugEnabled; }
            set { this.RaiseAndSetIfChanged(ref _DebugEnabled, value); }
        }

        public Settings()
        {
            var root_folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _OutputFolder = Path.Combine(root_folder, "Images");
            _CacheFolder = Path.Combine(root_folder, "Cache");
            _CachingEnabled = true;
            _DebugEnabled = true;
        }

        public void ToggleDebug()
        {
            DebugEnabled = !DebugEnabled;
        }
    }
}
