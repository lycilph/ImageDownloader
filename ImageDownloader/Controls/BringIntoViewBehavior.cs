using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace ImageDownloader.Controls
{
    public class BringIntoViewBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.Unloaded += OnUnloaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.Unloaded -= OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var incc = AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (incc == null) return;

            incc.CollectionChanged += OnCollectionChanged;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var incc = AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (incc == null) return;

            incc.CollectionChanged -= OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int count = AssociatedObject.Items.Count;
                if (count == 0)
                    return;

                var item = AssociatedObject.Items[count - 1];

                AssociatedObject.ScrollIntoView(item);
            }
        }
    }
}
