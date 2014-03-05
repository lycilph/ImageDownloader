using Caliburn.Micro;
using $rootnamespace$.Core;
using $rootnamespace$.Core.Messages;
using ReactiveUI;
using System.ComponentModel.Composition;

namespace $rootnamespace$.Tools.ViewModels
{
    [Export(typeof(ILayoutItem))]
    public class OutputToolViewModel : Tool, IHandle<OutputMessage>
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

        public override double DefaultWidth
        {
            get { return 200; }
        }

        public override double DefaultHeight
        {
            get { return 200; }
        }

        [ImportingConstructor]
        public OutputToolViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = "Log";

            event_aggregator.Subscribe(this);
        }

        public void Clear()
        {
            Messages.Clear();
        }

        public void Handle(OutputMessage message)
        {
            Messages.Add(message.Text);
        }
    }
}
