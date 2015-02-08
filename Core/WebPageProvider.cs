using System.Net;

namespace Core
{
    public class WebPageProvider : DisposableObject, IPageProvider
    {
        private bool disposed;
        private int pages_downloaded;
        private readonly WebClient web_client = new WebClient();

        public string Get(string url)
        {
            pages_downloaded++;
            return web_client.DownloadString(url);
        }

        public string Status()
        {
            return string.Format("Pages downloaded: {0}", pages_downloaded);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            try
            {
                if (disposing)
                {
                    // Free any other managed objects here.
                    web_client.Dispose();
                }

                // Free any unmanaged objects here.
            }
            finally
            {
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
