using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace Webscraper
{
    public class Settings : ObservableObject
    {
        private string _Website = string.Empty;
        public string Website
        {
            get { return _Website; }
            set
            {
                if (_Website == value) return;
                _Website = value;
                NotifyPropertyChanged();
            }
        }

        private string _OutputDir = string.Empty;
        public string OutputDir
        {
            get { return _OutputDir; }
            set
            {
                if (_OutputDir == value) return;
                _OutputDir = value;
                NotifyPropertyChanged();
            }
        }

        private string _CacheDir = string.Empty;
        public string CacheDir
        {
            get { return _CacheDir; }
            set
            {
                if (_CacheDir == value) return;
                _CacheDir = value;
                NotifyPropertyChanged();
            }
        }

        private bool _UseCache;
        public bool UseCache
        {
            get { return _UseCache; }
            set
            {
                if (_UseCache == value) return;
                _UseCache = value;
                NotifyPropertyChanged();
            }
        }

        private int _MinWidth = 0;
        public int MinWidth
        {
            get { return _MinWidth; }
            set
            {
                if (_MinWidth == value) return;
                _MinWidth = value;
                NotifyPropertyChanged();
            }
        }

        private int _MaxWidth = 0;
        public int MaxWidth
        {
            get { return _MaxWidth; }
            set
            {
                if (_MaxWidth == value) return;
                _MaxWidth = value;
                NotifyPropertyChanged();
            }
        }

        private int _MinHeight = 0;
        public int MinHeight
        {
            get { return _MinHeight; }
            set
            {
                if (_MinHeight == value) return;
                _MinHeight = value;
                NotifyPropertyChanged();
            }
        }

        private int _MaxHeight = 0;
        public int MaxHeight
        {
            get { return _MaxHeight; }
            set
            {
                if (_MaxHeight == value) return;
                _MaxHeight = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ItemViewModel> _ExcludedParts = new ObservableCollection<ItemViewModel>();
        public ObservableCollection<ItemViewModel> ExcludedParts
        {
            get { return _ExcludedParts; }
            set
            {
                if (_ExcludedParts == value) return;
                _ExcludedParts = value;
                NotifyPropertyChanged();
            }
        }

        public static string GetSettingsFilename()
        {
            return Path.Combine(FileUtils.GetExeDirectory(), "Settings.txt");
        }

        public static Settings Load(string filename)
        {
            var settings = new Settings();

            if (File.Exists(filename))
            {
                using (var fs = File.Open(filename, FileMode.Open))
                using (var sw = new StreamReader(fs))
                {
                    var json = sw.ReadToEnd();
                    settings = JsonConvert.DeserializeObject<Settings>(json);
                }
            }

            return settings;
        }

        public static void Save(Settings settings, string filename)
        {
            using (var fs = File.Open(filename, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                sw.Write(json);
            }
        }
    }
}
