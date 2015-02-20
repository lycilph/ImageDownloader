using System;
using System.ComponentModel.Composition;
using System.Security.Policy;
using AutoMapper;
using ImageDownloader.Controllers;
using Panda.ApplicationCore;
using ReactiveUI;

namespace ImageDownloader.Screens.Options
{
    [Export(typeof(StepScreenBase))]
    [Export(typeof(OptionsViewModel))]
    [ExportOrder(2)]
    public sealed class OptionsViewModel : StepScreenBase
    {
        private readonly StatusController status_controller;
        private readonly SiteController site_controller;

        private string _Folder;
        public string Folder
        {
            get { return _Folder; }
            set { this.RaiseAndSetIfChanged(ref _Folder, value); }
        }

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
        public OptionsViewModel(StatusController status_controller, SiteController site_controller)
        {
            DisplayName = "Options";
            this.status_controller = status_controller;
            this.site_controller = site_controller;
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Mapper.Map(site_controller.SiteOptions, this);
            
            status_controller.IsBusy = true;
            await site_controller.SiteCacheTask;
            status_controller.IsBusy = false;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            Mapper.Map(this, site_controller.SiteOptions);
            site_controller.SaveSiteOptions();
        }
    }
}
