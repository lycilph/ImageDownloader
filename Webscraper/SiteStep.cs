using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Webscraper
{
    public class SiteStep : Step
    {
        private bool should_execute_on_load;
        private Scraper scraper;

        public ObservableCollection<ItemViewModel> Pages
        {
            get { return controller.Pages; }
        }
        public ObservableCollection<string> Rejected { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public SiteStep(Settings settings, IApplicationController controller) : base("Website", settings, controller)
        {
            Rejected = new ObservableCollection<string>();
            DeleteCommand = new RelayCommand(_ => Pages.RemoveIf(p => p.IsSelected), _ => Pages.Any(p => p.IsSelected));
            should_execute_on_load = false;

            var progress = new Progress<ProgressInfo>(Report);
            scraper = new Scraper(controller.Loader, settings, progress);
        }

        protected override void OnLoaded()
        {
            if (!should_execute_on_load) return;

            controller.IsBusy = true;
            controller.BusyText = "Analysing website...";
            should_execute_on_load = false;

            Pages.Clear();
            Rejected.Clear();

            Task.Factory.StartNew(() => scraper.DownloadAllPages(settings.Website))
                        .ContinueWith(parent => controller.IsBusy = false, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Report(ProgressInfo info)
        {
            if (info.accepted)
                Pages.Add(new ItemViewModel(info.url));
            else
                Rejected.Add(info.url);

            var count = Pages.Count();
            var str = (count == 1 ? "page" : "pages");
            controller.BusyText = string.Format("{0} {1} found", count, str);
        }

        public override void Activate()
        {
            should_execute_on_load = true;
        }

        public override bool CanGotoPrev()
        {
            return true;
        }

        public override bool CanGotoNext()
        {
            return Pages.Any();
        }
    }
}
