using ImageDownloader.Interfaces;
using ImageDownloader.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;

namespace ImageDownloader.Utils
{
    [Export(typeof(IScraper))]
    public class Webscraper : IScraper
    {
        private ICache cache;
        private Settings settings;

        private string domain = string.Empty;

        private Stack<string> pages = new Stack<string>();
        private List<string> accepted = new List<string>();
        private List<string> rejected = new List<string>();

        [ImportingConstructor]
        public Webscraper(ICache cache, Settings settings)
        {
            this.cache = cache;
            this.settings = settings;
        }

        public ScraperResult FindAllPages(string url, IProgress<ScraperInfo> progress, CancellationToken token)
        {
            Reset();
            domain = GetDomain(url);

            pages.Push(url);
            while (pages.Any())
            {
                var page = pages.Pop();
                FindPage(page);
            }

            //for (int i = 0; i < 50; i++)
            //{
            //    Thread.Sleep(100);

            //    if (token.IsCancellationRequested)
            //        break;

            //    var info = new ScraperInfo(string.Format("Page {0}", i+1), ScraperInfo.StateType.Accepted);
            //    progress.Report(info);
            //}

            return new ScraperResult();
        }

        private void FindPage(string url)
        {
            if (IsProcessed(url)) return;

            // Load page
            string page = string.Empty;
            try
            {
                page = loader.Get(url);
                accepted.Add(url);

                if (progress != null)
                    progress.Report(ProgressInfo.CreateAcceptedInfo(url));
            }
            catch (Exception)
            {
                rejected.Add(url + " (exception)");

                if (progress != null)
                    progress.Report(ProgressInfo.CreateRejectedInfo(url));
                return;
            }

            // Extract links
            var all_links = GetAllLinks(page);
            var accepted_pages = all_links.Where(l => l.EndsWith("html") || l.EndsWith("htm"));
            var rejected_pages = all_links.Except(accepted_pages);

            foreach (var l in accepted_pages)
            {
                var link = FixLink(url, l);

                if (!pages.Contains(link) && !IsProcessed(link) && IsInDomain(link))
                    pages.Push(link);
            }

            foreach (var l in rejected_pages)
            {
                if (!rejected.Contains(l))
                {
                    rejected.Add(l);

                    if (progress != null)
                        progress.Report(ProgressInfo.CreateRejectedInfo(l));
                }
            }
        }

        private void Reset()
        {
            pages.Clear();
            accepted.Clear();
            rejected.Clear();
        }

        private string GetDomain(string url)
        {
            var uri = new Uri(url);
            return uri.Host;
        }

        public ScraperResult FindAllImages(IEnumerable<string> urls, IProgress<ScraperInfo> progress, CancellationToken token)
        {
            return new ScraperResult();
        }
    }
}
