using ImageDownloader.Interfaces;
using System.ComponentModel.Composition;
using ReactiveUI;
using System;
using System.Windows.Threading;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IStep))]
    [ExportMetadata("Order", 4)]
    public class ImagesStepViewModel : StepBase
    {
        private Random rnd = new Random();
        private DispatcherTimer timer;

        private string _BannerText = "Item 0";
        public string BannerText
        {
            get { return _BannerText; }
            set { this.RaiseAndSetIfChanged(ref _BannerText, value); }
        }

        private bool _ShowBanner = false;
        public bool ShowBanner
        {
            get { return _ShowBanner; }
            set { this.RaiseAndSetIfChanged(ref _ShowBanner, value); }
        }

        [ImportingConstructor]
        public ImagesStepViewModel() : base("Images")
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(750);
            timer.Tick += TimerTick;
            //timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            ShowBanner = false;
            timer.Stop();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            IsEnabled = true;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                IsEnabled = false;
        }

        public void ItemChanged()
        {
            //BannerText = string.Format("Item {0}", rnd.Next(1, 11));
            ShowBanner = !ShowBanner;
            //timer.Stop();
            //timer.Start();
        }
    }
}
