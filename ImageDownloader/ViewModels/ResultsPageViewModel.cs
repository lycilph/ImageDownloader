using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IPage))]
    public class ResultsPageViewModel : ReactiveScreen, IPage
    {
        public PageType Page
        {
            get { return PageType.ShowResults; }
        }

        public ResultsPageViewModel() {}

        public void Edit() { }
    }
}
