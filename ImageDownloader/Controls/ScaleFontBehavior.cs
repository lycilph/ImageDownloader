using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace ImageDownloader.Controls
{
    // http://stackoverflow.com/questions/15641473/how-to-automatically-scale-font-size-for-a-group-of-controls
    public class ScaleFontBehavior : Behavior<TextBlock>
    {
        public double MaxFontSize
        {
            get { return (double)GetValue(MaxFontSizeProperty); }
            set { SetValue(MaxFontSizeProperty, value); }
        }
        public static readonly DependencyProperty MaxFontSizeProperty =
            DependencyProperty.Register("MaxFontSize", typeof(double), typeof(ScaleFontBehavior), new PropertyMetadata(20.0));

        public double MinFontSize
        {
            get { return (double)GetValue(MinFontSizeProperty); }
            set { SetValue(MinFontSizeProperty, value); }
        }
        public static readonly DependencyProperty MinFontSizeProperty =
            DependencyProperty.Register("MinFontSize", typeof(double), typeof(ScaleFontBehavior), new PropertyMetadata(10.0));

        protected override void OnAttached()
        {
            var parent = VisualTreeHelper.GetParent(AssociatedObject) as FrameworkElement;
            if (parent == null) return;

            parent.SizeChanged += CalculateFontSize;
        }

        protected override void OnDetaching()
        {
            var parent = VisualTreeHelper.GetParent(AssociatedObject) as FrameworkElement;
            if (parent == null) return;

            parent.SizeChanged -= CalculateFontSize;
        }

        private void CalculateFontSize(object sender, SizeChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AssociatedObject.Text))
                return;

            var parent = VisualTreeHelper.GetParent(AssociatedObject) as FrameworkElement;
            if (parent == null) return;

            var font_size = MaxFontSize;

            var desired_size = MeasureText(AssociatedObject);

            var margin = AssociatedObject.Margin;
            var margin_width = margin.Left + margin.Right;
            var margin_height = margin.Top + margin.Bottom;

            var desired_width = desired_size.Width + margin_width;
            var desired_height = desired_size.Height + margin_height;

            if (parent.ActualWidth < desired_width)
            {
                var factor = (parent.ActualWidth - margin_width) / (desired_width * 1.05 - margin_width);
                font_size = Math.Min(font_size, MaxFontSize * factor);
            }

            font_size = Math.Max(font_size, MinFontSize);

            AssociatedObject.FontSize = font_size;
        }

        // Measures text size of textblock
        private Size MeasureText(TextBlock tb)
        {
            var formatted_text = new FormattedText(tb.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                this.MaxFontSize, Brushes.Black); // always uses MaxFontSize for desiredSize

            return new Size(formatted_text.Width, formatted_text.Height);
        }
    }
}
