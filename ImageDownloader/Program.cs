using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var exe_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var cache_dir = Path.Combine(exe_dir, "Cache");
            var images_dir = Path.Combine(exe_dir, "Images");
            var images_filename = Path.Combine(exe_dir, "images.txt");

            var loader = new Loader(cache_dir);
            var scraper = new WebScraper(loader);
            //var images = scraper.ParseRecursively(@"http://www.skovboernehave.dk");
            //File.WriteAllLines(images_filename, images);

            var sw = new Stopwatch();
            sw.Start();

            var images = File.ReadAllLines(images_filename);
            object directory_lock = new object();
            int downloads = 0;
            int caches = 0;
            //foreach (var img in images)
            Parallel.ForEach(images.Take(100), new ParallelOptions { MaxDegreeOfParallelism = 4 }, img =>
            {
                var uri = new Uri(img);
                var segments = uri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var filtered_segments = segments.Where(s => s.ToLower() != "slides");
                var img_path = Path.Combine(images_dir, string.Join(@"\", filtered_segments));

                if (File.Exists(img_path))
                {
                    Interlocked.Increment(ref caches);
                    return;
                }

                // Make sure directories exists
                var img_dir = Path.GetDirectoryName(img_path);
                if (!Directory.Exists(img_dir))
                {
                    lock (directory_lock)
                    {
                        Directory.CreateDirectory(img_dir);
                    }
                }

                var client = new WebClient();
                client.DownloadFile(img, img_path);

                Interlocked.Increment(ref downloads);
            });

            sw.Stop();
            Console.WriteLine("Done in {0} ({1} downloads, {2} caches))", sw.Elapsed, downloads, caches);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
