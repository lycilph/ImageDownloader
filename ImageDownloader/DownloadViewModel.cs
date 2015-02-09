using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Core;
using ReactiveUI;

namespace ImageDownloader
{
    public sealed class DownloadViewModel : StepViewModel
    {
        private Task worker_task;
        private CancellationTokenSource cts;

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        private bool _IsAllItemsDownloaded;
        public bool IsAllItemsDownloaded
        {
            get { return _IsAllItemsDownloaded; }
            set { this.RaiseAndSetIfChanged(ref _IsAllItemsDownloaded, value); }
        }

        private ReactiveList<DownloadItemViewModel> _Items;
        public ReactiveList<DownloadItemViewModel> Items
        {
            get { return _Items; }
            set { this.RaiseAndSetIfChanged(ref _Items, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanHome;
        public bool CanHome { get { return _CanHome.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanStartStop;
        public bool CanStartStop { get { return _CanStartStop.Value; } }

        public DownloadViewModel(Settings settings, ShellViewModel shell) : base(settings, shell)
        {
            DisplayName = "Download";
            shell.WhenAnyValue(x => x.IsBusy).Subscribe(x => IsBusy = x);

            _CanHome = this.WhenAny(x => x.IsBusy, x => !x.Value).ToProperty(this, x => x.CanHome);
            _CanStartStop = this.WhenAny(x => x.IsAllItemsDownloaded, x => !x.Value).ToProperty(this, x => x.CanStartStop);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Items = new ReactiveList<DownloadItemViewModel>(shell.Selection.Files.Select(f => new DownloadItemViewModel(f)));
        }

        protected override async void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            cts.Cancel();
            await worker_task;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await StartDownload();
        }

        private async Task StartDownload()
        {
            shell.IsBusy = true;
            IsAllItemsDownloaded = false;
            cts = new CancellationTokenSource();

            worker_task = Task.Factory.StartNew(() =>
            {
                using (var client = new WebClient())
                {
                    foreach (var item in Items.Where(i => !i.Done))
                    {
                        var uri = new Uri(item.Text);
                        var base_dir = uri.Host.MakeFilenameSafe();
                        var sub_dirs = uri.Segments
                            .Select(s => s.TrimEnd(new[] {'/'}))
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Aggregate(Path.Combine);
                        var path = Path.Combine(settings.DataFolder, base_dir, sub_dirs);

                        var dir = Path.GetDirectoryName(path);
                        Debug.Assert(dir != null, "dir != null");
                        Directory.CreateDirectory(dir);

                        client.DownloadFile(item.Text, path);
                        item.Done = true;

                        if (cts.Token.IsCancellationRequested)
                            break;

                        Thread.Sleep(Settings.ImageDownloadDelay);
                    }
                }
            }, cts.Token);
            await worker_task;

            shell.IsBusy = false;
            IsAllItemsDownloaded = Items.All(i => i.Done);
        }

        public void Home()
        {
            shell.Home();
        }

        public async void StartStop()
        {
            if (IsBusy)
            {
                cts.Cancel();
                await worker_task;
            }
            else
            {
                await StartDownload();
            }
        }
    }
}
