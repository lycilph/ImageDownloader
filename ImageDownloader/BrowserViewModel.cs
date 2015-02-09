using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace ImageDownloader
{
    public class BrowserViewModel : ReactiveScreen
    {
        private const string HomeUrl = "www.google.com";

        private readonly ShellViewModel shell;

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { this.RaiseAndSetIfChanged(ref _Address, value); }
        }

        public BrowserViewModel(ShellViewModel shell)
        {
            this.shell = shell;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Address = (string.IsNullOrWhiteSpace(shell.Selection.Name) ? HomeUrl : shell.Selection.Name);
        }

        public void Cancel()
        {
            shell.Back();
        }

        public void Capture()
        {
            shell.Selection = new Selection(Address, Selection.SelectionKind.WebCapture);
            shell.Back();
        }

        public void Home()
        {
            Address = HomeUrl;
        }
    }
}
