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
                BitmapImage image;
                var state = cache.TryGetImage(url, out image);

                if (state)
                {
                    if (image != null)
                        progress.Report(url);
                }
                else
                {
                    if (image.PixelWidth >= 50 && image.PixelHeight >= 50)
                        cache.SaveImage(url);
                    else
                        cache.DiscardImage(url);
                }

                System.Threading.Thread.Sleep(1500);
            }
        }
    }
}
