using System.ComponentModel.Composition;

namespace ImageDownloader.Contents.Job.ViewModels
{
    [Export(typeof(IJobStep))]
    [ExportMetadata("Order", 3)]
    public class JobStepResultsViewModel : JobStepBase
    {
        public JobStepResultsViewModel()
        {
            DisplayName = "Results";
        }
    }
}
