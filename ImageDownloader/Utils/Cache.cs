using Caliburn.Micro;
using ImageDownloader.Interfaces;
using ImageDownloader.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<string, string> data = new ConcurrentDictionary<string, string>();

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

            var filename = GetCacheFilename(base_url);
            var filepath = Path.Combine(settings.CacheFolder, filename);

            if (!File.Exists(filepath))
                return;

            using (var archive = ZipFile.OpenRead(filepath))
            {
                var entry = archive.GetEntry("dictionary.cache");
                using (var sw = new StreamReader(entry.Open()))
                {
                    var json = sw.ReadToEnd();
                    data = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(json);
                }
            }
        }

        public void Update()
        {
            if (!(settings.CachingEnabled && dirty))
                return;

            var filename = GetCacheFilename(base_url);
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

        public string GetImage(string url)
        {
            if (!string.IsNullOrWhiteSpace(settings.OutputFolder) && !Directory.Exists(settings.OutputFolder))
                Directory.CreateDirectory(settings.OutputFolder);

            var uri = new Uri(url);
            var uri_path = uri.AbsolutePath.Trim(new char[] { '/' });
            var filename = GetFilename(uri_path);
            var cache_filename = Path.Combine(settings.OutputFolder, filename);

            var cached_file = Directory.EnumerateFiles(settings.OutputFolder).FirstOrDefault(f => f == cache_filename);
            if (!string.IsNullOrWhiteSpace(cached_file))
            {
                log.Info("Found cached image for {0}", url);
                return cached_file;
            }

            // Check if we have an url to download the image from
            if (!string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        log.Info("Downloading image for {0}", url);
                        client.DownloadFile(url, cache_filename);
                    }
                    return cache_filename;
                }
                catch (WebException e)
                {
                    log.Error(e);
                }
            }

            return string.Empty;
        }

        public string Get(string url)
        {
            var page = string.Empty;

            // See if the cache contains the key
            if (data.TryGetValue(url, out page))
                return page;

            // Otherwise add it
            try
            {
                using (var client = new WebClient())
                {
                    page = client.DownloadString(url);
                    log.Info("Downloading " + url);
                }
            }
            catch (WebException we)
            {
                log.Warn("Exception for \"{0}\" - {1}", url, we.Message);
            }
            AddCacheEntry(url, page);

            return page;
        }

        private void AddCacheEntry(string url, string page)
        {
            if (!data.TryAdd(url, page))
                throw new InvalidOperationException("Couldn't add key to dictionary");
            dirty = true;
        }

        private string GetCacheFilename(string url)
        {
            return GetFilename(url) + ".cache";
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