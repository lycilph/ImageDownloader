using System;
using System.IO;
using System.Linq;
using ImageDownloader.Controllers;
using ImageDownloader.Model;
using Panda.Utilities.Extensions;
using ReactiveUI;

namespace ImageDownloader.Screens.Option
{
    public sealed class OptionViewModel : BaseViewModel
    {
        private SiteOptions options;

        private bool _UseCache;
        public bool UseCache
        {
            get { return _UseCache; }
            set { this.RaiseAndSetIfChanged(ref _UseCache, value); }
        }

        private int _CacheLifetime;
        public int CacheLifetime
        {
            get { return _CacheLifetime; }
            set { this.RaiseAndSetIfChanged(ref _CacheLifetime, value); }
        }

        public OptionViewModel(ApplicationController controller) : base(controller)
        {
            DisplayName = "Options";
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var path = GetOptionFilename();
            options = File.Exists(path) ? JsonExtensions.ReadFromFile<SiteOptions>(path) : controller.Settings.GetDefaultSiteOptions();
        }

        private string GetOptionFilename()
        {
            var uri = new Uri(controller.SiteInformation.Url);
            var host = uri.Host;
            var segments = uri.Segments.Skip(1).Select(s => s.TrimEnd(new[] {'/'})).Aggregate(string.Empty, (s, s1) => s + s1);
            var filename = (host + segments).Replace(".", "") + ".option";
            return Path.Combine(controller.Settings.DataFolder, filename);
        }
    }
}
