using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace ImageDownloader.Framework.Dialogs.About
{
    public partial class AboutDialog
    {
        public ObservableCollection<Package> Packages
        {
            get { return (ObservableCollection<Package>)GetValue(PackagesProperty); }
            set { SetValue(PackagesProperty, value); }
        }
        public static readonly DependencyProperty PackagesProperty =
            DependencyProperty.Register("Packages", typeof(ObservableCollection<Package>), typeof(AboutDialog), new PropertyMetadata(null));

        public AboutDialog(MetroWindow parent, MetroDialogSettings settings) : base(parent, settings)
        {
            InitializeComponent();
            DataContext = this;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ImageDownloader.packages.config"))
            {
                var doc = new XmlDocument();
                doc.Load(stream);

                var nodes = doc.SelectNodes("//package");
                var packages = nodes.OfType<XmlNode>().Select(n => new Package { Name = n.Attributes["id"].Value, Version = n.Attributes["version"].Value });
                Packages = new ObservableCollection<Package>(packages);
            }
        }
    }
}
