using ImageDownloader.Controllers;
using Microsoft.Win32;
using ReactiveUI;

namespace ImageDownloader.Screens.Site
{
    public class SiteOptionViewModel : BaseViewModel
    {
        private readonly ObservableAsPropertyHelper<bool> _CanSave;
        public bool CanSave { get { return _CanSave.Value; } }

        public SiteOptionViewModel(ApplicationController controller) : base(controller)
        {
            _CanSave = controller.WhenAny(x => x.IsBusy, x => !x.Value)
                                 .ToProperty(this, x => x.CanSave);
        }

        public void Save()
        {
            var save_file_dialog = new SaveFileDialog
            {
                InitialDirectory = controller.Settings.DataFolder,
                DefaultExt = ".site",
                Filter = "Site file (.site)|*.site"
            };

            if (save_file_dialog.ShowDialog() == true)
            {
                //site_controller.Save(save_file_dialog.FileName);
                //controller.MainStatusText = string.Format("Saved {0} to {1}", site_controller.Url, save_file_dialog.FileName);
            }
        }
    }
}
