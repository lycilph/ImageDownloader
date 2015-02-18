using System.Windows;
using AutoMapper;
using ImageDownloader.Model;
using ImageDownloader.Screens.Option;

namespace ImageDownloader
{
    public partial class App
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            Mapper.CreateMap<SiteOptions, OptionViewModel>();
            Mapper.CreateMap<OptionViewModel, SiteOptions>();
        }
    }
}
