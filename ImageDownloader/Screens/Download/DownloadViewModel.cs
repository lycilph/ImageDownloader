using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ImageDownloader.Controllers;
using ImageDownloader.Model;
using Panda.Utilities.Extensions;
using ReactiveUI;
using WebCrawler.Extensions;

namespace ImageDownloader.Screens.Download
{
    public sealed class DownloadViewModel : BaseViewModel
    {
        private ReactiveList<string> _Files = new ReactiveList<string>();
        public ReactiveList<string> Files
        {
            get { return _Files; }
            set { this.RaiseAndSetIfChanged(ref _Files, value); }
        }

        public DownloadViewModel(ApplicationController controller) : base(controller)
        {
            DisplayName = "Download";
            Option = new DownloadOptionViewModel(controller);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            controller.IsBusy = true;

            var host_folder = controller.SiteInformation.Sitemap.Name.GetHost().MakeFilenameSafe();
            var base_folder = Path.Combine(controller.Settings.DataFolder, host_folder);

            Files = new ReactiveList<string>();
            IProgress<string> progress = new Progress<string>(Files.Add);
            await Task.Factory.StartNew(() =>
            {
                foreach (var file in controller.SiteInformation.Files)
                {
                    var uri = new Uri(file);
                    var folder = base_folder;
                    uri.Segments
                        .Skip(1)
                        .Take(uri.Segments.Count() - 2)
                        .Select(s => s.TrimEnd(new[] {'/'}))
                        .Apply(s => folder = Path.Combine(folder, s));
                    var path = Path.Combine(folder, uri.Segments.Last());

                    Directory.CreateDirectory(folder);
                    if (!File.Exists(path))
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(file, path);
                        }
                        progress.Report(file + " downloaded");
                    }
                    else
                    {
                        progress.Report(file + " already exists!");
                    }
                    Thread.Sleep(Settings.ThreadDelay);
                }
            });

            controller.IsBusy = false;
        }
    }
}
