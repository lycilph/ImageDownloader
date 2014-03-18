using Caliburn.Micro;
using ImageDownloader.Core.Messages;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace ImageDownloader.Framework.MainMenu.ViewModels
{
    [Export(typeof(IMenu))]
    public class MainMenuViewModel : ReactiveList<MenuItemBase>, IMenu
    {
        private IEventAggregator event_aggregator;
        private IWindowManager window_manager;

        public IEnumerable<MenuItemBase> All
        {
            get { return this; }
        }

        [ImportingConstructor]
        public MainMenuViewModel(IEventAggregator event_aggregator, IWindowManager window_manager)
        {
            this.event_aggregator = event_aggregator;
            this.window_manager = window_manager;

            AddRange(new List<MenuItemBase>
            {
                new MenuItem("_File")
                {
                    new MenuItem("_New", NewJob).WithGlobalShortcut(ModifierKeys.Control, Key.N),
                    new MenuItem("_Open"),
                    MenuItemBase.Separator,
                    new MenuItem("_Close", CloseCurrent).WithGlobalShortcut(ModifierKeys.Control, Key.W),
                    new MenuItem("Close _All"),
                    MenuItemBase.Separator,
                    new MenuItem("_Save").WithGlobalShortcut(ModifierKeys.Control, Key.S),
                    new MenuItem("Save _As"),
                    MenuItemBase.Separator,
                    new MenuItem ("_Recent"), 
                    MenuItemBase.Separator,
                    new MenuItem("E_xit", Exit).WithGlobalShortcut(ModifierKeys.Alt, Key.F4),
                },
                new MenuItem("_Window")
                /*{
                    new MenuItem("_Settings"),
                    MenuItemBase.Separator,
                    new MenuItem("[Opened windows]")
                }*/,
                new MenuItem("_Debug")
                {
                    new MenuItem("_Save layout", SaveLayout)
                },
                new MenuItem("_Help")
            });
        }

        private void SaveLayout()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.SaveLayout);
        }

        private void NewJob()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.NewJob);
        }

        private void CloseCurrent()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.CloseCurrent);
        }

        private void Exit()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.Exit);
        }
    }
}
