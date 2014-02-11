using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Webscraper
{
    public class EndStep : Step
    {
        private bool should_execute_on_load = false;
        private int downloaded_images = 0;
        private int existing_images = 0;
        private object directory_lock = new object();

        private int _DownloadCount = 0;
        public int DownloadCount
        {
            get { return _DownloadCount; }
            set
            {
                if (_DownloadCount == value) return;
                _DownloadCount = value;
                NotifyPropertyChanged();
            }
        }

        private int _ExistingCount = 0;
        public int ExistingCount
        {
            get { return _ExistingCount; }
            set
            {
                if (_ExistingCount == value) return;
                _ExistingCount = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand RestartCommand { get; private set; }

        public EndStep(Settings settings, IApplicationController controller) : base("Results", settings, controller)
        {
            RestartCommand = new RelayCommand(_ => controller.Restart());
        }

        protected override void OnLoaded()
        {
            if (!should_execute_on_load) return;

            controller.IsBusy = true;
            controller.BusyText = "Downloading images...";
            should_execute_on_load = false;

            DownloadCount = 0;

            Task.Factory.StartNew(() => DownloadAll())
                        .ContinueWith(parent => Finished(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Finished()
        {
            controller.IsBusy = false;
            DownloadCount = downloaded_images;
            ExistingCount = existing_images;
        }

        private void DownloadAll()
        {
            var images = controller.Images.Select(i => i.Text);
            var excluded_parts = settings.ExcludedParts.Select(p => p.Text.ToLower());

            Parallel.ForEach(images, new ParallelOptions { MaxDegreeOfParallelism = 4 },
                img =>
                {
                    var uri = new Uri(img);
                    var segments = uri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    var filtered_segments = segments.Where(s => !excluded_parts.Contains(s.ToLower()));
                    var image_path = Path.Combine(settings.OutputDir, string.Join(@"\", filtered_segments));

                    if (File.Exists(image_path))
                    {
                        Interlocked.Increment(ref existing_images);
                        return;
                    }

                    // Make sure directories exists
                    var image_directory = Path.GetDirectoryName(image_path);
                    if (!Directory.Exists(image_directory))
                    {
                        lock (directory_lock)
                        {
                            Directory.CreateDirectory(image_directory);
                        }
                    }

                    using (var client = new WebClient())
                    {
                        client.DownloadFile(img, image_path);
                    }

                    Interlocked.Increment(ref downloaded_images);
                });
        }

        public override void Activate()
        {
            should_execute_on_load = true;
        }
    }
}
