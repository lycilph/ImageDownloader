using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageDownloader.Controls
{
    public partial class EditableTextControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditableTextControl), new FrameworkPropertyMetadata(string.Empty, OnTextChangedCallback) { BindsTwoWayByDefault = true });

        private string EditedText
        {
            get { return (string)GetValue(EditedTextProperty); }
            set { SetValue(EditedTextProperty, value); }
        }
        public static readonly DependencyProperty EditedTextProperty =
            DependencyProperty.Register("EditedText", typeof(string), typeof(EditableTextControl), new PropertyMetadata(string.Empty));

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }
        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditableTextControl), new FrameworkPropertyMetadata(false) { BindsTwoWayByDefault = true });

        public EditableTextControl()
        {
            InitializeComponent();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                IsEditing = true;
                e.Handled = true;
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    IsEditing = true;
                    e.Handled = true;
                    break;
                case Key.Enter:
                    IsEditing = false;
                    e.Handled = true;
                    Text = EditedText;
                    break;
                case Key.Escape:
                    IsEditing = false;
                    e.Handled = true;
                    EditedText = Text;
                    break;
            }
        }

        public static void OnTextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EditableTextControl;
            if (control == null) return;

            if (control.EditedText != control.Text)
                control.EditedText = control.Text;
        }

        private void OnTextBoxVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox == null) return;

            if ((bool)e.NewValue)
            {
                Keyboard.Focus(textbox);
                textbox.CaretIndex = textbox.Text.Length;
            }
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            Text = EditedText;
        }
    }
}
