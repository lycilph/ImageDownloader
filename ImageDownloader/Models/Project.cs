using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Models
{
    public class Project : ReactiveObject
    {
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private string _Site = string.Empty;
        public string Site
        {
            get { return _Site; }
            set { this.RaiseAndSetIfChanged(ref _Site, value); }
        }

        private DateTime _Created = DateTime.Now;
        public DateTime Created
        {
            get { return _Created; }
            set { this.RaiseAndSetIfChanged(ref _Created, value); }
        }

        private DateTime _LastExecution = DateTime.Now;
        public DateTime LastExecution
        {
            get { return _LastExecution; }
            set { this.RaiseAndSetIfChanged(ref _LastExecution, value); }
        }

        private int _ExecutionCount = 0;
        public int ExecutionCount
        {
            get { return _ExecutionCount; }
            set { this.RaiseAndSetIfChanged(ref _ExecutionCount, value); }
        }
    }
}
