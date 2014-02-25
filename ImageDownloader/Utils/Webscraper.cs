using HtmlAgilityPack;
using ImageDownloader.Interfaces;
using ImageDownloader.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;

namespace ImageDownloader.Utils
{
    [Export(typeof(IWebscraper))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Webscraper : IWebscraper
    {
        private ICache cache;

        private IProgress<Info> progress;
        private BlockingCollection<string> output;

        private List<string> include_keywords;
        private List<string> exclude_keywords;
        private string domain = string.Empty;

        private Predicate<HtmlNode> is_valid_image;

        private Stack<string> pages = new Stack<string>();
        private List<string> accepted = new List<string>();
        private List<string> rejected = new List<string>();

        [ImportingConstructor]
        public Webscraper(ICache cache)
        {
            this.cache = cache;
        }

        public Result FindAllPages(Project project, IProgress<Info> progress, CancellationToken token)
        {
            return FindAllPages(project, progress, token, null);
        }

        public Result FindAllPages(Project project, IProgress<Info> progress, CancellationToken token, BlockingCollection<string> output)
        {
            this.progress = progress;
            this.output = output;

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

            if (output != null)
                output.CompleteAdding();

            cache.Update();

            return new Result(accepted);
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
            var all_links = ExtractAllLinks(page);
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

        public Result FindAllImages(Project project, IEnumerable<string> urls, IProgress<Info> progress, CancellationToken token)
        {
            this.progress = progress;

            include_keywords = project.Keywords.Where(k => k.Type == Keyword.RestrictionType.Include).Select(k => k.Text).ToList();
            exclude_keywords = project.Keywords.Where(k => k.Type == Keyword.RestrictionType.Exclude).Select(k => k.Text).ToList();

            Reset();
            domain = GetDomain(project.Site);
            cache.Initialize(domain);

            is_valid_image = GetImagePredicate(project);

            foreach (var url in urls)
            {
                FindImages(url);

                if (token.IsCancellationRequested)
                    break;
            }

            return new Result(accepted);
        }

        private void FindImages(string url)
        {
            // Load page
            string page = cache.Get(url);

            // Extract images
            var all_images = ExtractAllImages(page);
            foreach (var i in all_images)
            {
                var img = FixLink(url, i);

                if (!accepted.Contains(img) && IsInDomain(img) && !Filter(img))
                    Accept(img);
            }
        }

        private IEnumerable<string> ExtractAllLinks(string page)
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

        private IEnumerable<string> ExtractAllImages(string page)
        {
            var images = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            var nodes = doc.DocumentNode.SelectNodes("//img[@width and @height and @src]");
            if (nodes == null)
                return images;

            foreach (var node in nodes)
                if (is_valid_image(node))
                    images.Add(node.Attributes["src"].Value);

            return images;
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

        private bool IsInDomain(string url)
        {
            return domain == GetDomain(url);
        }

        private void Accept(string url)
        {
            accepted.Add(url);

            if (output != null)
                output.Add(url);

            if (progress != null)
                progress.Report(new Info(url, Info.StateType.Accepted));
        }

        private void Reject(string url)
        {
            rejected.Add(url);

            if (progress != null)
                progress.Report(new Info(url, Info.StateType.Rejected));
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

        private Predicate<HtmlNode> GetImagePredicate(Project project)
        {
            return node =>
            {
                var extension = Path.GetExtension(node.Attributes["src"].Value).ToLower();
                return project.Extensions.Contains(extension);
            };
        }
    }
}
