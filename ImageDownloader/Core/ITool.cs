using System.Windows.Input;

namespace ImageDownloader.Core
{
    public interface ITool : ILayoutItem
    {
        PaneLocation DefaultLocation { get; }
        double DefaultSize { get; }
        bool CanAutoHide { get; }
        bool IsVisible { get; set; }
        ICommand CloseCommand { get; set; }
    }
}
