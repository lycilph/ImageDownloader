using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Test.ViewModels;
using ReactiveUI;
using System.ComponentModel.Composition;

namespace ImageDownloader.Shell.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveScreen, IShell
    {
        private ReactiveList<ToolViewModel> _Tools;
        public ReactiveList<ToolViewModel> Tools
        {
            get { return _Tools; }
            set { this.RaiseAndSetIfChanged(ref _Tools, value); }
        }

        public ShellViewModel()
        {
            DisplayName = "Image Downloader";
            Tools = new ReactiveList<ToolViewModel>
            {
                new ToolViewModel("Windows")
            };
        }
    }
}
