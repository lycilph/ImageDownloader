using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageDownloader.Data;
using Panda.Utilities.Extensions;
using WebCrawler.Data;
using WebCrawler.Extensions;

namespace ImageDownloader.Controllers
{
    [Export(typeof(SiteController))]
    public class SiteController
    {
        private const string OptionsExtension = ".options";
        private const string CacheExtension = ".cache";

        private readonly Settings settings;

        public string Url { get; set; }
        public SiteOptions SiteOptions { get; set; }
        public Task<Cache> SiteCacheTask { get; set; }

        [ImportingConstructor]
        public SiteController(Settings settings)
        {
            this.settings = settings;
        }

        private string GetSiteOptionsPath()
        {
            var uri = new Uri(Url);
            var host = uri.Host;
            var segments = uri.Segments.Skip(1).Select(s => s.TrimEnd(new[] { '/' })).Aggregate(string.Empty, (str, segment) => str + segment);
            var filename = (host + segments).Replace(".", "") + OptionsExtension;
            return Path.Combine(settings.DataFolder, filename);
        }

        private string GetCachePath()
        {
            var filename = Url.GetHost().MakeFilenameSafe() + CacheExtension;
            return Path.Combine(settings.DataFolder, filename);
        }

        private void LoadOrCreateSiteOptions()
        {
            var path = GetSiteOptionsPath();
            SiteOptions = File.Exists(path) ? JsonExtensions.ReadFromFile<SiteOptions>(path) : settings.GetDefaultSiteOptions();
        }

        private void LoadOrCreateSiteCache()
        {
            var path = GetCachePath();
            SiteCacheTask = Task.Factory.StartNew(() => new Cache(path), TaskCreationOptions.LongRunning);
        }

        public void SaveSiteOptions()
        {
            var path = GetSiteOptionsPath();
            JsonExtensions.WriteToFile(path, SiteOptions);
        }

        public void Cleanup()
        {
            Url = null;
            SiteOptions = null;
            if (SiteCacheTask == null) 
                return;

            var cache = SiteCacheTask.Result;
            cache.Dispose();
            SiteCacheTask = null;
        }

        public void Initialize(string url)
        {
            Cleanup();

            Url = url;
            settings.SetFavoriteUrl(url);
            LoadOrCreateSiteOptions();
            LoadOrCreateSiteCache();
        }

        public void Load(string path)
        {
            Cleanup();            
        }
    }
}
