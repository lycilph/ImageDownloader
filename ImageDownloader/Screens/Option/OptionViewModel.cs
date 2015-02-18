using System;
using System.IO;
using System.Linq;
using AutoMapper;
using ImageDownloader.Controllers;
using ImageDownloader.Model;
using Panda.Utilities.Extensions;
using ReactiveUI;

namespace ImageDownloader.Screens.Option
{
    public sealed class OptionViewModel : BaseViewModel
    {
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

        private string _Folder;
        public string Folder
        {
            get { return _Folder; }
            set { this.RaiseAndSetIfChanged(ref _Folder, value); }
        }

        private bool _FlattenFilename;
        public bool FlattenFilename
        {
            get { return _FlattenFilename; }
            set { this.RaiseAndSetIfChanged(ref _FlattenFilename, value); }
        }

        private bool _OnlySubpages;
        public bool OnlySubpages
        {
            get { return _OnlySubpages; }
            set { this.RaiseAndSetIfChanged(ref _OnlySubpages, value); }
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

        public OptionViewModel(ApplicationController controller) : base(controller)
        {
            DisplayName = "Options";
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var path = GetOptionFilename();
            var options = File.Exists(path) ? JsonExtensions.ReadFromFile<SiteOptions>(path) : controller.Settings.GetDefaultSiteOptions();
            Mapper.Map(options, this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            var path = GetOptionFilename();
            var options = new SiteOptions();
            Mapper.Map(this, options);
            JsonExtensions.WriteToFile(path, options);
        }

        private string GetOptionFilename()
        {
            var uri = new Uri(controller.SiteInformation.Url);
            var host = uri.Host;
            var segments = uri.Segments.Skip(1).Select(s => s.TrimEnd(new[] {'/'})).Aggregate(string.Empty, (s, s1) => s + s1);
            var filename = (host + segments).Replace(".", "") + ".option";
            return Path.Combine(controller.Settings.DataFolder, filename);
        }
    }
}
