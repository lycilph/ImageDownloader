using CefSharp;

namespace ImageDownloader
{
    public class CustomLifeSpanHandler : ILifeSpanHandler
    {
        public bool OnBeforePopup(IWebBrowser browser, string url, ref int x, ref int y, ref int width, ref int height)
        {
            browser.Load(url);
            return true;
        }

        public void OnBeforeClose(IWebBrowser browser)
        {
        }
    }
}
