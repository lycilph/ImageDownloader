using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper
{
    public class Scraper
    {
        private PageLoader loader;
        private Settings settings;
        private IProgress<ProgressInfo> progress;

        private string domain = string.Empty;

        private Stack<string> pages = new Stack<string>();
        private List<string> accepted = new List<string>();
        private List<string> rejected = new List<string>();

        public Scraper(PageLoader loader, Settings settings, IProgress<ProgressInfo> progress)
        {
            this.loader = loader;
            this.settings = settings;
            this.progress = progress;
        }

        public void FindAllImages(IEnumerable<string> urls)
        {
            Reset();

            foreach (var url in urls)
            {
                domain = GetDomain(url);
                FindImages(url);
            }
        }

        private void FindImages(string url)
        {
            // Load page
            string page = string.Empty;
            try
            {
                page = loader.Get(url);
            }
            catch (Exception)
            {
                if (progress != null)
                    progress.Report(ProgressInfo.CreateRejectedInfo(url));
                return;
            }

            // Extract images
            var all_images = GetAllImages(page, IsValidImage);
            foreach (var i in all_images)
            {
                var img = FixLink(url, i);

                if (!accepted.Contains(img) && IsInDomain(img))
                {
                    if (progress != null)
                        progress.Report(ProgressInfo.CreateAcceptedInfo(img));

                    accepted.Add(img);
                }
            }
        }

        public void DownloadAllPages(string url)
        {
            Reset();
            domain = GetDomain(url);

            pages.Push(url);
            while (pages.Any())
            {
                var page = pages.Pop();
                DownloadPage(page);
            }
        }

        private void Reset()
        {
            pages.Clear();
            accepted.Clear();
            rejected.Clear();
        }

        private void DownloadPage(string url)
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

        private IEnumerable<string> GetAllImages(string page, Predicate<HtmlNode> accept)
        {
            var images = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            var nodes = doc.DocumentNode.SelectNodes("//img[@width and @height]");
            if (nodes == null)
                return images;

            foreach (var node in nodes)
                if (accept(node))
                    images.Add(node.Attributes["src"].Value);

            return images;
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

        private bool IsValidImage(HtmlNode node)
        {
            var width = Int32.Parse(node.Attributes["width"].Value);
            var height = Int32.Parse(node.Attributes["height"].Value);

            return ((settings.MinWidth > 0 && width > settings.MinWidth) || (settings.MinWidth <= 0)) &&
                   ((settings.MaxWidth > 0 && width < settings.MaxWidth) || (settings.MaxWidth <= 0)) &&
                   ((settings.MinHeight > 0 && height > settings.MinHeight) || (settings.MinHeight <= 0)) &&
                   ((settings.MaxHeight > 0 && height < settings.MaxHeight) || (settings.MaxHeight <= 0));
        }

        private bool IsProcessed(string url)
        {
            return accepted.Contains(url) || rejected.Contains(url);
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

        private string FixLink(string url, string link)
        {
            var base_uri = new Uri(url);
            var uri = new Uri(base_uri, link);
            var result = uri.ToString();
            return result;
        }

    }
}
