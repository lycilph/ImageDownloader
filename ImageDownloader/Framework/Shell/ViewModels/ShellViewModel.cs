using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Core;
using ImageDownloader.Contents.Browser.ViewModels;
using ImageDownloader.Core;
using ImageDownloader.Core.Messages;
using ImageDownloader.Framework.MainMenu.ViewModels;
using ImageDownloader.Framework.Shell.Utils;
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
        private readonly IEnumerable<IModule> modules;
        private readonly Dictionary<IContent, ContentWrapper> content_wrappers = new Dictionary<IContent, ContentWrapper>();

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

        private ReactiveList<IWindowCommand> _ShellWindowCommands;
        public ReactiveList<IWindowCommand> ShellWindowCommands
        {
            get { return _ShellWindowCommands; }
            set { this.RaiseAndSetIfChanged(ref _ShellWindowCommands, value); }
        }

        private ReactiveList<IFlyout> _ShellFlyouts;
        public ReactiveList<IFlyout> ShellFlyouts
        {
            get { return _ShellFlyouts; }
            set { this.RaiseAndSetIfChanged(ref _ShellFlyouts, value); }
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

            this.modules = modules.OrderBy(m => m.Metadata.Order).Select(m => m.Value);

            var sorted_commands = commands.OrderBy(c => c.Metadata.Order).Select(c => c.Value);
            ShellWindowCommands = new ReactiveList<IWindowCommand>(sorted_commands);

            ShellFlyouts = new ReactiveList<IFlyout>(flyouts);

            this.event_aggregator = event_aggregator;
            event_aggregator.Subscribe(this);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            modules.Apply(m => m.Initialize());
        }

        public override void ActivateItem(ILayoutItem item)
        {
            base.ActivateItem(item);
            item.IsSelected = true;
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
            var flyout = ShellFlyouts.FirstOrDefault(t => flyout_type.IsAssignableFrom(t.GetType()));
            if (flyout != null)
                flyout.Toggle();
        }

        private void AddContent(IContent content)
        {
            Content.Add(content);
            ActivateItem(content);

            // Add to "opened windows" menu
            var menu_item = new MenuItem(string.Empty, () => ShowContent(content));
            MainMenu.All.First(m => m.Name.ToLower() == "window").Add(menu_item);

            // Add wrapper
            var wrapper = new ContentWrapper(menu_item, content);
            content_wrappers.Add(content, wrapper);
        }

        private void ShowContent(IContent content)
        {
            ActivateItem(content);
        }
        
        private void Close(IContent content)
        {
            Content.Remove(content);
            DeactivateItem(content, true);

            // Remove menu and wrapper
            var wrapper = content_wrappers[content];
            MainMenu.All.First(m => m.Name.ToLower() == "window").Remove(wrapper.MenuItem);
            content_wrappers.Remove(content);
            wrapper.Dispose();
        }

        private void Close(ITool tool)
        {
            tool.IsVisible = false;
            DeactivateItem(tool, false);
        }

        private void CloseCurrent()
        {
            if (ActiveItem is ITool)
                Close(ActiveItem as ITool);
            else
                Close(ActiveItem as IContent);
        }

        private void CloseAll()
        {
            Content.ToList().Apply(c => Close(c));
            Tools.ToList().Apply(t => Close(t));
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
                case ShellMessage.MessageTypes.AddContent:
                    AddContent(message.Content);
                    break;
                case ShellMessage.MessageTypes.ShowContent:
                    ShowContent(message.Content);
                    break;
                case ShellMessage.MessageTypes.CloseContent:
                    Close(message.Content);
                    break;
                case ShellMessage.MessageTypes.CloseCurrent:
                    CloseCurrent();
                    break;
                case ShellMessage.MessageTypes.CloseAll:
                    CloseAll();
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
