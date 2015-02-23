using System.ComponentModel.Composition;
using System.Windows.Input;
using AutoMapper;
using Caliburn.Micro;
using ImageDownloader.Controllers;
using ImageDownloader.Data;
using ImageDownloader.Screens.Options;
using ImageDownloader.Screens.Sitemap.Option;
using ImageDownloader.Screens.Start;
using NLog;
using Panda.ApplicationCore;
using LogManager = NLog.LogManager;

namespace ImageDownloader
{
    public class ApplicationTasks
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void ActivateApplicationController()
        {
            logger.Trace("Activating application controller");
            var settings = IoC.Get<Settings>();
            settings.Load();

            var navigation = IoC.Get<NavigationController>();
            navigation.Initialize();
        }

        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void ApplyMessageBinderSpecialValues()
        {
            logger.Trace("Applying MessageBinder SpecialValues");

            MessageBinder.SpecialValues.Add("$pressedkey", (context) =>
            {
                // NOTE: IMPORTANT - you MUST add the dictionary key as lowercase as CM
                // does a ToLower on the param string you add in the action message, in fact ideally
                // all your param messages should be lowercase just in case.
                var key_args = context.EventArgs as KeyEventArgs;

                if (key_args != null)
                    return key_args.Key;

                return null;
            });
        }

        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof (BootstrapperTask))]
        public void RegisterAutomapperMappings()
        {
            logger.Trace("Registering Automapper mappings");
            Mapper.CreateMap<Settings, StartViewModel>()
                  .ForMember(destination => destination.CurrentFavoriteUrl, opt => opt.MapFrom(source => source.LastFavoriteUrl))
                  .ForMember(destination => destination.CurrentFavoriteFile, opt => opt.MapFrom(source => source.LastFavoriteFile));
            Mapper.CreateMap<SiteOptions, OptionsViewModel>()
                  .ReverseMap();
            Mapper.CreateMap<SiteOptions, SitemapOptionViewModel>()
                  .ReverseMap();
        }

        [Export(ApplicationBootstrapper.SHUTDOWN_TASK_NAME, typeof(BootstrapperTask))]
        public void DeactivateApplicationController()
        {
            logger.Trace("Deactivating application controller");
            var settings = IoC.Get<Settings>();
            settings.Save();
        }
    }
}
