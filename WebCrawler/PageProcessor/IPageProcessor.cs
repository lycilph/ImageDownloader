using WebCrawler.Data;

namespace WebCrawler.PageProcessor
{
    public interface IPageProcessor
    {
        void Process(Page page);
    }
}
