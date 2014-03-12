using ImageDownloader.Framework.Shell.ViewModels;
using ImageDownloader.Tools.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace ImageDownloader.Framework.MainMenu.ViewModels
{
    [Export(typeof(IMenu))]
    public class MainMenuViewModel : ReactiveList<MenuItemBase>, IMenu
    {
        [Import]
        private IShell shell;

        public MainMenuViewModel()
        {
            AddRange(new List<MenuItemBase>
            {
                new MenuItem("_File")
                {
                    new MenuItem("_New", New),
                    new MenuItem("_Open"),
                    MenuItemBase.Separator,
                    new MenuItem("_Close"),
                    new MenuItem("Close _All"),
                    MenuItemBase.Separator,
                    new MenuItem ("_Recent"), 
                    MenuItemBase.Separator,
                    new MenuItem("E_xit", Exit)
                },
                new MenuItem("_Window")
                {
                    new MenuItem("_Start"),
                    new MenuItem("_Output", ShowOutput),
                    new MenuItem("_Settings"),
                    new MenuItem("Open windows")
                },
                new MenuItem("_Help")
                {
                    new MenuItem("_About")
                }
            });
        }

        public void New()
        {
            shell.NewContent();
        }

        public void ShowOutput()
        {
            shell.ShowTool<IOutput>();
        }

        public void Exit()
        {
            shell.Close();
        }
    }
}
