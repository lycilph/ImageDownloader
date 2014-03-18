﻿using Caliburn.Micro;
using ImageDownloader.Framework.Services;
using ReactiveUI;
using System.Globalization;
using System.Windows.Input;

namespace ImageDownloader.Framework.MainMenu.ViewModels
{
    public class MenuItem : MenuItemBase
    {
        private System.Action action;
        private KeyGesture key_gesture;

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        public string ActionText { get; private set; }
        
        public override string Name
        {
            get { return string.IsNullOrEmpty(Text) ? null : Text.Replace("_", string.Empty); }
        }

        public string InputGestureText
        {
            get { return key_gesture == null ? string.Empty : key_gesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture); }
        }

        private bool _CanExecute = true;
        public bool CanExecute
        {
            get { return _CanExecute; }
            set { this.RaiseAndSetIfChanged(ref _CanExecute, value); }
        }

        public MenuItem(string text)
        {
            _Text = text;
            ActionText = "Execute";
        }

        public MenuItem(string text, System.Action action) : this(text)
        {
            this.action = action;
        }

        public void Execute()
        {
            if (action != null)
                action();
        }

        public MenuItem WithGlobalShortcut(ModifierKeys modifier, Key key)
        {
            key_gesture = new KeyGesture(key, modifier);
            var input_manager = IoC.Get<IInputManager>();
            input_manager.SetShortcut(key_gesture, this);
            return this;
        }
    }
}
