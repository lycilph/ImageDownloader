using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using ImageDownloader.Controllers;
using ImageDownloader.Data;
using ImageDownloader.Screens.Download.Option;
using Panda.ApplicationCore;
using ReactiveUI;
using WebCrawler.Extensions;

namespace ImageDownloader.Screens.Download
{
    [Export(typeof(StepScreen))]
    [Export(typeof(DownloadViewModel))]
    [ExportOrder(5)]
    public sealed class DownloadViewModel : StepScreen
    {
        private readonly Settings settings;
        private readonly SiteController site_controller;
        private readonly StatusController status_controller;

        private ReactiveList<string> _DownloadedFiles = new ReactiveList<string>();
        public ReactiveList<string> DownloadedFiles
        {
            get { return _DownloadedFiles; }
            set { this.RaiseAndSetIfChanged(ref _DownloadedFiles, value); }
        }

        public override bool CanNext
        {
            get { return false; }
            protected set { throw new NotSupportedException(); }
        }

        public override bool CanPrevious
        {
            get { return true; }
            protected set { throw new NotSupportedException(); }
        }

        [ImportingConstructor]
        public DownloadViewModel(Settings settings, SiteController site_controller, StatusController status_controller, DownloadOptionViewModel option_view_model)
        {
            DisplayName = "Download";
            this.settings = settings;
            this.site_controller = site_controller;
            this.status_controller = status_controller;

            Option = option_view_model;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            status_controller.IsBusy = true;

            var host_folder = site_controller.Sitemap.Name.GetHost().MakeFilenameSafe();
            var base_folder = Path.Combine(settings.DataFolder, host_folder);

            DownloadedFiles = new ReactiveList<string>();
            IProgress<string> progress = new Progress<string>(DownloadedFiles.Add);
            await Task.Factory.StartNew(() =>
            {
                using (var client = new WebClient())
                {
                    foreach (var file in site_controller.SelectedFiles)
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
                            client.DownloadFile(file, path);
                            progress.Report(file + " downloaded");
                        }
                        else
                        {
                            progress.Report(file + " already exists!");
                        }
                        Thread.Sleep(Settings.ThreadDelay);
                    }
                }
            }, TaskCreationOptions.LongRunning);

            DownloadedFiles.Add("Done!");
            status_controller.IsBusy = false;
        }
    }
}
