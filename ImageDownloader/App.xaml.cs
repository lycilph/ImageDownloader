using System.IO;
using System.Reflection;
using System.Windows;
using CefSharp;

namespace ImageDownloader
{
    public partial class App
    {
        public App()
        {
            var settings = new CefSettings
            {
                CachePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };

            Cef.Initialize(settings);
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}
