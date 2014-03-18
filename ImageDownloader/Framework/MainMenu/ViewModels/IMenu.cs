using System.Collections.Generic;

namespace ImageDownloader.Framework.MainMenu.ViewModels
{
    public interface IMenu : IEnumerable<MenuItemBase>
    {
        IEnumerable<MenuItemBase> All { get; }
    }
}
