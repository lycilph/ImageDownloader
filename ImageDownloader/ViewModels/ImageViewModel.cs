using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace ImageDownloader.ViewModels
{
    public class ImageViewModel : ReactiveObject
    {
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
                {
                    return new Uri("/ImageDownloader;Component/Images/appbar.image.png", UriKind.Relative);
                }
                return _Image;
            }
            private set { this.RaiseAndSetIfChanged(ref _Image, value); }
        }

        public ImageViewModel(string url)
        {
            _Url = url;
        }
    }
}
