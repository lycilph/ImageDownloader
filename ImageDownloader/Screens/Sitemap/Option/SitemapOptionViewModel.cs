using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Controllers;
using ImageDownloader.Data;
using Microsoft.Win32;
using ReactiveUI;

namespace ImageDownloader.Screens.Sitemap.Option
{
    [Export(typeof(SitemapOptionViewModel))]
    public class SitemapOptionViewModel : ReactiveScreen
    {
        private readonly Settings settings;
        private readonly StatusController status_controller;
        private readonly SiteController site_controller;
        private SitemapNodeViewModel root_sitemap_node;

        private string _ExcludedExtensionText;
        public string ExcludedExtensionText
        {
            get { return _ExcludedExtensionText; }
            set { this.RaiseAndSetIfChanged(ref _ExcludedExtensionText, value); }
        }

        private string _ExcludedStringText;
        public string ExcludedStringText
        {
            get { return _ExcludedStringText; }
            set { this.RaiseAndSetIfChanged(ref _ExcludedStringText, value); }
        }

        private string _CurrentExcludedExtension;
        public string CurrentExcludedExtension
        {
            get { return _CurrentExcludedExtension; }
            set { this.RaiseAndSetIfChanged(ref _CurrentExcludedExtension, value); }
        }

        private string _CurrentExcludedString;
        public string CurrentExcludedString
        {
            get { return _CurrentExcludedString; }
            set { this.RaiseAndSetIfChanged(ref _CurrentExcludedString, value); }
        }

        private ReactiveList<string> _ExcludedExtensions = new ReactiveList<string>();
        public ReactiveList<string> ExcludedExtensions
        {
            get { return _ExcludedExtensions; }
            set { this.RaiseAndSetIfChanged(ref _ExcludedExtensions, value); }
        }

        private ReactiveList<string> _ExcludedStrings = new ReactiveList<string>();
        public ReactiveList<string> ExcludedStrings
        {
            get { return _ExcludedStrings; }
            set { this.RaiseAndSetIfChanged(ref _ExcludedStrings, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanSave;
        public bool CanSave { get { return _CanSave.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanAddExtension;
        public bool CanAddExtension { get { return _CanAddExtension.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanRemoveExtension;
        public bool CanRemoveExtension { get { return _CanRemoveExtension.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanAddString;
        public bool CanAddString { get { return _CanAddString.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanRemoveString;
        public bool CanRemoveString { get { return _CanRemoveString.Value; } }

        [ImportingConstructor]
        public SitemapOptionViewModel(Settings settings, StatusController status_controller, SiteController site_controller)
        {
            this.settings = settings;
            this.status_controller = status_controller;
            this.site_controller = site_controller;

            _CanSave = status_controller.WhenAny(x => x.IsBusy, x => !x.Value)
                                        .ToProperty(this, x => x.CanSave);

            _CanAddExtension = status_controller.WhenAny(x => x.IsBusy, x => !x.Value)
                                                .ToProperty(this, x => x.CanAddExtension);

            _CanRemoveExtension = this.WhenAny(x => x.CurrentExcludedExtension, x => !string.IsNullOrWhiteSpace(x.Value))
                                      .CombineLatest(status_controller.WhenAnyValue(x => x.IsBusy), (has_text, busy) => has_text && !busy)
                                      .ToProperty(this, x => x.CanRemoveExtension);

            _CanAddString = status_controller.WhenAny(x => x.IsBusy, x => !x.Value)
                                             .ToProperty(this, x => x.CanAddString);

            _CanRemoveString = this.WhenAny(x => x.CurrentExcludedString, x => !string.IsNullOrWhiteSpace(x.Value))
                                      .CombineLatest(status_controller.WhenAnyValue(x => x.IsBusy), (has_text, busy) => has_text && !busy)
                                      .ToProperty(this, x => x.CanRemoveString);
        }

        private async void UpdateExclusions()
        {
            status_controller.IsBusy = true;
            await Task.Factory.StartNew(() => root_sitemap_node.UpdateExclusions(ExcludedStrings, ExcludedExtensions));
            status_controller.IsBusy = false;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Mapper.Map(site_controller.SiteOptions, this);
            UpdateExclusions();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Mapper.Map(this, site_controller.SiteOptions);
        }

        public void SetRootNode(SitemapNodeViewModel node)
        {
            root_sitemap_node = node;
        }

        public void Save()
        {
            var save_file_dialog = new SaveFileDialog
            {
                InitialDirectory = settings.DataFolder,
                DefaultExt = ".site",
                Filter = "Site file (.site)|*.site"
            };

            if (save_file_dialog.ShowDialog() == true)
            {
                site_controller.Sitemap.Save(save_file_dialog.FileName);
                status_controller.MainStatusText = string.Format("Saved {0} to {1}", site_controller.Url, save_file_dialog.FileName);
            }
        }

        public void AddExtensionOnEnter(Key key)
        {
            if (key == Key.Enter || key == Key.Return)
                AddExtension();
        }

        public void AddExtension()
        {
            if (!string.IsNullOrWhiteSpace(ExcludedExtensionText) && !ExcludedExtensions.Contains(ExcludedExtensionText))
            {
                ExcludedExtensions.Add(ExcludedExtensionText.ToLowerInvariant());
                ExcludedExtensionText = string.Empty;
                UpdateExclusions();
            }
        }

        public void RemoveExtension()
        {
            ExcludedExtensions.Remove(CurrentExcludedExtension);
            UpdateExclusions();
        }

        public void AddStringOnEnter(Key key)
        {
            if (key == Key.Enter || key == Key.Return)
                AddString();
        }

        public void AddString()
        {
            if (!string.IsNullOrWhiteSpace(ExcludedStringText) && !ExcludedStrings.Contains(ExcludedStringText))
            {
                ExcludedStrings.Add(ExcludedStringText.ToLowerInvariant());
                ExcludedStringText = string.Empty;
                UpdateExclusions();
            }
        }

        public void RemoveString()
        {
            ExcludedStrings.Remove(CurrentExcludedString);
            UpdateExclusions();
        }
    }
}
