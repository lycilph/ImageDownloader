using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using NLog;

namespace Core
{
    public class SiteCrawler : DisposableObject
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private bool disposed;
        private readonly List<string> page_extensions = new List<string> {".html", ".htm"};
        private readonly IPageProvider page_provider;
        private readonly List<string> visited = new List<string>();
        private readonly Stack<string> unvisited = new Stack<string>();

        public SiteCrawler(IPageProvider page_provider)
        {
            this.page_provider = page_provider;
        }

        public Site Crawl(string url)
        {
            var site = new Site(url);
            unvisited.Push(url);

            log.Trace("Starting crawl of: " + url);

            while (unvisited.Count > 0)
            {
                var current_url = unvisited.Pop();
                if (visited.Contains(current_url)) 
                    continue;

                log.Trace("Url: " + UrlHelper.Filename(current_url) + " (Pages left: " + unvisited.Count + ")");

                try
                {
                    visited.Add(current_url);
                    CrawlPage(site, current_url);
                }
                catch (Exception e)
                {
                    site.Errors.Add(e.Message);
                    log.Error(Environment.NewLine + "Error " + e.Message);
                }
            }

            visited.Clear();
            unvisited.Clear();

            return site;
        }

        private void CrawlPage(Site site, string page_url)
        {
            var page_data = page_provider.Get(page_url);
            if (string.IsNullOrWhiteSpace(page_data))
                return;

            var doc = new HtmlDocument();
            doc.LoadHtml(page_data);

            // Get all links
            var link_nodes = doc.DocumentNode.SelectNodes("//a[@href]");
            var links = (link_nodes == null ?
                         new List<string>() :
                         new List<string>(link_nodes.Select(n => n.Attributes["href"].Value)));
            var full_links = links.Select(l => UrlHelper.FullLink(page_url, l))
                                  .Distinct()
                                  .ToList();
            var frame_nodes = doc.DocumentNode.SelectNodes("//frame[@src]");
            var frames = (frame_nodes == null ?
                         new List<string>() :
                         new List<string>(frame_nodes.Select(n => n.Attributes["src"].Value)));
            var frames_fulllink = frames.Select(l => UrlHelper.FullLink(page_url, l)).ToList();

            full_links.AddRange(frames_fulllink);

            var internal_links = full_links.Where(l => site.Host == UrlHelper.Host(l)).ToList();
            var internal_page_links = internal_links.Where(l => page_extensions.Contains(UrlHelper.Extension(l)) || string.IsNullOrWhiteSpace(UrlHelper.Extension(l))).ToList();
            var internal_other_links = internal_links.Except(internal_page_links).ToList();

            // Get all images
            var image_nodes = doc.DocumentNode.SelectNodes("//img");
            var images = (image_nodes == null ?
                         new List<string>() :
                         new List<string>(image_nodes.Select(n => n.Attributes["src"].Value)));
            var images_fulllink = images.Select(l => UrlHelper.FullLink(page_url, l)).Distinct().ToList();

            var page = site.GetOrCreatePage(page_url);
            page.PageLinks = internal_page_links;
            page.OtherLinks = internal_other_links;
            page.Images = images_fulllink;

            // Update external links
            var external_links = full_links.Except(internal_links);
            site.ExternalLinks.AddRange(external_links.Except(site.ExternalLinks));

            // Add new pages to the unvisited list
            internal_page_links.Except(visited).Except(unvisited).Apply(l => unvisited.Push(l));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            try
            {
                if (disposing)
                {
                    // Free any other managed objects here.
                    page_provider.Dispose();
                }

                // Free any unmanaged objects here. 
            }
            finally
            {
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
