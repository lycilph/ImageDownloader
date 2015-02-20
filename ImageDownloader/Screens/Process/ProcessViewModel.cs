using System;
using System.ComponentModel.Composition;
using Panda.ApplicationCore;

namespace ImageDownloader.Screens.Process
{
    [Export(typeof(StepScreenBase))]
    [Export(typeof(ProcessViewModel))]
    [ExportOrder(3)]
    public sealed class ProcessViewModel : StepScreenBase
    {
        public override bool CanNext { get; protected set; }

        public override bool CanPrevious
        {
            get { return true; }
            protected set { throw new NotSupportedException(); }
        }

        public ProcessViewModel()
        {
            DisplayName = "Process";
        }
    }
}
