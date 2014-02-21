using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using ImageDownloader.Messages;
using ImageDownloader.Models;
using ImageDownloader.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveConductor<IPage>, IShell, IHandle<PageType>, IHandle<ShellMessage>, IViewAware
    {
        private static ILog log = LogManager.GetLog(typeof(ShellViewModel));

        private IRepository repository;
        private IEventAggregator event_aggregator;
        private Settings settings;
        private List<IPage> pages;

        private ReactiveList<FlyoutBase> _FlyoutViewModels;
        public ReactiveList<FlyoutBase> FlyoutViewModels
        {
            get { return _FlyoutViewModels; }
            set { this.RaiseAndSetIfChanged(ref _FlyoutViewModels, value); }
        }

        private IReactiveDerivedList<FlyoutCommandViewModel> _FlyoutCommands;
        public IReactiveDerivedList<FlyoutCommandViewModel> FlyoutCommands
        {
            get { return _FlyoutCommands; }
            set { this.RaiseAndSetIfChanged(ref _FlyoutCommands, value); }
        }

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator event_aggregator,
                              IRepository repository,
                              Settings settings,
                              [ImportMany] IEnumerable<IPage> pages,
                              [ImportMany] IEnumerable<FlyoutBase> flyouts)
        {
            this.repository = repository;
            this.settings = settings;
            this.event_aggregator = event_aggregator;
            this.pages = new List<IPage>(pages);

            repository.WhenAnyDynamic(new string[] { "Current" },
                                      new string[] { "Current", "Name" },
                                      (p1, p2) => p1)
                .Subscribe(x => CurrentProjectChanged((Project)x.Value));

            FlyoutViewModels = new ReactiveList<FlyoutBase>(flyouts);
            FlyoutCommands = FlyoutViewModels.CreateDerivedCollection(f => new FlyoutCommandViewModel(f), f => f.ShowInTitlebar);

            event_aggregator.Subscribe(this);
            Handle(PageType.ProjectSelection);
        }

        public async void ShowAbout()
        {
            await DialogService.ShowAboutDialog();
        }

        public void ToggleDebug()
        {
            settings.ToggleDebug();
        }

        public void Edit()
        {
            ActiveItem.Edit();
        }

        private void CurrentProjectChanged(Project project)
        {
            DisplayName = (project == Project.Empty ? "Image Downloader" : "Image Downloader - " + project.Name);
        }

        public void Handle(PageType page)
        {
            var vm = pages.FirstOrDefault(p => p.Page == page);
            if (vm != null)
                ActivateItem(vm);
            else
                throw new InvalidOperationException("Unknown page type");
        }

        public void Handle(ShellMessage state)
        {
            IsEnabled = (state == ShellMessage.Enabled);
        }
    }
}