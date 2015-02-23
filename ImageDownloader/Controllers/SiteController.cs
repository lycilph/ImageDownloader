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
            SiteOptions.CacheTask = Task.Factory.StartNew(() => new Cache(path, SiteOptions.CacheLifetime), TaskCreationOptions.LongRunning);
        }

        public async Task UpdateSiteOptions()
        {
            var path = GetSiteOptionsPath();
            JsonExtensions.WriteToFile(path, SiteOptions);

            if (SiteOptions.UseCache && SiteOptions.CacheTask == null)
                LoadOrCreateSiteCache();

            if (!SiteOptions.UseCache && SiteOptions.CacheTask != null)
                await CleanupCache();
        }

        public async Task Cleanup()
        {
            Url = null;
            SiteOptions = null;
            await CleanupCache();
        }

        public async Task CleanupCache()
        {
            if (SiteOptions.CacheTask == null)
                return;

            var cache = await SiteOptions.CacheTask;
            cache.Dispose();
            SiteOptions.CacheTask = null;
        }

        public void Initialize(string url)
        {
            Url = url;
            settings.SetFavoriteUrl(url);
            LoadOrCreateSiteOptions();

            if (SiteOptions.UseCache)
                LoadOrCreateSiteCache();
        }

        public void Load(string path)
        {
        }
    }
}
