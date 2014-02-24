using Caliburn.Micro;
using ImageDownloader.Interfaces;
using ImageDownloader.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace ImageDownloader.Utils
{
    [Export(typeof(ICache))]
    public class Cache : ICache
    {
        private static ILog log = LogManager.GetLog(typeof(Cache));

        private Settings settings;
        private string base_url;
        private bool dirty;
        private Dictionary<string, string> data = new Dictionary<string, string>();

        [ImportingConstructor]
        public Cache(Settings settings)
        {
            this.settings = settings;
        }

        public void Initialize(string url)
        {
            if (base_url == url)
                return;

            data.Clear();
            base_url = url;
            dirty = false;

            if (!settings.CachingEnabled)
                return;

            var filename = GetFilename(base_url);
            var filepath = Path.Combine(settings.CacheFolder, filename);

            if (!File.Exists(filepath))
                return;

            using (var archive = ZipFile.OpenRead(filepath))
            {
                var entry = archive.GetEntry("dictionary.cache");
                using (var sw = new StreamReader(entry.Open()))
                {
                    var json = sw.ReadToEnd();
                    data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
            }
        }

        public void Update()
        {
            if (!(settings.CachingEnabled && dirty))
                return;

            var filename = GetFilename(base_url);
            var filepath = Path.Combine(settings.CacheFolder, filename);

            if (!string.IsNullOrWhiteSpace(settings.CacheFolder) && !Directory.Exists(settings.CacheFolder))
                Directory.CreateDirectory(settings.CacheFolder);

            using (var fs = File.Open(filepath, FileMode.Create))
            using (var archive = new ZipArchive(fs, ZipArchiveMode.Update))
            {
                var entry = archive.CreateEntry("dictionary.cache");

                using (var sw = new StreamWriter(entry.Open()))
                {
                    var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    sw.Write(json);
                }
            }
        }

        public void Clear()
        {
            if (!string.IsNullOrWhiteSpace(settings.CacheFolder) && Directory.Exists(settings.CacheFolder))
                Directory.Delete(settings.CacheFolder, true);
        }

        public string Get(string url)
        {
            if (data.ContainsKey(url))
                return data[url];

            var page = string.Empty;
            try
            {
                using (var client = new WebClient())
                {
                    page = client.DownloadString(url);
                    AddCacheEntry(url, page);
                }
            }
            catch (WebException we)
            {
                log.Warn("Exception for \"{0}\" - {1}", url, we.Message);
                AddCacheEntry(url, page);
            }
            return page;
        }

        private void AddCacheEntry(string url, string page)
        {
            data.Add(url, page);
            dirty = true;
        }

        private string GetFilename(string url)
        {
            string result = url;
            foreach (char c in Path.GetInvalidFileNameChars())
                result = result.Replace(c, '_');
            return result + ".cache";
        }
    }
}