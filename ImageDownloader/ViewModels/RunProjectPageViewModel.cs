using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ReactiveUI;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IPage))]
    public class RunProjectPageViewModel : ReactiveScreen, IPage
    {
        private IEventAggregator event_aggregator;
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

        public PageType Page
        {
            get { return PageType.RunProject; }
        }

        [ImportingConstructor]
        public RunProjectPageViewModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;

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

        public void Edit() { }

        public void Back()
        {
            event_aggregator.PublishOnCurrentThread(PageType.ProjectSelection);
        }
    }
}
