using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Webscraper
{
    public class Step : ObservableObject
    {
        protected IApplicationController controller;
        protected Settings settings;

        private string _Header;
        public string Header
        {
            get { return _Header; }
            set
            {
                if (_Header == value) return;
                _Header = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand LoadedCommand { get; private set; }

        public Step(string header, Settings settings, IApplicationController controller)
        {
            Header = header;
            this.controller = controller;
            this.settings = settings;

            settings.PropertyChanged += ForwardPropertyChanged;

            LoadedCommand = new RelayCommand(_ => OnLoaded());
        }

        protected virtual void OnLoaded() {}

        public virtual void Activate() {}

        public virtual bool CanGotoPrev()
        {
            return false;
        }
        public virtual bool CanGotoNext()
        {
            return false;
        }

        private void ForwardPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }
    }
}
