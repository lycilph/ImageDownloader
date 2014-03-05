using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using System;

namespace ImageDownloader.Core
{
    public class LayoutItem : ReactiveScreen, ILayoutItem
    {
        private readonly Guid id = Guid.NewGuid();

        public Guid Id
        {
            get { return id; }
        }

        public string ContentId
        {
            get { return id.ToString(); }
        }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsSelected, value); }
        }
    }
}
