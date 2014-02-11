using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Webscraper
{
    public class MainViewModel : ObservableObject, IApplicationController
    {
        public ObservableCollection<Step> Steps { get; private set; }
        public PageLoader Loader { get; private set; }

        private Step _CurrentStep;
        public Step CurrentStep
        {
            get { return _CurrentStep; }
            set
            {
                if (_CurrentStep == value) return;
                _CurrentStep = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                if (_IsBusy == value) return;
                _IsBusy = value;
                NotifyPropertyChanged();
            }
        }

        private string _BusyText = string.Empty;
        public string BusyText
        {
            get { return _BusyText; }
            set
            {
                if (_BusyText == value) return;
                _BusyText = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ItemViewModel> Pages { get; private set; }
        public ObservableCollection<ItemViewModel> Images { get; private set; }

        public ICommand PreviousCommand { get; private set; }
        public ICommand NextCommand { get; private set; }

        public MainViewModel(Settings settings)
        {
            Loader = new PageLoader(settings);
            Pages = new ObservableCollection<ItemViewModel>();
            Images = new ObservableCollection<ItemViewModel>();

            Steps = new ObservableCollection<Step>
            {
                new StartStep(settings),
                new SiteStep(settings, this),
                new ImagesStep(settings, this),
                new EndStep(settings, this)
            };
            CurrentStep = Steps.First();

            PreviousCommand = new RelayCommand(Previous, _ => CurrentStep.CanGotoPrev());
            NextCommand = new RelayCommand(Next, _ => CurrentStep.CanGotoNext());
        }

        private void Previous(object obj)
        {
            var index = Steps.IndexOf(CurrentStep);
            CurrentStep = Steps[index - 1];
        }

        private void Next(object obj)
        {
            var index = Steps.IndexOf(CurrentStep);
            var step = Steps[index + 1];

            step.Activate();
            CurrentStep = step;
        }

        public void Restart()
        {
            CurrentStep = Steps.First();
        }
    }
}
