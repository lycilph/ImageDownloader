using System.Collections.Generic;
using WebCrawler.Data;

namespace WebCrawler.LinkExtractor
{
    public interface ILinkExtractor
    {
        List<string> Get(Page page);
    }
}