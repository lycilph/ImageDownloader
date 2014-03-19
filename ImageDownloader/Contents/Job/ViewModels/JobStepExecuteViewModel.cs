using System.ComponentModel.Composition;

namespace ImageDownloader.Contents.Job.ViewModels
{
    [Export(typeof(IJobStep))]
    [ExportMetadata("Order", 2)]
    public class JobStepExecuteViewModel : JobStepBase
    {
        public JobStepExecuteViewModel()
        {
            DisplayName = "Execute";
        }
    }
}
