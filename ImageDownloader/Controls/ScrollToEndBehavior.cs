using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ImageDownloader.Controls
{
    public class ScrollToEndBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.TextChanged += OnTextChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.TextChanged -= OnTextChanged;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ScrollToEnd();

            AssociatedObject.Loaded -= OnLoaded;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!AssociatedObject.IsKeyboardFocused)
                ScrollToEnd();
        }

        private void ScrollToEnd()
        {
            AssociatedObject.CaretIndex = AssociatedObject.Text.Length;
            var rect = AssociatedObject.GetRectFromCharacterIndex(AssociatedObject.CaretIndex);
            AssociatedObject.ScrollToHorizontalOffset(rect.Right);
        }
    }
}
