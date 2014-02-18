using ImageDownloader.Interfaces;
using ImageDownloader.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using HtmlAgilityPack;

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
            cache.Initialize(url);

            pages.Push(url);
            while (pages.Any())
            {
                var page = pages.Pop();
                FindPage(page, progress);

                if (token.IsCancellationRequested)
                    break;
            }

            cache.Update();

            return new ScraperResult();
        }

        private void FindPage(string url, IProgress<ScraperInfo> progress)
        {
            if (IsProcessed(url)) return;

            // Load page
            string page = string.Empty;
            try
            {
                page = cache.Get(url);
                accepted.Add(url);

                if (progress != null)
                    progress.Report(new ScraperInfo(url, ScraperInfo.StateType.Accepted));
            }
            catch (Exception)
            {
                rejected.Add(url + " (exception)");

                if (progress != null)
                    progress.Report(new ScraperInfo(url, ScraperInfo.StateType.Rejected));
                return;
            }

            // Extract links
            //var all_links = GetAllLinks(page);
            //var accepted_pages = all_links.Where(l => l.EndsWith("html") || l.EndsWith("htm"));
            //var rejected_pages = all_links.Except(accepted_pages);

            //foreach (var l in accepted_pages)
            //{
            //    var link = FixLink(url, l);

            //    if (!pages.Contains(link) && !IsProcessed(link) && IsInDomain(link))
            //        pages.Push(link);
            //}

            //foreach (var l in rejected_pages)
            //{
            //    if (!rejected.Contains(l))
            //    {
            //        rejected.Add(l);

            //        if (progress != null)
            //            progress.Report(new ScraperInfo(url, ScraperInfo.StateType.Rejected));
            //    }
            //}
        }

        private void Reset()
        {
            pages.Clear();
            accepted.Clear();
            rejected.Clear();
        }

        private IEnumerable<string> GetAllLinks(string page)
        {
            var links = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
            if (nodes == null)
                return links;

            foreach (var node in nodes)
                links.Add(node.Attributes["href"].Value);

            return links;
        }

        private string GetDomain(string url)
        {
            var uri = new Uri(url);
            return uri.Host;
        }

        private bool IsInDomain(string url)
        {
            return domain == GetDomain(url);
        }

        private bool IsProcessed(string url)
        {
            return accepted.Contains(url) || rejected.Contains(url);
        }

        private string FixLink(string url, string link)
        {
            var base_uri = new Uri(url);
            var uri = new Uri(base_uri, link);
            var result = uri.ToString();
            return result;
        }

        public ScraperResult FindAllImages(IEnumerable<string> urls, IProgress<ScraperInfo> progress, CancellationToken token)
        {
            return new ScraperResult();
        }
    }
}
