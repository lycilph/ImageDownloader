using ReactiveUI;

namespace ImageDownloader.ViewModels
{
    public class FlyoutCommandViewModel : ReactiveObject
    {
        public FlyoutBase Flyout { get; private set; }

        public string Header
        {
            get { return Flyout.Header; }
        }

        public FlyoutCommandViewModel(FlyoutBase flyout)
        {
            Flyout = flyout;
        }

        public void Toggle()
        {
            Flyout.Toggle();
        }
    }
}
