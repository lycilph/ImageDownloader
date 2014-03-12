using System.Windows.Input;
using ReactiveUI;
using System.ComponentModel.Composition;
using ImageDownLoader.Core;
using ImageDownloader.Framework.Shell.ViewModels;

namespace ImageDownloader.Core
{
    public class Content : LayoutItem, IContent
    {
        private IShell shell;

        private ICommand _CloseCommand;
        public ICommand CloseCommand
        {
            get { return _CloseCommand; }
            set { this.RaiseAndSetIfChanged(ref _CloseCommand, value); }
        }

        [ImportingConstructor]
        public Content(IShell shell)
        {
            this.shell = shell;

            CloseCommand = new RelayCommand(obj => Close());
        }

        private void Close()
        {
            shell.CloseContent(this);
        }
    }
}
