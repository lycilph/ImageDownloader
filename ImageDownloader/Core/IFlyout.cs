using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace ImageDownloader.Core
{
    public interface IFlyout : IHaveDisplayName
    {
        bool IsOpen { get; set; }
        Position Position { get; set; }
        void Toggle();
    }
}
