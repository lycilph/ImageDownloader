using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ImageDownloader.Controls
{
    public class UnselectableBehavior : Behavior<ListBox>
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
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssociatedObject.SelectedItem = null;
        }
    }
}
