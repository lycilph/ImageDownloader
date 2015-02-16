using System.Windows;
using System.Windows.Input;

namespace ImageDownloader.Screens.Site
{
    public static class VirtualToggleButton
    {
        public static bool? GetIsChecked(DependencyObject obj)
        {
            return (bool?)obj.GetValue(IsCheckedProperty);
        }
        public static void SetIsChecked(DependencyObject obj, bool? value)
        {
            obj.SetValue(IsCheckedProperty, value);
        }
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.RegisterAttached("IsChecked", typeof(bool?), typeof(VirtualToggleButton), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        public static bool GetIsVirtualToggleButton(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsVirtualToggleButtonProperty);
        }
        public static void SetIsVirtualToggleButton(DependencyObject obj, bool value)
        {
            obj.SetValue(IsVirtualToggleButtonProperty, value);
        }
        public static readonly DependencyProperty IsVirtualToggleButtonProperty =
            DependencyProperty.RegisterAttached("IsVirtualToggleButton", typeof(bool), typeof(VirtualToggleButton), new PropertyMetadata(false, IsVirtualToggleButtonChanged));

        private static void IsVirtualToggleButtonChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var element = obj as IInputElement;
            if (element == null) 
                return;

            if ((bool)args.NewValue)
            {
                element.MouseLeftButtonDown += OnMouseLeftButtonDown;
                element.KeyDown += OnKeyDown;
            }
            else
            {
                element.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                element.KeyDown -= OnKeyDown;
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as DependencyObject;
            SetIsChecked(obj, GetIsChecked(obj) != true);
            e.Handled = true;
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource != sender) 
                return;

            var obj = sender as DependencyObject;
            var accepts_return = (obj != null && (bool) obj.GetValue(KeyboardNavigation.AcceptsReturnProperty));

            if (e.Key == Key.Space)
            {
                // ignore alt+space which invokes the system menu
                if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) 
                    return;

                SetIsChecked(obj, GetIsChecked(obj) != true);
                e.Handled = true;

            }
            else if (e.Key == Key.Enter && accepts_return)
            {
                SetIsChecked(obj, GetIsChecked(obj) != true);
                e.Handled = true;
            }
        }
    }
}
