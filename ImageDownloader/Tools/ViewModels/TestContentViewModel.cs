using Caliburn.Micro;
using ImageDownloader.Core;
using ImageDownloader.Shell.Views;
using ReactiveUI;
using System.IO;
using System.Reflection;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace ImageDownloader.Tools.ViewModels
{
    public class TestContentViewModel : Content
    {
        private ILog log = LogManager.GetLog(typeof(TestContentViewModel));

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { this.RaiseAndSetIfChanged(ref _Address, value); }
        }

        public TestContentViewModel(string name)
        {
            DisplayName = name;

            Home();
        }

        protected override void OnActivate()
        {
            log.Info("TestContentViewModel.Activated");
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            log.Info("TestContentViewModel.Deactivated");
            base.OnDeactivate(close);
        }

        public void Home()
        {
            Address = "http://www.google.com";
        }

        public void Load()
        {
            log.Info("Loading layout");

            var window = Application.Current.MainWindow as ShellView;
            var serializer = new XmlLayoutSerializer(window.docking_manager);
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Layout.xml");

            if (!File.Exists(path))
                return;

            using (var fs = File.Open(path, FileMode.Open))
            using (var sr = new StreamReader(fs))
            {
                serializer.Deserialize(sr);
            }
        }

        public void Save()
        {
            log.Info("Saving layout");

            var window = Application.Current.MainWindow as ShellView;
            var serializer = new XmlLayoutSerializer(window.docking_manager);
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Layout.xml");

            using (var fs = File.Open(path, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                serializer.Serialize(sw);
            }
        }
    }
}
