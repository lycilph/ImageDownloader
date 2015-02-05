using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Core
{
    [DebuggerDisplay("Uri = {uri}, Pages = {Pages.Keys.Count}, External Links = {ExternalLinks.Count}, Errors = {Errors.Count}")]
    public class Site
    {
        private Uri uri;

        private string url;
        public string Url
        {
            get { return url; }
            set
            {
                url = value;
                uri = new Uri(url);
            }
        }

        public Dictionary<string, Page> Pages { get; set; }
        public List<string> ExternalLinks { get; set; }
        public List<string> Errors { get; set; }

        public string Host { get { return uri.Host; } }
        public Page MainPage { get { return Pages[Url]; } }

        public Site(string url)
        {
            Url = url;
            Pages = new Dictionary<string, Page>();
            ExternalLinks = new List<string>();
            Errors = new List<string>();
        }

        public Page GetOrCreatePage(string page_url)
        {
            if (Pages.ContainsKey(page_url))
                return Pages[page_url];

            var page = new Page(page_url);
            Pages.Add(page_url, page);
            return page;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Site url: " + Url);
            Pages.Values.Apply(p => sb.AppendLine(p.ToString()));
            ExternalLinks.Apply(l => sb.AppendLine("External link: " + l));
            Errors.Apply(e => sb.AppendLine("Errors: " + e));
            return sb.ToString();
        }
    }
}
