using Caliburn.Micro;
using ImageDownloader.Core.Messages;
using ImageDownloader.Model;
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
        private IJobFactory job_factory;

        public IEnumerable<MenuItemBase> All
        {
            get { return this; }
        }

        [ImportingConstructor]
        public MainMenuViewModel(IEventAggregator event_aggregator, IJobFactory job_factory)
        {
            this.event_aggregator = event_aggregator;
            this.job_factory = job_factory;

            AddRange(new List<MenuItemBase>
            {
                new MenuItem("_File")
                {
                    new MenuItem("_New", CreateJob).WithGlobalShortcut(ModifierKeys.Control, Key.N),
                    new MenuItem("_Open"),
                    MenuItemBase.Separator,
                    new MenuItem("_Close", () => Publish(ShellMessage.CloseCurrent)).WithGlobalShortcut(ModifierKeys.Control, Key.W),
                    new MenuItem("Close _All", () => Publish(ShellMessage.CloseAll)),
                    MenuItemBase.Separator,
                    new MenuItem("_Save").WithGlobalShortcut(ModifierKeys.Control, Key.S),
                    new MenuItem("Save _As"),
                    MenuItemBase.Separator,
                    new MenuItem ("_Recent"),
                    MenuItemBase.Separator,
                    new MenuItem("E_xit", () => Publish(ShellMessage.Exit)).WithGlobalShortcut(ModifierKeys.Alt, Key.F4),
                },
                new MenuItem("_View"),
                new MenuItem("_Window")
                {
                    new MenuItem("_Save layout", () => Publish(ShellMessage.SaveLayout)),
                    new MenuItem("_Load layout"),
                    new MenuItem("_Reset layout"),
                    MenuItemBase.Separator
                },
                new MenuItem("_Help")
            });
        }

        private void CreateJob()
        {
            var job = job_factory.Create();
            Publish(ShellMessage.AddContent(job));
        }
        
        private void Publish(ShellMessage message)
        {
            event_aggregator.PublishOnCurrentThread(message);
        }
    }
}
