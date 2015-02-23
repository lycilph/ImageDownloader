using System.Collections.Concurrent;
using WebCrawler.Data;

namespace WebCrawler.PageProcessor
{
    public class PageCollector : IPageProcessor
    {
        private readonly ConcurrentQueue<Page> pages;

        public PageCollector(ConcurrentQueue<Page> pages)
        {
            this.pages = pages;
        }

        public void Process(Page page)
        {
            pages.Enqueue(page);
        }
    }
}
