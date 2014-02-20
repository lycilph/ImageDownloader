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
        private bool is_changing_item;

        private string _BannerText = "Item 0";
        public string BannerText
        {
            get { return _BannerText; }
            set { this.RaiseAndSetIfChanged(ref _BannerText, value); }
        }

        private bool _IsBannerShown = true;
        public bool IsBannerShown
        {
            get { return _IsBannerShown; }
            set { this.RaiseAndSetIfChanged(ref _IsBannerShown, value); }
        }

        [ImportingConstructor]
        public ImagesStepViewModel() : base("Images")
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(3000);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            IsBannerShown = false;
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
            is_changing_item = true;

            BannerText = string.Format("Item {0}", rnd.Next(1, 11));
            IsBannerShown = true;
            timer.Stop();
            timer.Start();
        }

        public void ShowBanner()
        {
            IsBannerShown = true;
            timer.Stop();
        }

        public void HideBanner()
        {
            if (is_changing_item)
            {
                is_changing_item = false;
                return;
            }

            IsBannerShown = false;
            timer.Stop();
        }
    }
}
