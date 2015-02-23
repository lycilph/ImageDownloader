using WebCrawler.Data;

namespace WebCrawler.PageProvider
{
    public class PageProviderFactory : IPageProviderFactory
    {
        private readonly bool use_cache;
        private readonly Cache cache;
        private readonly string user_agent;
        private readonly int request_timeout;

        public PageProviderFactory(bool use_cache, Cache cache, string user_agent, int request_timeout)
        {
            this.use_cache = use_cache;
            this.cache = cache;
            this.user_agent = user_agent;
            this.request_timeout = request_timeout;
        }

        public IPageProvider Create()
        {
            return (use_cache
                ? (IPageProvider) new CachedPageProvider(cache, user_agent, request_timeout)
                : (IPageProvider) new WebPageProvider(user_agent, request_timeout));
        }
    }
}
