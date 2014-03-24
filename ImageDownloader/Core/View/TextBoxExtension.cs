using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ImageDownloader.Core.View
{
    public static class TextBoxExtension
    {
        public static DependencyProperty GetUpdateOnEnter(DependencyObject obj)
        {
            return (DependencyProperty)obj.GetValue(UpdateOnEnterProperty);
        }
        public static void SetUpdateOnEnter(DependencyObject obj, DependencyProperty value)
        {
            obj.SetValue(UpdateOnEnterProperty, value);
        }
        public static readonly DependencyProperty UpdateOnEnterProperty =
            DependencyProperty.RegisterAttached("UpdateOnEnter", typeof(DependencyProperty), typeof(TextBoxExtension), new PropertyMetadata(null, OnUpdateOnEnterChanged));

        private static void OnUpdateOnEnterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = d as UIElement;
            if (element == null) return;

            if (e.OldValue != null)
                element.PreviewKeyDown -= HandlePreviewKeyDown;

            if (e.NewValue != null)
                element.PreviewKeyDown += HandlePreviewKeyDown;
        }

        private static void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            DependencyProperty property = GetUpdateOnEnter(sender as DependencyObject);
            if (property == null) return;

            UIElement element = sender as UIElement;
            if (element == null) return;

            var binding = BindingOperations.GetBindingExpression(element, property);
            if (binding != null)
                binding.UpdateSource();
        }
    }
}
