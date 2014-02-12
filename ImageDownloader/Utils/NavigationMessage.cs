using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Utils
{
    public class NavigationMessage
    {
        public enum Page { ProjectSelection, ProjectPage };

        public Page NavigateToPage { get; set; }

        public NavigationMessage(Page page)
        {
            NavigateToPage = page;
        }

        public static NavigationMessage NavigateToProjectPage()
        {
            return new NavigationMessage(Page.ProjectPage);
        }

        public static NavigationMessage NavigateToProjectSelectionPage()
        {
            return new NavigationMessage(Page.ProjectSelection);
        }
    }
}
