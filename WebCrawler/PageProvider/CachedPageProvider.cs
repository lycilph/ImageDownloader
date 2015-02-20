using Panda.Utilities;
using WebCrawler.Data;

namespace WebCrawler.PageProvider
{
    public class CachedPageProvider : DisposableObject, IPageProvider
    {
        private readonly IPageProvider page_provider;
        private readonly Cache cache;
        private bool disposed;
        private int hits;
        private int misses;
        
        public CachedPageProvider(Cache cache, string user_agent, int timeout)
        {
            this.cache = cache;
            page_provider = new WebPageProvider(user_agent, timeout);
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
                    page_provider.Dispose();
                }
                // Free any unmanaged objects here. 
            }
            finally
            {
                disposed = true;
                base.Dispose(disposing);
            }
        }

        public Page Get(string url)
        {
            // Check if the url is in the cache
            Page page;
            if (cache.TryGetValue(url, out page))
            {
                hits++;
                return page;
            }

            // Otherwise pass on to the internal page provider and add to cache
            misses++;
            page = page_provider.Get(url);
            cache.TryAdd(url, page);
            return page;
        }

        public string Status()
        {
            return string.Format("Cache: hits {0}, misses {1}, {2}", hits, misses, page_provider.Status());
        }
    }
}
