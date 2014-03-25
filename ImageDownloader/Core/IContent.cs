using System.ComponentModel;
using System.Windows.Input;

namespace ImageDownloader.Core
{
    public interface IContent : ILayoutItem, INotifyPropertyChanged
    {
        ICommand CloseCommand { get; set; }
    }
}
