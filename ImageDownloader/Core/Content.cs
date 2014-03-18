using Caliburn.Micro;
using ImageDownloader.Core.Messages;
using ImageDownLoader.Core;
using ReactiveUI;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace ImageDownloader.Core
{
    public class Content : LayoutItem, IContent
    {
        private IEventAggregator event_aggregator;

        private ICommand _CloseCommand;
        public ICommand CloseCommand
        {
            get { return _CloseCommand; }
            set { this.RaiseAndSetIfChanged(ref _CloseCommand, value); }
        }

        [ImportingConstructor]
        public Content(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;

            CloseCommand = new RelayCommand(obj => Close());
        }

        private void Close()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.CloseContent(this));
        }
    }
}
