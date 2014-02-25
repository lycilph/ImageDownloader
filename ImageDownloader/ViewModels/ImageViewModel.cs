using ImageDownloader.Interfaces;
using ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    public class ImageViewModel : ReactiveObject
    {
        private ICache cache;

        private string _Url;
        public string Url
        {
            get { return _Url; }
            set { this.RaiseAndSetIfChanged(ref _Url, value); }
        }

        private Uri _Image;
        public Uri Image
        {
            get
            {
                if (_Image == null)
                    return new Uri("/ImageDownloader;Component/Images/appbar.image.png", UriKind.Relative);
                return _Image;
            }
            private set { this.RaiseAndSetIfChanged(ref _Image, value); }
        }

        public ImageViewModel(string url, ICache cache)
        {
            this.cache = cache;
            _Url = url;
        }

        public void Update()
        {
            Task.Factory.StartNew(() => cache.GetImage(Url))
                        .ContinueWith(parent => 
                        {
                            if (!string.IsNullOrWhiteSpace(parent.Result))
                                Image = new Uri(parent.Result);
                        }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
