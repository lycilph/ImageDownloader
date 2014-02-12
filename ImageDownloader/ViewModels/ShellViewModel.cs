using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using ImageDownloader.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveConductor<IPage>, IShell, IPartImportsSatisfiedNotification, IHandle<NavigationMessage>
    {
        private static ILog log = LogManager.GetLog(typeof(ShellViewModel));

        [Import]
        private IEventAggregator EventAggregator { get; set; }

        [Import]
        private ProjectSelectionPageViewModel ProjectSelectionPage { get; set; }

        [Import]
        private ProjectPageViewModel ProjectPage { get; set; }

        private ReactiveList<FlyoutBase> _FlyoutViewModels = new ReactiveList<FlyoutBase>();
        [ImportMany]
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

        public ShellViewModel()
        {
            DisplayName = "Image Downloader";
            FlyoutCommands = FlyoutViewModels.CreateDerivedCollection(f => new FlyoutCommandViewModel(f), f => f.ShowInTitlebar);
        }

        public void ShowAbout()
        {
            EventAggregator.PublishOnCurrentThread(NavigationMessage.NavigateToProjectSelectionPage());
        }

        public void OnImportsSatisfied()
        {
            EventAggregator.Subscribe(this);
            ActivateItem(ProjectSelectionPage);
        }

        public void Handle(NavigationMessage message)
        {
            switch (message.NavigateToPage)
            {
                case NavigationMessage.Page.ProjectSelection:
                    ActivateItem(ProjectSelectionPage);
                    break;
                case NavigationMessage.Page.ProjectPage:
                    ActivateItem(ProjectPage);
                    break;
                default:
                    break;
            }
        }
    }
}