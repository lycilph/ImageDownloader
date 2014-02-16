using ImageDownloader.Interfaces;
using System.ComponentModel.Composition;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IStep))]
    [ExportMetadata("Order", 4)]
    public class ImagesStepViewModel : StepBase
    {
        //public override bool CanGotoNext
        //{
        //    get { return false; }
        //}

        [ImportingConstructor]
        public ImagesStepViewModel() : base("Images") { }

        protected override void OnActivate()
        {
            base.OnActivate();
            IsEnabled = true;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                IsEnabled = false;
        }
    }
}
