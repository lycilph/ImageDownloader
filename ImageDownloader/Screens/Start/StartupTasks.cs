using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using Panda.ApplicationCore;

namespace ImageDownloader.Screens.Start
{
    public class StartupTasks
    {
        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        [ExportMetadata("Order", 0)]
        public void ApplyMessageBinderSpecialValues()
        {
            MessageBinder.SpecialValues.Add("$pressedkey", (context) =>
            {
                // NOTE: IMPORTANT - you MUST add the dictionary key as lowercase as CM
                // does a ToLower on the param string you add in the action message, in fact ideally
                // all your param messages should be lowercase just in case. I don't really like this
                // behaviour but that's how it is!
                var key_args = context.EventArgs as KeyEventArgs;

                if (key_args != null)
                    return key_args.Key;

                return null;
            });
        }
    }
}
