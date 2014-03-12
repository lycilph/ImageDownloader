using Caliburn.Micro;
using ImageDownloader.Framework.Shell.Utils;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ImageDownloader.Framework.Services
{
    [Export(typeof(IInputManager))]
    public class InputManager : IInputManager
    {
        public void SetShortcut(DependencyObject view, InputGesture gesture, object handler)
        {
            var trigger = new InputBindingTrigger();
            trigger.InputBinding = new InputBinding(new RoutedCommand(), gesture);

            Interaction.GetTriggers(view).Add(trigger);

            trigger.Actions.Add(new GestureTriggerAction(handler));
        }

        public void SetShortcut(InputGesture gesture, object handler)
        {
            SetShortcut(Application.Current.MainWindow, gesture, handler);
        }

        private class GestureTriggerAction : TriggerAction<FrameworkElement>
        {
            private readonly object _handler;

            public GestureTriggerAction(object handler)
            {
                _handler = handler;
            }

            protected override void Invoke(object parameter)
            {
                Action.Invoke(_handler, "Execute");
            }
        }
    }
}
