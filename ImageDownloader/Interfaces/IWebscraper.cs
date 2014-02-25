using ImageDownloader.Models;
using ImageDownloader.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ImageDownloader.Interfaces
{
    public interface IWebscraper
    {
        Result FindAllPages(Project project, IProgress<Info> progress, CancellationToken token);
        Result FindAllPages(Project project, IProgress<Info> progress, CancellationToken token, BlockingCollection<string> output);
        Result FindAllImages(Project project, IEnumerable<string> urls, IProgress<Info> progress, CancellationToken token);
    }
}
