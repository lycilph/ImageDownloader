using System.Net;

namespace Core
{
    public class WebPageProvider : DisposableObject, IPageProvider
    {
        private bool disposed;
        private readonly WebClient web_client = new WebClient();

        public string Get(string url)
        {
            return web_client.DownloadString(url);
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
