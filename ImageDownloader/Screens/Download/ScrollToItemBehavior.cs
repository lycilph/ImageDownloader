using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using ReactiveUI;

namespace ImageDownloader.Screens.Download
{
    public class ScrollToItemBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
            AssociatedObject.Unloaded += AssociatedObjectOnUnloaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObjectOnLoaded;
            AssociatedObject.Unloaded -= AssociatedObjectOnUnloaded;
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routed_event_args)
        {
            var items = AssociatedObject.ItemsSource as ReactiveList<string>;
            if (items == null)
                throw new ArgumentException("ItemsSource must be a ReactiveList<string>");
            items.CollectionChanged += ItemsOnCollectionChanged;
        }

        private void AssociatedObjectOnUnloaded(object sender, RoutedEventArgs routed_event_args)
        {
            var items = AssociatedObject.ItemsSource as ReactiveList<string>;
            if (items == null)
                throw new ArgumentException("ItemsSource must be a ReactiveList<string>");
            items.CollectionChanged -= ItemsOnCollectionChanged;
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var items = AssociatedObject.ItemsSource as ReactiveList<string>;
            if (items == null)
                throw new ArgumentException("ItemsSource must be a ReactiveList<string>");

            var container = AssociatedObject.ItemContainerGenerator.ContainerFromItem(items.Last()) as FrameworkElement;
            if (container != null)
                container.BringIntoView();
        }
    }
}
