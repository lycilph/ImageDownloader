using Caliburn.Micro;
using ReactiveUI;
using System.Collections;
using System.Collections.Generic;

namespace ImageDownloader.Shell.ViewModels
{
    public class MenuItemBase : ReactiveObject, IEnumerable<MenuItemBase>
    {
        public static MenuItemBase Separator
        {
            get { return new MenuItemSeparator(); }
        }

        public ReactiveList<MenuItemBase> Children { get; private set; }

        public MenuItemBase()
        {
            Children = new ReactiveList<MenuItemBase>();
        }

        public void Add(params MenuItemBase[] menuItems)
        {
            menuItems.Apply(Children.Add);
        }

        public IEnumerator<MenuItemBase> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
