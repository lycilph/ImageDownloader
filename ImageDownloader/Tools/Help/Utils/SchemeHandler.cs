using CefSharp;
using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace ImageDownloader.Tools.Help.Utils
{
    public class SchemeHandler : ISchemeHandler
    {
        public bool ProcessRequestAsync(IRequest request, SchemeHandlerResponse response, OnRequestCompletedHandler request_completed_callback)
        {
            var uri = new Uri(request.Url);
            var host = uri.Host;

            var name = Assembly.GetExecutingAssembly().GetManifestResourceNames().FirstOrDefault(n => n.ToLower().Contains(host));
            if (!string.IsNullOrWhiteSpace(name))
            {
                response.ResponseStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
                response.MimeType = "text/html";
                request_completed_callback();

                return true;
            }
            
            return false;
        }
    }
}
