using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Webscraper
{
    public class StartStep : Step
    {
        public string Website
        {
            get { return settings.Website; }
            set { settings.Website = value; }
        }

        public string OutputDir
        {
            get { return settings.OutputDir; }
            set { settings.OutputDir = value; }
        }

        public string CacheDir
        {
            get { return settings.CacheDir;}
            set { settings.CacheDir = value; }
        }

        public bool UseCache
        {
            get { return settings.UseCache; }
            set { settings.UseCache = value; }
        }

        public ICommand BrowseOutputDirCommand { get; private set; }
        public ICommand BrowseCacheDirCommand { get; private set; }

        public StartStep(Settings settings) : base("Choose Website", settings, null)
        {
            BrowseOutputDirCommand = new RelayCommand(BrowseOutputDir);
            BrowseCacheDirCommand = new RelayCommand(BrowseCacheDir);
        }

        private void BrowseCacheDir(object obj)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            if (string.IsNullOrWhiteSpace(CacheDir))
                fbd.SelectedPath = FileUtils.GetExeDirectory();
            else
                fbd.SelectedPath = CacheDir;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                CacheDir = fbd.SelectedPath;
        }

        private void BrowseOutputDir(object obj)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            if (string.IsNullOrWhiteSpace(OutputDir))
                fbd.SelectedPath = FileUtils.GetExeDirectory();
            else
                fbd.SelectedPath = OutputDir;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OutputDir = fbd.SelectedPath;
        }

        public override bool CanGotoNext()
        {
            return !string.IsNullOrWhiteSpace(Website);
        }
    }
}
