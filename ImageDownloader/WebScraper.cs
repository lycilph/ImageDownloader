using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ImageDownloader
{
    public class WebScraper
    {
        private Loader loader;
        private List<string> visited_pages = new List<string>();
        private List<string> rejected_pages = new List<string>();
        private List<string> errored_pages = new List<string>();
        private List<string> images = new List<string>();
        private Stack<string> pages = new Stack<string>();
        private int revisits;

        public WebScraper(Loader loader)
        {
            this.loader = loader;
        }

        public List<string> ParseRecursively(string url)
        {
            pages.Push(url);

            var uri = new Uri(url);
            var domain = uri.Host;

            revisits = 0;

            var sw = new Stopwatch();
            sw.Start();
            while (pages.Any())
            {
                var page = pages.Pop();
                Parse(domain, page);
            }
            sw.Stop();
            Console.WriteLine("Done in {0} (revisits {1}))", sw.Elapsed, revisits);

            return images;
        }

        public void Parse(string domain, string url)
        {
            Console.WriteLine("Parsing " + url);

            if (visited_pages.Contains(url))
            {
                Console.WriteLine("Page {0} has already been visited", url);
                revisits++;
                return;
            }

            // Load page
            string page = string.Empty;
            try
            {
                page = loader.Get(url);
            }
            catch (Exception)
            {
                //Console.WriteLine("Exception " + e.Message);
                errored_pages.Add(url);
            }

            // Extract images
            var all_images = GetAllImages(page, ValidImage);
            foreach (var i in all_images)
            {
                var img = FixLink(url, i);

                if (!images.Contains(img))
                    images.Add(img);
            }

            // Extract links
            var all_links = GetAllLinks(page);
            var accepted = all_links.Where(l => l.EndsWith("html") || l.EndsWith("htm"));
            var rejected = all_links.Except(accepted);

            //Console.WriteLine("Accepted");
            foreach (var l in accepted)
            {
                var link = FixLink(url, l);

                if (!pages.Contains(link) && !visited_pages.Contains(link) && IsInDomain(domain, link))
                {
                    //Console.WriteLine("Found new link " + link);
                    pages.Push(link);
                }
                //else
                //    Console.WriteLine("Link already found ({0})", link);
            }
            //Console.WriteLine("Rejected");
            foreach (var l in rejected)
            {
                if (!rejected_pages.Contains(l))
                {
                    //Console.WriteLine("Rejected page " + l);
                    rejected_pages.Add(l);
                }
                //else
                //    Console.WriteLine("Link already rejected ({0})", l);
            }

            visited_pages.Add(url);
        }

        private bool ValidImage(HtmlNode node)
        {
            var width = Int32.Parse(node.Attributes["width"].Value);
            var height = Int32.Parse(node.Attributes["height"].Value);

            return (width > 100) && (height > 100);
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

        private bool IsInDomain(string domain, string url)
        {
            var uri = new Uri(url);
            return domain == uri.Host;
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
