using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Panda.Utilities.Extensions;

namespace ImageDownloader.Model
{
    public class Settings
    {
        private const string Filename = "settings.txt";
        private const string DefaultDataFolder = "Data";

        public const int ThreadDelay = 100;
        public const int MaxThreadCount = 16;

        public static string ApplicationFolder
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        private string _DataFolder;
        public string DataFolder
        {
            get { return _DataFolder; }
            set { SetDataFolder(value); }
        }

        public bool DefaultUseCache { get; set; }
        public int DefaultCacheLifetime { get; set; }
        public bool DefaultFlattenFilename { get; set; }
        public bool DefaultOnlySubpages { get; set; }
        public List<string> DefaultExcludedExtensions { get; set; }
        public List<string> DefaultExcludedStrings { get; set; }

        public List<string> FavoriteSiteUrls { get; set; }
        public List<string> FavoriteSiteFiles { get; set; }

        // Default image extensions

        public Settings()
        {
            DefaultUseCache = true;
            DefaultCacheLifetime = 7;
            DefaultFlattenFilename = false;
            DefaultOnlySubpages = true;
            DefaultExcludedExtensions = new List<string>();
            DefaultExcludedStrings = new List<string>();

            FavoriteSiteUrls = new List<string>();
            FavoriteSiteFiles = new List<string>();

            SetDataFolder(Path.Combine(ApplicationFolder, DefaultDataFolder));
        }

        private void SetDataFolder(string folder)
        {
            _DataFolder = folder;
            Directory.CreateDirectory(folder);
        }

        public SiteOptions GetDefaultSiteOptions()
        {
            return new SiteOptions
            {
                UseCache = DefaultUseCache,
                CacheLifetime = DefaultCacheLifetime,
                FlattenFilename = DefaultFlattenFilename,
                OnlySubpages = DefaultOnlySubpages,
                ExcludedExtensions = new List<string>(DefaultExcludedExtensions),
                ExcludedStrings = new List<string>(DefaultExcludedStrings),
                Folder = DataFolder,
            };
        }

        public static Settings Load()
        {
            var path = Path.Combine(ApplicationFolder, Filename);
            return File.Exists(path) ? JsonExtensions.ReadFromFile<Settings>(path) : new Settings();
        }

        public void Save()
        {
            var path = Path.Combine(ApplicationFolder, Filename);
            JsonExtensions.WriteToFile(path, this);
        }
    }
}
