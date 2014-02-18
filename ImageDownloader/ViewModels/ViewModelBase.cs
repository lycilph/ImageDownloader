using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    public class ViewModelBase<T> : ReactiveObject where T : INotifyPropertyChanged
    {
        private T _Model;
        public T Model
        {
            get { return _Model; }
            set { this.RaiseAndSetIfChanged(ref _Model, value); }
        }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsSelected, value); }
        }

        private bool _IsEditing = false;
        public bool IsEditing
        {
            get { return _IsEditing; }
            set { this.RaiseAndSetIfChanged(ref _IsEditing, value); }
        }

        public ViewModelBase(T model)
        {
            _Model = model;

            // List for property changed events on the model and forward them (using a weak event handler to avoid memory leaks)
            PropertyChangedEventManager.AddHandler(model, ForwardPropertyNotifications, string.Empty);
        }

        private void ForwardPropertyNotifications(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
        }
    }
}
