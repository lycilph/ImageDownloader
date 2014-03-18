using CefSharp;
using ImageDownloader.Core;
using ImageDownloader.Tools.Help.Utils;
using System.ComponentModel.Composition;

namespace ImageDownloader.Tools.Help.ViewModels
{
    [Export(typeof(ITool))]
    [Export(typeof(IHelp))]
    public class HelpToolViewModel : Tool, IHelp
    {
        public override PaneLocation DefaultLocation
        {
            get { return PaneLocation.Right; }
        }

        public override double DefaultSize
        {
            get { return 250; }
        }

        public override bool CanAutoHide
        {
            get { return true; }
        }

        public HelpToolViewModel()
        {
            DisplayName = "Help";
            IsVisible = false;


            CEF.RegisterScheme("help", new SchemeHandlerFactory());
        }
    }
}
