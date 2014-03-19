using System.ComponentModel.Composition;

namespace ImageDownloader.Contents.Job.ViewModels
{
    [Export(typeof(IJobStep))]
    [ExportMetadata("Order", 1)]
    public class JobStepOptionsViewModel : JobStepBase
    {
        public JobStepOptionsViewModel()
        {
            DisplayName = "Options";
        }
    }
}
