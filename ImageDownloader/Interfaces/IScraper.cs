using ImageDownloader.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ImageDownloader.Interfaces
{
    public interface IScraper
    {
        ScraperResult FindAllPages(string url, IProgress<ScraperInfo> progress, CancellationToken token);
        ScraperResult FindAllImages(IEnumerable<string> urls, IProgress<ScraperInfo> progress, CancellationToken token);
    }
}
