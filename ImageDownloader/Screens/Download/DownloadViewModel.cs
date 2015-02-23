using System;
using System.ComponentModel.Composition;
using ImageDownloader.Screens.Download.Option;
using Panda.ApplicationCore;

namespace ImageDownloader.Screens.Download
{
    [Export(typeof(StepScreenBase))]
    [Export(typeof(DownloadViewModel))]
    [ExportOrder(5)]
    public sealed class DownloadViewModel : StepScreenBase
    {
        public override bool CanNext
        {
            get { return false; }
            protected set { throw new NotSupportedException(); }
        }

        public override bool CanPrevious
        {
            get { return true; }
            protected set { throw new NotSupportedException(); }
        }

        [ImportingConstructor]
        public DownloadViewModel(DownloadOptionViewModel option_view_model)
        {
            DisplayName = "Download";
            Option = option_view_model;
        }
    }
}
