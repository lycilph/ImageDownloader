using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Core;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace ImageDownloader.Shell.ViewModels
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

        public ShellViewModel()
        {
            DisplayName = "Shell";
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var content = IoC.Get<IContent>();
            Content.Add(content);

            ActivateItem(content);
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
