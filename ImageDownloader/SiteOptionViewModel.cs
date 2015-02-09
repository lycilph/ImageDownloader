using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace ImageDownloader
{
    public class SiteOptionViewModel : ReactiveScreen
    {
        private readonly SiteViewModel site_view_model;

        private readonly ObservableAsPropertyHelper<bool> _CanSave;
        public bool CanSave { get { return _CanSave.Value; } }

        public SiteOptionViewModel(SiteViewModel site_view_model, ShellViewModel shell)
        {
            this.site_view_model = site_view_model;
            _CanSave = shell.WhenAny(x => x.IsBusy, x => !x.Value)
                            .ToProperty(this, x => x.CanSave);
        }

        public void Save()
        {
            site_view_model.Save();
        }
    }
}
