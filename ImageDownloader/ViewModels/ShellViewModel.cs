using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using ImageDownloader.Models;
using ImageDownloader.Utils;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveConductor<IPage>, IShell, IHandle<NavigationMessage>
    {
        private static ILog log = LogManager.GetLog(typeof(ShellViewModel));

        private IEventAggregator event_aggregator;
        private Settings settings;
        private ProjectSelectionPageViewModel project_selection_page;
        private ProjectPageViewModel project_page;

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

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator event_aggregator,
                              Settings settings,
                              ProjectSelectionPageViewModel project_selection_page,
                              ProjectPageViewModel project_page,
                              [ImportMany] IEnumerable<FlyoutBase> flyouts)
        {
            DisplayName = "Image Downloader";

            this.event_aggregator = event_aggregator;
            this.settings = settings;
            this.project_selection_page = project_selection_page;
            this.project_page = project_page;
            FlyoutViewModels = new ReactiveList<FlyoutBase>(flyouts);
            FlyoutCommands = FlyoutViewModels.CreateDerivedCollection(f => new FlyoutCommandViewModel(f), f => f.ShowInTitlebar);

            event_aggregator.Subscribe(this);
            ActivateItem(project_selection_page);
        }

        public void ShowAbout()
        {
            event_aggregator.PublishOnCurrentThread(NavigationMessage.NavigateToProjectSelectionPage());
        }

        public void ToggleDebug()
        {
            settings.DebugEnabled = !settings.DebugEnabled;
        }

        public void Handle(NavigationMessage message)
        {
            switch (message.NavigateToPage)
            {
                case NavigationMessage.Page.ProjectSelection:
                    ActivateItem(project_selection_page);
                    break;
                case NavigationMessage.Page.ProjectPage:
                    ActivateItem(project_page);
                    break;
                default:
                    break;
            }
        }
    }
}