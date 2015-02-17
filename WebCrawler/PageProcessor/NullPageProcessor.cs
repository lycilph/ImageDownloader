using WebCrawler.Data;

namespace WebCrawler.PageProcessor
{
    public class NullPageProcessor : IPageProcessor
    {
        public void Process(Page page) { }
    }
}