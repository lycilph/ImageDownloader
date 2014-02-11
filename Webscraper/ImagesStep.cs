using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Webscraper
{
    public class ImagesStep : Step
    {
        private bool should_execute_on_load;
        private Scraper scraper;

        public int MinWidth
        {
            get { return settings.MinWidth; }
            set { settings.MinWidth = value; }
        }

        public int MaxWidth
        {
            get { return settings.MaxWidth; }
            set { settings.MaxWidth = value; }
        }

        public int MinHeight
        {
            get { return settings.MinHeight; }
            set { settings.MinHeight = value; }
        }

        public int MaxHeight
        {
            get { return settings.MaxHeight; }
            set { settings.MaxHeight = value; }
        }

        public ObservableCollection<ItemViewModel> ExcludedParts
        {
            get { return settings.ExcludedParts; }
        }
        public ObservableCollection<ItemViewModel> Images
        {
            get { return controller.Images; }
        }
        public ObservableCollection<string> Errors { get; private set; }

        private string _PartToExclude = string.Empty;
        public string PartToExclude
        {
            get { return _PartToExclude; }
            set
            {
                if (_PartToExclude == value) return;
                _PartToExclude = value;
                NotifyPropertyChanged();
            }
        }

        private bool _ShowOptions = false;
        public bool ShowOptions
        {
            get { return _ShowOptions; }
            set
            {
                if (_ShowOptions == value) return;
                _ShowOptions = value;
                NotifyPropertyChanged();

                // Reanalyze images when options are hidden
                if (!_ShowOptions)
                {
                    Activate();
                    OnLoaded();
                }
            }
        }

        public ICommand DeleteCommand { get; private set; }
        public ICommand AddPartCommand { get; private set; }
        public ICommand RemovePartCommand { get; private set; }

        public ImagesStep(Settings settings, IApplicationController controller) : base("Images", settings, controller)
        {
            should_execute_on_load = false;
            Errors = new ObservableCollection<string>();
            DeleteCommand = new RelayCommand(_ => Images.RemoveIf(p => p.IsSelected), _ => Images.Any(p => p.IsSelected));
            RemovePartCommand = new RelayCommand(_ => ExcludedParts.RemoveIf(p => p.IsSelected), _ => ExcludedParts.Any(p => p.IsSelected) );
            AddPartCommand = new RelayCommand(_ => AddPart(), _ => !string.IsNullOrWhiteSpace(PartToExclude));

            var progress = new Progress<ProgressInfo>(Report);
            scraper = new Scraper(controller.Loader, settings, progress);
        }

        protected override void OnLoaded()
        {
            if (!should_execute_on_load) return;

            controller.IsBusy = true;
            controller.BusyText = "Finding images...";
            should_execute_on_load = false;

            Images.Clear();
            Errors.Clear();

            var urls = controller.Pages.Select(i => i.Text);
            Task.Factory.StartNew(() => scraper.FindAllImages(urls))
                        .ContinueWith(parent => Finish(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Finish()
        {
            controller.IsBusy = false;

            if (Errors.Any())
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("The following page(s) contained errors:");
                foreach (var e in Errors)
                    sb.AppendLine(" - " + e);
                MessageBox.Show(sb.ToString());
            }
        }

        private void Report(ProgressInfo info)
        {
            if (info.accepted)
            {
                var filtered_url = GetFilteredUrl(info.url);
                Images.Add(new ItemViewModel(info.url, filtered_url));
            }
            else
                Errors.Add(info.url);

            var count = Images.Count();
            var str = (count == 1 ? "image" : "images");
            controller.BusyText = string.Format("{0} {1} found", count, str);
        }

        private string GetFilteredUrl(string url)
        {
            if (!ExcludedParts.Any())
                return url;

            var segments = url.Split(new char[] { '/' });
            var excluded_parts_lower = ExcludedParts.Select(p => p.Text.ToLower());
            var filtered_segments = segments.Where(s => !excluded_parts_lower.Contains(s.ToLower()));

            return string.Join("/", filtered_segments);
        }

        private void AddPart()
        {
            ExcludedParts.Add(new ItemViewModel(PartToExclude));
            PartToExclude = "";
        }

        public override void Activate()
        {
            should_execute_on_load = true;
        }

        public override bool CanGotoPrev()
        {
            return !ShowOptions;
        }

        public override bool CanGotoNext()
        {
            return Images.Any() && !ShowOptions;
        }
    }
}
