using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ImageDownloader.Model
{
    public class SiteAnalyzer : SiteBase
    {
        private BlockingCollection<string> output;

        private List<string> accepted = new List<string>();

        public SiteAnalyzer(ICache cache, IProgress<string> progress) : base(cache, progress) { }

        public void FindAllImages(IEnumerable<string> urls, BlockingCollection<string> output)
        {
            this.output = output;

            Reset();

            foreach (var url in urls)
            {
                FindImages(url);

                // Check for cancellation

                System.Threading.Thread.Sleep(1000);
            }

            output.CompleteAdding();
        }

        private void FindImages(string url)
        {
            // Load page
            string page = cache.Get(url);

            // Extract images
            ExtractAllImages(page).Select(i => FixLink(url, i))
                                  .Where(i => !accepted.Contains(i))
                                  .Apply(i => Accept(i));
        }

        private void Reset()
        {
            accepted.Clear();
        }

        private void Accept(string url)
        {
            accepted.Add(url);
            output.Add(url);
            progress.Report(url);
        }

        private IEnumerable<string> ExtractAllImages(string page)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            var nodes = doc.DocumentNode.SelectNodes("//img[@src]");
            if (nodes == null)
                return new List<string>();

            return new List<string>(nodes.Select(n => n.Attributes["src"].Value));
        }
    }
}
