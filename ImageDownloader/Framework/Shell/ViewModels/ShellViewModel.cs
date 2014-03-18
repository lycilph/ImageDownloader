using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Core;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Framework.MainMenu.ViewModels;
using ImageDownloader.Framework.Shell.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace ImageDownloader.Framework.Shell.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveConductor<ILayoutItem>.Collection.OneActive, IShell, IHandle<ShellMessage>
    {
        private readonly IEventAggregator event_aggregator;
        private readonly List<IModule> modules;

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

        private IMenu _MainMenu;
        public IMenu MainMenu
        {
            get { return _MainMenu; }
            set { this.RaiseAndSetIfChanged(ref _MainMenu, value); }
        }

        [ImportingConstructor]
        public ShellViewModel([ImportMany] IEnumerable<ITool> tools, [ImportMany] IEnumerable<Lazy<IModule, OrderMetadata>> modules, IMenu main_menu, IEventAggregator event_aggregator)
        {
            DisplayName = "Shell";

            Tools = new ReactiveList<ITool>(tools);
            Tools.Apply(t => ActivateItem(t));

            MainMenu = main_menu;

            var sorted_modules = modules.OrderBy(m => m.Metadata.Order).Select(m => m.Value);
            this.modules = new List<IModule>(sorted_modules);

            this.event_aggregator = event_aggregator;
            event_aggregator.Subscribe(this);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            InitializeModules();
        }

        private void InitializeModules()
        {
            modules.Apply(m => m.Initialize());
        }

        private void ShowTool(Type tool_type)
        {
            var tool = Tools.FirstOrDefault(t => tool_type.IsAssignableFrom(t.GetType()));
            if (tool != null)
            {
                tool.IsVisible = true;
                ActivateItem(tool);
            }
        }

        private void NewJob()
        {
            var content = IoC.Get<IContent>();
            content.IsSelected = true;

            Content.Add(content);
            ActivateItem(content);
        }

        private void Close(IContent content)
        {
            Content.Remove(content);
        }

        private void CloseCurrent()
        {
            if (ActiveItem is ITool)
            {
                var tool = ActiveItem as ITool;
                tool.IsVisible = false;
            }
            else
            {
                Close(ActiveItem as IContent);
            }
        }

        private void Exit()
        {
            TryClose();
        }

        private void SaveLayout()
        {
            var shell_view = Views.Values.Single() as IShellView;
            shell_view.SaveLayout();
        }

        public void Handle(ShellMessage message)
        {
            switch (message.MessageType)
            {
                case ShellMessage.MessageTypes.SaveLayout:
                    SaveLayout();
                    break;
                case ShellMessage.MessageTypes.Exit:
                    Exit();
                    break;
                case ShellMessage.MessageTypes.NewJob:
                    NewJob();
                    break;
                case ShellMessage.MessageTypes.CloseContent:
                    Close(message.Content);
                    break;
                case ShellMessage.MessageTypes.CloseCurrent:
                    CloseCurrent();
                    break;
                case ShellMessage.MessageTypes.ShowTool:
                    ShowTool(message.ToolType);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
