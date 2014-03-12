using Caliburn.Micro;
using ImageDownloader.Framework.Shell.Utils;
using ImageDownloader.Framework.Shell.ViewModels;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ImageDownloader.Framework.Services
{
    [Export(typeof(IInputManager))]
    public class InputManager : IInputManager
    {
        private IShell shell;

        [ImportingConstructor]
        public InputManager(IShell shell)
        {
            this.shell = shell;
        }

        public void SetShortcut(DependencyObject view, InputGesture gesture, object handler)
        {
            var trigger = new InputBindingTrigger();
            trigger.InputBinding = new InputBinding(new RoutedCommand(), gesture);

            Interaction.GetTriggers(view).Add(trigger);

            trigger.Actions.Add(new GestureTriggerAction(handler));
        }

        public void SetShortcut(InputGesture gesture, object handler)
        {
            if (Application.Current.MainWindow == null)
                shell.ViewLoaded += (s, e) => SetShortcut(Application.Current.MainWindow, gesture, handler);
            else
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
