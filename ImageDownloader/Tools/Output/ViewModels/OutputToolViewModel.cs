using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ReactiveUI;
using System.ComponentModel.Composition;

namespace ImageDownloader.Tools.Output.ViewModels
{
    [Export(typeof(ITool))]
    [Export(typeof(IOutput))]
    public class OutputToolViewModel : Tool, IOutput, IHandle<OutputMessage>
    {
        private ReactiveList<string> _Messages = new ReactiveList<string>();
        public ReactiveList<string> Messages
        {
            get { return _Messages; }
            set { this.RaiseAndSetIfChanged(ref _Messages, value); }
        }

        public override PaneLocation DefaultLocation
        {
            get { return PaneLocation.Bottom; }
        }

        public override double DefaultSize
        {
            get { return 200; }
        }

        public override bool CanAutoHide
        {
            get { return true; }
        }

        [ImportingConstructor]
        public OutputToolViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = "Output";
            IsVisible = false;

            event_aggregator.Subscribe(this);
        }

        public void Clear()
        {
            Messages.Clear();
        }

        public void Write(string text)
        {
            Messages.Add(text);
        }

        public void Handle(OutputMessage message)
        {
            Write(message.Text);
        }
    }
}
