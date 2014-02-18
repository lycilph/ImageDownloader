using ImageDownloader.Interfaces;
using ImageDownloader.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;

namespace ImageDownloader.Utils
{
    [Export(typeof(ICache))]
    public class Cache : ICache
    {
        private Settings settings;
        private string base_url;
        private Dictionary<string, string> data = new Dictionary<string, string>();

        [ImportingConstructor]
        public Cache(Settings settings)
        {
            this.settings = settings;
        }

        public void Initialize(string url)
        {
            data.Clear();
            base_url = url;

            if (!settings.CachingEnabled)
                return;

            var filename = GetFilename(base_url);
            var filepath = Path.Combine(settings.CacheFolder, filename);

            if (!File.Exists(filepath))
                return;

            using (var fs = File.Open(filepath, FileMode.Open))
            using (var sw = new StreamReader(fs))
            {
                var json = sw.ReadToEnd();
                data = JsonConvert.DeserializeObject<Dictionary<string,string>>(json);
            }
        }

        public void Update()
        {
            if (!settings.CachingEnabled)
                return;

            var filename = GetFilename(base_url);
            var filepath = Path.Combine(settings.CacheFolder, filename);

            if (!string.IsNullOrWhiteSpace(settings.CacheFolder) && !Directory.Exists(settings.CacheFolder))
                Directory.CreateDirectory(settings.CacheFolder);

            using (var fs = File.Open(filepath, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                sw.Write(json);
            }
        }

        public string Get(string url)
        {
            if (data.ContainsKey(url))
                return data[url];

            var page = string.Empty;
            using (var client = new WebClient())
            {
                page = client.DownloadString(url);
                data.Add(url, page);
            }

            return page;
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