using System;
using System.IO;
using System.Linq;

namespace Core
{
    public static class UrlHelper
    {
        public static string FullLink(string page_url, string link)
        {
            var page_uri = new Uri(page_url);
            var uri = new Uri(page_uri, link);
            return uri.ToString();
        }

        public static string RemoveLastSegment(string url)
        {
            var uri = new Uri(url);
            var segments = uri.Segments.Take(uri.Segments.Length - 1).ToArray();
            var ub = new UriBuilder(uri) { Path = string.Concat(segments) };
            return ub.Uri.ToString().Trim(new[] { '/' });
        }

        public static string Filename(string url)
        {
            return Path.GetFileName(url);
        }

        public static string Host(string url)
        {
            var uri = new Uri(url);
            return uri.Host;
        }

        public static string Extension(string url)
        {
            return Path.GetExtension(url).ToLowerInvariant();
        }
    }
}
