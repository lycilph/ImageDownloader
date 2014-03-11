using System.Windows.Input;

namespace ImageDownloader.Core
{
    public interface IContent : ILayoutItem
    {
        ICommand CloseCommand { get; set; }
    }
}
