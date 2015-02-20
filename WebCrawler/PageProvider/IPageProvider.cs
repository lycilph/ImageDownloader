using System;
using WebCrawler.Data;

namespace WebCrawler.PageProvider
{
    public interface IPageProvider : IDisposable
    {
        Page Get(string url);
        string Status();
    }
}