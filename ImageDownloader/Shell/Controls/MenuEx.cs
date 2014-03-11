using System.Windows;

namespace ImageDownloader.Shell.Controls
{
    public class MenuEx : System.Windows.Controls.Menu
    {
        private object _currentItem;

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            _currentItem = item;
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return MenuItemEx.GetContainer(this, _currentItem);
        }
    }
}
