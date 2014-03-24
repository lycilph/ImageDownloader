using Caliburn.Micro;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Media.Imaging;
using ImageDownloader.Core.Utils;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageDownloader.Model
{
    [Export(typeof(ICache))]
    public class Cache : ICache
    {
        private static ILog log = LogManager.GetLog(typeof(Cache));

        // Persistent data
        private ConcurrentDictionary<string, string> page_data = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, bool> image_state = new ConcurrentDictionary<string, bool>();

        // Working data
        private ConcurrentDictionary<string, byte[]> image_data = new ConcurrentDictionary<string, byte[]>();

        private string image_folder;
        private string cache_folder;
        private string cache_fullpath;
        private bool dirty;

        public Cache()
        {
            image_folder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Images");
            cache_folder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Cache");
        }

        public void Initialize(string host)
        {
            var fullpath = Path.Combine(cache_folder, host.GetSafeFilename() + ".cache");

            if (fullpath == cache_fullpath)
                return;

            Reset();
            cache_fullpath = fullpath;
            dirty = false;

            if (!File.Exists(cache_fullpath))
                return;

            using (var archive = ZipFile.OpenRead(cache_fullpath))
            {
                var entry = archive.GetEntry("dictionary.cache");
                using (var sw = new StreamReader(entry.Open()))
                {
                    var json = sw.ReadToEnd();
                    var data = new { PageData = page_data, ImageState = image_state };
                    var result = JsonConvert.DeserializeAnonymousType(json, data);
                    page_data = result.PageData;
                    image_state = result.ImageState;
                }
            }
        }

        public void Update()
        {
            if (!dirty)
                return;

            if (!string.IsNullOrWhiteSpace(cache_folder) && !Directory.Exists(cache_folder))
                Directory.CreateDirectory(cache_folder);

            using (var fs = File.Open(cache_fullpath, FileMode.Create))
            using (var archive = new ZipArchive(fs, ZipArchiveMode.Update))
            {
                var entry = archive.CreateEntry("dictionary.cache");

                using (var sw = new StreamWriter(entry.Open()))
                {
                    var data = new { PageData = page_data, ImageState = image_state };
                    var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    sw.Write(json);
                }
            }
        }

        private void Reset()
        {
            page_data.Clear();
            image_state.Clear();
            image_data.Clear();
        }

        public string Get(string url)
        {
            var page = string.Empty;

            // See if the cache contains the key
            if (page_data.TryGetValue(url, out page))
            {
                log.Info("Cache hit for: {0}", url);
                return page;
            }
            else
            {
                log.Info("Cache miss for: {0}", url);
            }

            // Otherwise add it
            try
            {
                using (var client = new WebClient())
                {
                    log.Info("Downloading " + url);
                    page = client.DownloadString(url);
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
            if (!page_data.TryAdd(url, page))
                throw new InvalidOperationException("Couldn't add key to dictionary");
            dirty = true;
        }

        // Returns if the image was found in the cache or not
        public bool TryGetImage(string url, out BitmapImage image)
        {
            // See if the cache contains the key
            bool state;
            if (image_state.TryGetValue(url, out state))
            {
                // Was the image saved
                if (state)
                {
                    // Check if the file actually exists
                    var fullpath = Path.Combine(image_folder, url.GetFilename());
                    if (File.Exists(fullpath))
                    {
                        log.Info("Cache hit for: {0}", url);
                        image = new BitmapImage(new Uri(url));
                        return true;
                    }
                    else
                    {
                        // Otherwise download it
                        log.Info("Cache hit for: {0}, but file not found", url);
                        image_state.TryRemove(url, out state);
                        image = DownloadImage(url);
                        return false;
                    }
                }
                else // or was it discard
                {
                    log.Info("Cache hit for: {0}, but image was discarded", url);
                    image = null;
                    return true;
                }
            }
            else
            {
                // Otherwise add it
                log.Info("Cache miss for: {0}", url);
                image = DownloadImage(url);
                return false;
            }
        }

        public void SaveImage(string url)
        {
            var fullpath = Path.Combine(image_folder, url.GetFilename());
            Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

            byte[] data = UpdateImage(url, true);

            using (var stream = new FileStream(fullpath, FileMode.Create))
            {
                stream.Write(data, 0, data.Length);
            }
            dirty = true;
        }

        public void DiscardImage(string url)
        {
            UpdateImage(url, false);
            dirty = true;
        }

        private BitmapImage DownloadImage(string url)
        {
            var image = new BitmapImage();
            using (var client = new WebClient())
            {
                log.Info("Downloading " + url);
                var data = client.DownloadData(url);
                image_data.TryAdd(url, data);

                image.BeginInit();
                image.StreamSource = new MemoryStream(data);
                image.EndInit();
            }
            return image;
        }

        private byte[] UpdateImage(string url, bool state)
        {
            byte[] data;
            image_data.TryRemove(url, out data);
            image_state.TryAdd(url, state);
            return data;
        }
    }
}
