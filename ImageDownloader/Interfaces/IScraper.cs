using ImageDownloader.Models;
using ImageDownloader.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ImageDownloader.Interfaces
{
    public interface IScraper
    {
        IProgress<Info> Progress { get; set; }
        Result FindAllPages(Project project, CancellationToken token);
        Result FindAllImages(Project project, Result urls, CancellationToken token);
    }
}
