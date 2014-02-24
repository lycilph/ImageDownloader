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
    [Export(typeof(IPage))]
    public class ResultsPageViewModel : ReactiveScreen, IPage, IHandle<Result>
    {
        private IEventAggregator event_aggregator;

        private ReactiveList<ImageViewModel> _Images;
        public ReactiveList<ImageViewModel> Images
        {
            get { return _Images; }
            set { this.RaiseAndSetIfChanged(ref _Images, value); }
        }

        public PageType Page
        {
            get { return PageType.ShowResults; }
        }

        [ImportingConstructor]
        public ResultsPageViewModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;

            event_aggregator.Subscribe(this);
        }

        public void Edit() { }

        public void GoBack()
        {
            event_aggregator.PublishOnCurrentThread(PageType.ProjectSelection);
        }

        public void Handle(Result images_result)
        {
            Images = new ReactiveList<ImageViewModel>(images_result.Items.Select(i => new ImageViewModel(i)));
        }
    }
}
