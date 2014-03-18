using CefSharp;

namespace ImageDownloader.Tools.Help.Utils
{
    public class SchemeHandlerFactory : ISchemeHandlerFactory
    {
        public ISchemeHandler Create()
        {
            return new SchemeHandler();
        }
    }
}
