using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ImageDownloader.Controllers;
using Microsoft.Win32;
using ReactiveUI;

namespace ImageDownloader.Screens.Site
{
    public class SiteOptionViewModel : BaseViewModel
    {
        private readonly SiteViewModel site_view_model;

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        private string _ExclusionExtension;
        public string ExclusionExtension
        {
            get { return _ExclusionExtension; }
            set { this.RaiseAndSetIfChanged(ref _ExclusionExtension, value); }
        }

        private string _ExclusionString;
        public string ExclusionString
        {
            get { return _ExclusionString; }
            set { this.RaiseAndSetIfChanged(ref _ExclusionString, value); }
        }

        private string _CurrentExtension;
        public string CurrentExtension
        {
            get { return _CurrentExtension; }
            set { this.RaiseAndSetIfChanged(ref _CurrentExtension, value); }
        }

        private string _CurrentString;
        public string CurrentString
        {
            get { return _CurrentString; }
            set { this.RaiseAndSetIfChanged(ref _CurrentString, value); }
        }

        private ReactiveList<string> _Extensions = new ReactiveList<string>();
        public ReactiveList<string> Extensions
        {
            get { return _Extensions; }
            set { this.RaiseAndSetIfChanged(ref _Extensions, value); }
        }

        private ReactiveList<string> _Strings = new ReactiveList<string>();
        public ReactiveList<string> Strings
        {
            get { return _Strings; }
            set { this.RaiseAndSetIfChanged(ref _Strings, value); }
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

        public SiteOptionViewModel(ApplicationController controller, SiteViewModel site_view_model) : base(controller)
        {
            this.site_view_model = site_view_model;
            controller.WhenAnyValue(x => x.IsBusy).Subscribe(x => IsBusy = x);

            _CanSave = this.WhenAny(x => x.IsBusy, x => !x.Value)
                           .ToProperty(this, x => x.CanSave);

            _CanAddExtension = this.WhenAny(x => x.IsBusy, x => !x.Value)
                                   .ToProperty(this, x => x.CanAddExtension);

            _CanRemoveExtension = this.WhenAny(x => x.IsBusy, x => x.CurrentExtension, (busy, ext) => !busy.Value && ext.Value != null)
                                      .ToProperty(this, x => x.CanRemoveExtension);

            _CanAddString = this.WhenAny(x => x.IsBusy, x => !x.Value)
                                .ToProperty(this, x => x.CanAddString);

            _CanRemoveString = this.WhenAny(x => x.IsBusy, x => x.CurrentString, (busy, str) => !busy.Value && str.Value != null)
                                   .ToProperty(this, x => x.CanRemoveString);
        }

        private async void UpdateExclusions()
        {
            controller.IsBusy = true;
            await Task.Factory.StartNew(() => site_view_model.Nodes.First().UpdateExclusions(Strings, Extensions));
            controller.IsBusy = false;
        }

        public void Save()
        {
            var save_file_dialog = new SaveFileDialog
            {
                InitialDirectory = controller.Settings.DataFolder,
                DefaultExt = ".site",
                Filter = "Site file (.site)|*.site"
            };

            if (save_file_dialog.ShowDialog() == true)
            {
                controller.SiteInformation.Sitemap.Save(save_file_dialog.FileName);
                controller.MainStatusText = string.Format("Saved {0} to {1}", controller.SiteInformation.Url, save_file_dialog.FileName);
            }
        }

        public void AddExtensionOnEnter(Key key)
        {
            if (key == Key.Enter || key == Key.Return)
                AddExtension();
        }

        public void AddExtension()
        {
            if (!string.IsNullOrWhiteSpace(ExclusionExtension) && !Extensions.Contains(ExclusionExtension))
            {
                Extensions.Add(ExclusionExtension.ToLowerInvariant());
                ExclusionExtension = string.Empty;
                UpdateExclusions();
            }
        }

        public void RemoveExtension()
        {
            Extensions.Remove(CurrentExtension);
            UpdateExclusions();
        }

        public void AddStringOnEnter(Key key)
        {
            if (key == Key.Enter || key == Key.Return)
                AddString();
        }

        public void AddString()
        {
            if (!string.IsNullOrWhiteSpace(ExclusionString) && !Strings.Contains(ExclusionString))
            {
                Strings.Add(ExclusionString.ToLowerInvariant());
                ExclusionString = string.Empty;
                UpdateExclusions();
            }
        }

        public void RemoveString()
        {
            Strings.Remove(CurrentString);
            UpdateExclusions();
        }
    }
}
