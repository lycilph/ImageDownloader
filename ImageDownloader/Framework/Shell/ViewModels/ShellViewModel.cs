using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Core;
using ImageDownloader.Contents.Job.ViewModels;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Framework.Commands;
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

        private ReactiveList<IWindowCommand> _WindowCommands;
        public ReactiveList<IWindowCommand> WindowCommands
        {
            get { return _WindowCommands; }
            set { this.RaiseAndSetIfChanged(ref _WindowCommands, value); }
        }

        private ReactiveList<IFlyout> _Flyouts;
        public ReactiveList<IFlyout> Flyouts
        {
            get { return _Flyouts; }
            set { this.RaiseAndSetIfChanged(ref _Flyouts, value); }
        }

        private IMenu _MainMenu;
        public IMenu MainMenu
        {
            get { return _MainMenu; }
            set { this.RaiseAndSetIfChanged(ref _MainMenu, value); }
        }

        [ImportingConstructor]
        public ShellViewModel([ImportMany] IEnumerable<ITool> tools,
                              [ImportMany] IEnumerable<Lazy<IModule, OrderMetadata>> modules,
                              [ImportMany] IEnumerable<Lazy<IWindowCommand, OrderMetadata>> commands,
                              [ImportMany] IEnumerable<IFlyout> flyouts,
                              IMenu main_menu,
                              IEventAggregator event_aggregator)
        {
            DisplayName = "Shell";

            Tools = new ReactiveList<ITool>(tools);
            Tools.Apply(t => ActivateItem(t));

            MainMenu = main_menu;

            var sorted_modules = modules.OrderBy(m => m.Metadata.Order).Select(m => m.Value);
            this.modules = new List<IModule>(sorted_modules);

            var sorted_commands = commands.OrderBy(c => c.Metadata.Order).Select(c => c.Value);
            WindowCommands = new ReactiveList<IWindowCommand>(sorted_commands);

            Flyouts = new ReactiveList<IFlyout>(flyouts);

            this.event_aggregator = event_aggregator;
            event_aggregator.Subscribe(this);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            InitializeModules();
        }

        public override void ActivateItem(ILayoutItem item)
        {
            base.ActivateItem(item);
            item.IsSelected = true;
        }

        private void InitializeModules()
        {
            modules.Apply(m => m.Initialize());
        }

        private void ToggleTool(Type tool_type)
        {
            var tool = Tools.FirstOrDefault(t => tool_type.IsAssignableFrom(t.GetType()));
            if (tool == null) return;

            if (tool.IsVisible)
                DeactivateItem(tool, false);
            else
                ActivateItem(tool);

            tool.IsVisible = !tool.IsVisible;
        }

        private void ToggleFlyout(Type flyout_type)
        {
            var flyout = Flyouts.FirstOrDefault(t => flyout_type.IsAssignableFrom(t.GetType()));
            if (flyout != null)
                flyout.Toggle();
        }

        private void NewJob()
        {
            var job = IoC.Get<IJob>() as IContent;
            Content.Add(job);

            ActivateItem(job);
        }

        private void Close(IContent content)
        {
            Content.Remove(content);
            DeactivateItem(content, true);
        }

        private void CloseCurrent()
        {
            if (ActiveItem is ITool)
            {
                var tool = ActiveItem as ITool;
                tool.IsVisible = false;
                DeactivateItem(tool, false);
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
                case ShellMessage.MessageTypes.ToggleTool:
                    ToggleTool(message.PayloadType);
                    break;
                case ShellMessage.MessageTypes.ToggleFlyout:
                    ToggleFlyout(message.PayloadType);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
