using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Webscraper
{
    public static class ScrollToEnd
    {
        public static bool GetEnable(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableProperty);
        }
        public static void SetEnable(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableProperty, value);
        }
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached("Enable", typeof(bool), typeof(ScrollToEnd), new UIPropertyMetadata(false, OnEnableChanged));

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox text_box = d as TextBox;
            if (text_box == null) return;

            bool old_val = (bool)e.OldValue;
            bool new_val = (bool)e.NewValue;
            if (old_val == new_val) return;

            if (new_val)
            {
                if (text_box.IsLoaded)
                    text_box.TextChanged += TextChanged;
                else
                    text_box.Loaded += Loaded;
            }
            else
            {
                text_box.TextChanged -= TextChanged;
            }
        }

        private static void Loaded(object sender, RoutedEventArgs e)
        {
            TextBox text_box = sender as TextBox;
            if (text_box == null) return;

            text_box.CaretIndex = text_box.Text.Length;
            var rect = text_box.GetRectFromCharacterIndex(text_box.CaretIndex);
            text_box.ScrollToHorizontalOffset(rect.Right);

            text_box.Loaded -= Loaded;
            text_box.TextChanged += TextChanged;
        }
        
        private static void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text_box = sender as TextBox;
            if (text_box == null) return;

            if (!text_box.IsKeyboardFocused)
            {
                //text_box.Focus();
                //text_box.CaretIndex = text_box.Text.Length;
                //text_box.ScrollToEnd();

                text_box.CaretIndex = text_box.Text.Length;
                var rect = text_box.GetRectFromCharacterIndex(text_box.CaretIndex);
                text_box.ScrollToHorizontalOffset(rect.Right);
            }
        }
    }
}
