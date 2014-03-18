using Caliburn.Micro;
using ImageDownloader.Framework.MainMenu.ViewModels;
using System.ComponentModel.Composition;

namespace ImageDownloader.Core
{
    public abstract class ModuleBase : IModule
    {
        [Import]
        protected IMenu main_menu;

        [Import]
        protected IEventAggregator event_aggregator;

        public virtual void Initialize() { }
    }
}
