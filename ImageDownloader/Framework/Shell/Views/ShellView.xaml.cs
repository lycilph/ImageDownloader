using System.IO;
using System.Reflection;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using MahApps.Metro.Controls.Dialogs;
using ImageDownloader.Framework.Dialogs;
using MahApps.Metro.Controls;

namespace ImageDownloader.Framework.Shell.Views
{
    public partial class ShellView : IShellView
    {
        public ShellView()
        {
            InitializeComponent();
        }

        public void SaveLayout()
        {
            var serializer = new XmlLayoutSerializer(docking_manager);
            serializer.Serialize(GetFilename());
        }

        private string GetFilename()
        {
            var exe_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(exe_dir, "layout.xml");
        }
    }
}
