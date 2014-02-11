using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Webscraper
{
    public partial class App : Application
    {
        private MainViewModel main_view_model;
        private Settings settings;

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            settings = Settings.Load(Settings.GetSettingsFilename());
            main_view_model = new MainViewModel(settings);
            var main_window = new MainWindow();
            main_window.DataContext = main_view_model;
            main_window.Show();
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Settings.Save(settings, Settings.GetSettingsFilename());
        }
    }
}
