using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Core.Utils
{
    public static class StringExtensions
    {
        public static string GetHostName(this string url)
        {
            var uri = new Uri(url);
            switch (uri.Scheme)
            {
                case "http":
                case "https": return uri.Host;
                case "file": return Path.GetDirectoryName(uri.AbsolutePath).GetSafeFilename();
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string GetFilename(this string url)
        {
            var uri = new Uri(url);
            var segments = uri.Segments.Select(s => s.Trim(Path.GetInvalidFileNameChars()))
                                       .Where(s => !string.IsNullOrWhiteSpace(s))
                                       .ToArray();
            return Path.Combine(segments);
        }

        public static string GetSafeFilename(this string str)
        {
            string result = str;
            foreach (char c in Path.GetInvalidFileNameChars())
                result = result.Replace(c, '_');
            return result;
        }
    }
}
