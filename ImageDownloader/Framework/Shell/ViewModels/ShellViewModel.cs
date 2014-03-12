using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Core;
using ImageDownloader.Framework.MainMenu.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace ImageDownloader.Framework.Shell.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveConductor<ILayoutItem>.Collection.OneActive, IShell, IPartImportsSatisfiedNotification
    {
        [ImportMany]
        private ReactiveList<ITool> _Tools;
        public ReactiveList<ITool> Tools
        {
            get { return _Tools; }
            set { this.RaiseAndSetIfChanged(ref _Tools, value); }
        }

        private ReactiveList<IContent> _Content = new ReactiveList<IContent>();
        public ReactiveList<IContent> Content
        {
            get { return _Content; }
            set { this.RaiseAndSetIfChanged(ref _Content, value); }
        }

        [Import]
        private IMenu _MainMenu;
        public IMenu MainMenu
        {
            get { return _MainMenu; }
            set { this.RaiseAndSetIfChanged(ref _MainMenu, value); }
        }

        public event EventHandler ViewLoaded = delegate { };

        public ShellViewModel()
        {
            DisplayName = "Shell";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            NewContent();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ViewLoaded(this, new EventArgs());
        }
        
        public void ShowTool<TTool>()
        {
            var tool = Tools.OfType<TTool>().Cast<ITool>().FirstOrDefault();
            if (tool != null)
            {
                tool.IsVisible = true;
                ActivateItem(tool);
            }
        }

        public void NewContent()
        {
            var content = IoC.Get<IContent>();
            content.IsSelected = true;

            Content.Add(content);
            ActivateItem(content);
        }

        public void CloseContent(IContent content)
        {
            Content.Remove(content);
        }

        public void Close()
        {
            TryClose();
        }

        public void OnImportsSatisfied()
        {
            Items.AddRange(Tools);
        }
    }
}
