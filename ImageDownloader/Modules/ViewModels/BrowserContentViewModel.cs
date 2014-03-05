using ImageDownloader.Core;
using System.ComponentModel.Composition;

namespace ImageDownloader.Modules.ViewModels
{
    [Export(typeof(ILayoutItem))]
    public class BrowserContentViewModel : Content
    {
        public BrowserContentViewModel()
        {
            DisplayName = "Browser";
        }
    }
}
