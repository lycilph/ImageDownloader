using ImageDownloader.Models;
using ImageDownloader.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ImageDownloader.Interfaces
{
    public interface IScraper
    {
        IProgress<ScraperInfo> Progress { get; set; }
        ScraperResult FindAllPages(Project project, CancellationToken token);
        ScraperResult FindAllImages(IEnumerable<string> urls, CancellationToken token);
    }
}
