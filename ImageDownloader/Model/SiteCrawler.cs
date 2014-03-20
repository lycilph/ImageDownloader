using Caliburn.Micro;
using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ImageDownloader.Model
{
    public class SiteCrawler : SiteBase
    {
        private BlockingCollection<string> output;

        private Stack<string> pages = new Stack<string>();
        private List<string> accepted = new List<string>();
        private List<string> rejected = new List<string>();

        public SiteCrawler(ICache cache, IProgress<string> progress) : base(cache, progress) {}

        public void FindAllPages(JobModel job, BlockingCollection<string> output)
        {
            this.output = output;

            Reset();

            pages.Push(job.Website);
            while (pages.Any())
            {
                var page = pages.Pop();
                FindPage(page);

                System.Threading.Thread.Sleep(500);
            }

            output.CompleteAdding();
        }

        private void FindPage(string url)
        {
            if (IsProcessed(url)) return;
                        
            // Load page
            string page = cache.Get(url);
            Accept(url);

            // Extract links
            var all_links = ExtractAllLinks(page);

            // Process potential links
            var potential_links = all_links.Where(l => l.EndsWith("html") || l.EndsWith("htm")).Distinct();
            potential_links.Select(link => FixLink(url, link))
                           .Where(link => !pages.Contains(link) && !IsProcessed(link))
                           .Apply(link => pages.Push(link));

            // Process the remaining links
            all_links.Except(potential_links)
                     .Distinct()
                     .Where(l => !rejected.Contains(l))
                     .Apply(l => rejected.Add(l));
        }

        private void Reset()
        {
            pages.Clear();
            accepted.Clear();
            rejected.Clear();
        }

        private void Accept(string url)
        {
            accepted.Add(url);
            output.Add(url);
            progress.Report(url);
        }

        private bool IsProcessed(string url)
        {
            return accepted.Contains(url) || rejected.Contains(url);
        }

        private IEnumerable<string> ExtractAllLinks(string page)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
            if (nodes == null)
                return new List<string>();

            return new List<string>(nodes.Select(n => n.Attributes["href"].Value));
        }
    }
}
