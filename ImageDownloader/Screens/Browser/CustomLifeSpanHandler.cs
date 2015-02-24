using CefSharp;
using NLog;

namespace ImageDownloader.Screens.Browser
{
    public class CustomLifeSpanHandler : ILifeSpanHandler
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public bool OnBeforePopup(IWebBrowser browser, string url, ref int x, ref int y, ref int width, ref int height)
        {
            logger.Trace("OnBeforePopup - " + url);

            browser.Load(url);
            return true;
        }

        public void OnBeforeClose(IWebBrowser browser) { }
    }
}
