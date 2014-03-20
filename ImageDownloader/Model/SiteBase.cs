using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Model
{
    public class SiteBase
    {
        protected IProgress<string> progress;
        protected ICache cache;

        public SiteBase(ICache cache, IProgress<string> progress)
        {
            this.cache = cache;
            this.progress = progress;
        }
        
        protected string FixLink(string url, string link)
        {
            var base_uri = new Uri(url);
            var uri = new Uri(base_uri, link);
            return uri.ToString();
        }
    }
}
