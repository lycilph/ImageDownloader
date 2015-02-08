using System;
using System.Collections.Generic;
using System.IO;
using NLog;

namespace Core
{
    public class CachedPageProvider : DisposableObject, IPageProvider
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private const int cache_lifetime = 7; // Lifetime in days

        private bool disposed;
        private bool cache_has_changed;
        private Dictionary<string, CacheItem> cache = new Dictionary<string, CacheItem>();
        private readonly bool dispose_page_provider;
        private readonly IPageProvider page_provider;
        private readonly string filename;

        public int CacheHit { get; private set; }
        public int CacheMiss { get; private set; }

        public CachedPageProvider(string filename) : this(filename, new WebPageProvider())
        {
            dispose_page_provider = true;
        }

        public CachedPageProvider(string filename, IPageProvider page_provider)
        {
            this.page_provider = page_provider;
            this.filename = filename;
            Load();

            CacheHit = 0;
            CacheMiss = 0;
        }

        private void Load()
        {
            if (!File.Exists(filename))
                return;

            cache = JsonExtensions.ReadFromFileAndUnzip<Dictionary<string, CacheItem>>(filename);
        }

        private void Save()
        {
            if (!cache_has_changed)
                return;

            JsonExtensions.ZipAndWriteToFile(filename, cache);
        }

        private void Add(string url, string data)
        {
            cache[url] = new CacheItem(data);
            cache_has_changed = true;
        }

        public string Get(string url)
        {
            CacheItem item;
            var has_key = cache.TryGetValue(url, out item);

            if (has_key && item.Timestamp.AddDays(cache_lifetime) > DateTime.Now)
            {
                CacheHit++;
                return item.Data;
            }

            try
            {
                var page_data = page_provider.Get(url);
                Add(url, page_data);
                return page_data;
            }
            catch (Exception e)
            {
                Add(url, string.Empty);
                log.Error("Error when getting " + url, e);
                throw;
            }
            finally
            {
                CacheMiss++;
            }
        }

        public string Status()
        {
            return string.Format("Cache: hits {0}, misses {1}, {2}", CacheHit, CacheMiss, page_provider.Status());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            try
            {
                if (disposing)
                {
                    // Free any other managed objects here.
                    if (dispose_page_provider)
                        page_provider.Dispose();
                    Save();
                }

                // Free any unmanaged objects here. 
            }
            finally
            {
                disposed = true;
                base.Dispose(disposing);                
            }
        }
    }
}
