using Caliburn.Micro.ReactiveUI;
using ImageDownloader.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace ImageDownloader.ViewModels
{
    [Export(typeof(IStep))]
    [ExportMetadata("Order", 2)]
    public class CriteriaStepViewModel : ReactiveScreen, IStep
    {
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        public bool CanGotoPrevious
        {
            get { return true; }
        }

        public bool CanGotoNext
        {
            get { return false; }
        }

        public CriteriaStepViewModel()
        {
            DisplayName = "Criteria";
            IsEnabled = false;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            IsEnabled = true;
        }
    }
}
