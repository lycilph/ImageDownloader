using ReactiveUI;

namespace ImageDownloader.ViewModels
{
    public class FlyoutCommandViewModel : ViewModelBase<FlyoutBase>
    {
        public string Header
        {
            get { return Model.Header; }
        }

        public FlyoutCommandViewModel(FlyoutBase flyout) : base(flyout) {}

        public void Toggle()
        {
            Model.Toggle();
        }
    }
}
