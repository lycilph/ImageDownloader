using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Core;

namespace ImageDownloader
{
    public class Settings
    {
        private const string Filename = "settings.txt";
        private const string DefaultDataFolder = "Data";

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

        public bool UseCache { get; set; }

        public List<string> FavoriteSiteUrls { get; set; }
        public List<string> FavoriteSiteFiles { get; set; }

        // Default image extensions

        public Settings()
        {
            UseCache = true;
            FavoriteSiteUrls = new List<string>();
            FavoriteSiteFiles = new List<string>();

            SetDataFolder(Path.Combine(ApplicationFolder, DefaultDataFolder));
        }

        private void SetDataFolder(string folder)
        {
            _DataFolder = folder;
            Directory.CreateDirectory(folder);
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
