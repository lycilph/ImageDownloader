using ImageDownloader.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    public class ProjectViewModel : ReactiveObject
    {
        private Project _Model;
        public Project Model
        {
            get { return _Model; }
            set { this.RaiseAndSetIfChanged(ref _Model, value); }
        }

        private bool _IsEditing;
        public bool IsEditing
        {
            get { return _IsEditing; }
            set { this.RaiseAndSetIfChanged(ref _IsEditing, value); }
        }

        public ProjectViewModel(Project project)
        {
            Model = project;
            IsEditing = false;
        }
    }
}
