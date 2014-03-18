using CefSharp;
using System;

namespace ImageDownloader
{
    public partial class App
    {
        public App()
        {
            Settings settings = new Settings();
            if (!CEF.Initialize(settings))
                throw new InvalidProgramException();
        }
    }
}
