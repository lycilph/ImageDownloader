using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageDownloader.Model
{
    public class SiteLoader : SiteBase
    {
        public SiteLoader(ICache cache, IProgress<string> progress) : base(cache, progress) { }

        public void LoadAllImages(IEnumerable<string> urls)
        {
            var dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Images");

            foreach (var url in urls)
            {
                using (var client = new WebClient())
                {
                    var data = client.DownloadData(url);
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(data);
                    image.EndInit();

                    var fullpath = Path.Combine(dir, GetFilename(url));
                    Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

                    using (var stream = new FileStream(fullpath, FileMode.Create))
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    progress.Report(string.Format("{0} ({1}, {2}) - {3}", url, image.PixelWidth, image.PixelHeight, fullpath));
                }
            }
        }

        private string GetFilename(string url)
        {
            var uri = new Uri(url);
            var segments = uri.Segments.Select(s => s.Trim(Path.GetInvalidFileNameChars()))
                                       .Where(s => !string.IsNullOrWhiteSpace(s))
                                       .ToArray();
            return Path.Combine(segments);
        }
    }
}
