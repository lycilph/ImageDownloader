using ImageDownloader.Core;
using System.ComponentModel.Composition;
using ReactiveUI;
using ImageDownloader.Framework.Shell.ViewModels;

namespace ImageDownloader.Modules.ViewModels
{
    [Export(typeof(IContent))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BrowserContentViewModel : Content
    {
        private string home_url = "http://www.google.com";

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { this.RaiseAndSetIfChanged(ref _Address, value); }
        }

        [ImportingConstructor]
        public BrowserContentViewModel(IShell shell) : base(shell)
        {
            DisplayName = "Browser";

            Home();
        }

        public void Home()
        {
            Address = home_url;
        }
    }
}
