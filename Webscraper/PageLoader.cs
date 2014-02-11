using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper
{
    public class PageLoader
    {
        private Dictionary<string, string> cache = new Dictionary<string, string>();
        private Settings settings;

        public PageLoader(Settings settings)
        {
            this.settings = settings;
        }

        public void CheckCacheDir()
        {
            if (settings.UseCache && !string.IsNullOrWhiteSpace(settings.CacheDir) && !Directory.Exists(settings.CacheDir))
                Directory.CreateDirectory(settings.CacheDir);
        }

        public string Get(string url)
        {
            var filename = GetFilename(url);
            var cache_filename = Path.Combine(settings.CacheDir, filename);
            var result = string.Empty;

            if (cache.ContainsKey(filename))
                return cache[filename];

            if (settings.UseCache && !string.IsNullOrWhiteSpace(settings.CacheDir) && File.Exists(cache_filename))
            {
                result = File.ReadAllText(cache_filename);
                cache.Add(filename, result);
                return result;
            }

            throw new InvalidDataException();

            using (var client = new WebClient())
            {
                result = client.DownloadString(url);
                cache.Add(filename, result);
            }

            if (settings.UseCache && !string.IsNullOrWhiteSpace(settings.CacheDir))
            {
                File.WriteAllText(cache_filename, result);
            }

            return result;
        }

        private string GetFilename(string url)
        {
            string result = url;
            foreach (char c in Path.GetInvalidFileNameChars())
                result = result.Replace(c, '_');
            return result;
        }
    }
}
