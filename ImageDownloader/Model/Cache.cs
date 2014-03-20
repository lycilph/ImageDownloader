using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Model
{
    public class Cache : ICache
    {
        public string Get(string url)
        {
            var page = string.Empty;
            using (var client = new WebClient())
            {
                page = client.DownloadString(url);
            }
            return page;
        }
    }
}
