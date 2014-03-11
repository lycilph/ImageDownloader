using System.Windows.Input;
using ReactiveUI;
using System.ComponentModel.Composition;
using ImageDownloader.Shell.ViewModels;
using ImageDownLoader.Core;

namespace ImageDownloader.Core
{
    public class Content : LayoutItem, IContent
    {
        private ICommand _CloseCommand;
        public ICommand CloseCommand
        {
            get { return _CloseCommand; }
            set { this.RaiseAndSetIfChanged(ref _CloseCommand, value); }
        }

        [ImportingConstructor]
        public Content(IShell shell)
        {
            CloseCommand = new RelayCommand(obj => Close());
        }

        private void Close()
        {
            System.Diagnostics.Debug.Print("closing");
        }
    }
}
