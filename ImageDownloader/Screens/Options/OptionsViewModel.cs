using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using ImageDownloader.Controllers;
using Ookii.Dialogs.Wpf;
using Panda.ApplicationCore;
using ReactiveUI;

namespace ImageDownloader.Screens.Options
{
    [Export(typeof(StepScreen))]
    [Export(typeof(OptionsViewModel))]
    [ExportOrder(2)]
    public sealed class OptionsViewModel : StepScreen
    {
        private readonly SiteController site_controller;

        private string _Folder;
        public string Folder
        {
            get { return _Folder; }
            set { this.RaiseAndSetIfChanged(ref _Folder, value); }
        }

        private bool _UseCache;
        public bool UseCache
        {
            get { return _UseCache; }
            set { this.RaiseAndSetIfChanged(ref _UseCache, value); }
        }

        private int _CacheLifetime;
        public int CacheLifetime
        {
            get { return _CacheLifetime; }
            set { this.RaiseAndSetIfChanged(ref _CacheLifetime, value); }
        }

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

        private readonly ObservableAsPropertyHelper<bool> _CanRemoveExtension;
        public bool CanRemoveExtension { get { return _CanRemoveExtension.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanRemoveString;
        public bool CanRemoveString { get { return _CanRemoveString.Value; } }

        private bool _CanNext = true;
        public override bool CanNext
        {
            get { return _CanNext; }
            protected set { this.RaiseAndSetIfChanged(ref _CanNext, value); }
        }

        public override bool CanPrevious
        {
            get { return true; }
            protected set { throw new NotSupportedException(); }
        }
        
        [ImportingConstructor]
        public OptionsViewModel(SiteController site_controller)
        {
            DisplayName = "Options";
            this.site_controller = site_controller;

            _CanRemoveExtension = this.WhenAny(x => x.CurrentExcludedExtension, x => !string.IsNullOrWhiteSpace(x.Value))
                                      .ToProperty(this, x => x.CanRemoveExtension);

            _CanRemoveString = this.WhenAny(x => x.CurrentExcludedString, x => !string.IsNullOrWhiteSpace(x.Value))
                                   .ToProperty(this, x => x.CanRemoveString);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Mapper.Map(site_controller.SiteOptions, this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            Mapper.Map(this, site_controller.SiteOptions);
            // This is done in a task, so that is the cache is loaded, it doesn't disrupt the ui
            Task.Factory.StartNew(() => site_controller.UpdateSiteOptions().Wait());
        }

        public void BrowseFolder()
        {
            var folder_dialog = new VistaFolderBrowserDialog();
            if (folder_dialog.ShowDialog() == true)
            {
                Folder = folder_dialog.SelectedPath;
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
            }
        }

        public void RemoveExtension()
        {
            ExcludedExtensions.Remove(CurrentExcludedExtension);
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
            }
        }

        public void RemoveString()
        {
            ExcludedStrings.Remove(CurrentExcludedString);
        }
    }
}
