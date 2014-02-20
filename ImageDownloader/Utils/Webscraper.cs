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

        private Project project;
        private IProgress<ScraperInfo> progress;

        private List<string> include_keywords;
        private List<string> exclude_keywords;

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

        public ScraperResult FindAllPages(Project project, IProgress<ScraperInfo> progress, CancellationToken token)
        {
            this.project = project;
            this.progress = progress;

            include_keywords = project.Keywords.Where(k => k.Type == Keyword.RestrictionType.Include).Select(k => k.Text).ToList();
            exclude_keywords = project.Keywords.Where(k => k.Type == Keyword.RestrictionType.Exclude).Select(k => k.Text).ToList();

            Reset();
            domain = GetDomain(project.Site);
            cache.Initialize(domain);

            pages.Push(project.Site);
            while (pages.Any())
            {
                var page = pages.Pop();
                FindPage(page);

                if (token.IsCancellationRequested)
                    break;
            }

            cache.Update();

            return new ScraperResult();
        }

        private void FindPage(string url)
        {
            if (IsProcessed(url)) return;

            if (Filter(url))
            {
                Reject(url);
                return;
            }

            // Load page
            string page = cache.Get(url);
            Accept(url);

            // Extract links
            var all_links = GetAllLinks(page);
            var accepted_links = all_links.Where(l => l.EndsWith("html") || l.EndsWith("htm"));
            var rejected_links = all_links.Except(accepted_links);

            foreach (var l in accepted_links)
            {
                var link = FixLink(url, l);

                if (!pages.Contains(link) && !IsProcessed(link) && IsInDomain(link))
                    pages.Push(link);
            }

            foreach (var l in rejected_links)
            {
                if (!rejected.Contains(l))
                    Reject(l);
            }
        }

        private void Accept(string url)
        {
            accepted.Add(url);

            if (progress != null)
                progress.Report(new ScraperInfo(url, ScraperInfo.StateType.Accepted));
        }

        private void Reject(string url)
        {
            rejected.Add(url);

            if (progress != null)
                progress.Report(new ScraperInfo(url, ScraperInfo.StateType.Rejected));
        }

        private bool Filter(string url)
        {
            bool result = false;

            // If there are any include keywords, the url MUST match at least 1 of them
            if (include_keywords.Any())
            {
                result = true;
                foreach (var keyword in include_keywords)
                    if (url.Contains(keyword))
                    {
                        result = false;
                        break;
                    }
            }

            // A keyword MUST NOT match any exclude keywords
            foreach (var keyword in exclude_keywords)
                if (url.Contains(keyword))
                {
                    result = true;
                    break;
                }

            return result;
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
