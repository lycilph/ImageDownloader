using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader
{
    public class Loader
    {
        private string cache_dir;

        public Loader(string dir)
        {
            cache_dir = dir;
            if (!Directory.Exists(cache_dir))
                Directory.CreateDirectory(cache_dir);
        }

        public string Get(string url)
        {
            var filename = GetFilename(url);
            var cached_path = Path.Combine(cache_dir, filename);

            if (File.Exists(cached_path))
            {
                //Console.WriteLine("Found cached version of {0} at {1}", url, cached_path);

                return File.ReadAllText(cached_path);
            }

            throw new InvalidDataException();

            string result = string.Empty;
            using (var client = new WebClient())
            {
                //Console.WriteLine("Downloading and caching {0} as {1}", url, cached_path);

                result = client.DownloadString(url);
                File.WriteAllText(cached_path, result);
            }
            return result;
        }

        private string GetFilename(string url)
        {
            string result = url;
            foreach (char c in Path.GetInvalidFileNameChars())
                result = result.Replace(c, '_');
            return result;
        }
    }
}
