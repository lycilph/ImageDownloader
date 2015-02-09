using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core;
using NLog;
using ReactiveUI;

namespace ImageDownloader
{
    public sealed class DownloadViewModel : StepViewModel
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        private ReactiveList<DownloadItemViewModel> _Items;
        public ReactiveList<DownloadItemViewModel> Items
        {
            get { return _Items; }
            set { this.RaiseAndSetIfChanged(ref _Items, value); }
        }

        public DownloadViewModel(Settings settings, ShellViewModel shell) : base(settings, shell)
        {
            DisplayName = "Download";
            shell.WhenAnyValue(x => x.IsBusy).Subscribe(x => IsBusy = x);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Items = new ReactiveList<DownloadItemViewModel>(shell.Selection.Files.Select(f => new DownloadItemViewModel(f)));
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            shell.IsBusy = true;

            await Task.Factory.StartNew(() =>
            {
                using (var client = new WebClient())
                {
                    foreach (var item in Items)
                    {
                        var uri = new Uri(item.Text);
                        var base_dir = uri.Host.MakeFilenameSafe();
                        var sub_dirs = uri.Segments
                                          .Select(s => s.TrimEnd(new[] { '/' }))
                                          .Where(s => !string.IsNullOrWhiteSpace(s))
                                          .Aggregate(Path.Combine);
                        var path = Path.Combine(settings.DataFolder, base_dir, sub_dirs);

                        var dir = Path.GetDirectoryName(path);
                        Directory.CreateDirectory(dir);

                        client.DownloadFile(item.Text, path);
                        item.Done = true;
                        Thread.Sleep(250);
                    }
                }
            });

            shell.IsBusy = false;
        }

        public void StartStop()
        {
            shell.IsBusy = !shell.IsBusy;
        }
    }
}
