using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Core;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace ImageDownloader.Shell.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : ReactiveConductor<ILayoutItem>.Collection.OneActive, IShell
    {
        private IReactiveDerivedList<ITool> _Tools;
        public IReactiveDerivedList<ITool> Tools
        {
            get { return _Tools; }
            set { this.RaiseAndSetIfChanged(ref _Tools, value); }
        }

        private IReactiveDerivedList<IContent> _Content;
        public IReactiveDerivedList<IContent> Content
        {
            get { return _Content; }
            set { this.RaiseAndSetIfChanged(ref _Content, value); }
        }

        [ImportingConstructor]
        public ShellViewModel([ImportMany] IEnumerable<ILayoutItem> items)
        {
            DisplayName = "Shell";

            Tools = items.CreateDerivedCollection(i => i as ITool, i => i is ITool);
            Content = items.CreateDerivedCollection(i => i as IContent, i => i is IContent);

            Items.AddRange(Tools);
            Items.AddRange(Content);

            ActivateItem(Items.First());
        }
    }
}
