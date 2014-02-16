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
        public static Project Empty = new Project();

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

        private int _ImagesFound = 0;
        public int ImagesFound
        {
            get { return _ImagesFound; }
            set { this.RaiseAndSetIfChanged(ref _ImagesFound, value); }
        }

        private ReactiveList<Keyword> _Keywords = new ReactiveList<Keyword>();
        public ReactiveList<Keyword> Keywords
        {
            get { return _Keywords; }
            set { this.RaiseAndSetIfChanged(ref _Keywords, value); }
        }

        private ReactiveList<string> _Extensions = new ReactiveList<string> { ".jpg", ".png", ".bmp" };
        public ReactiveList<string> Extensions
        {
            get { return _Extensions; }
            set { this.RaiseAndSetIfChanged(ref _Extensions, value); }
        }

        private int? _MinWidth = null;
        public int? MinWidth
        {
            get { return _MinWidth; }
            set { this.RaiseAndSetIfChanged(ref _MinWidth, value); }
        }

        private int? _MaxWidth = null;
        public int? MaxWidth
        {
            get { return _MaxWidth; }
            set { this.RaiseAndSetIfChanged(ref _MaxWidth, value); }
        }

        private int? _MinHeight = null;
        public int? MinHeight
        {
            get { return _MinHeight; }
            set { this.RaiseAndSetIfChanged(ref _MinHeight, value); }
        }

        private int? _MaxHeight = null;
        public int? MaxHeight
        {
            get { return _MaxHeight; }
            set { this.RaiseAndSetIfChanged(ref _MaxHeight, value); }
        }

        public bool CanRun()
        {
            return !string.IsNullOrWhiteSpace(Site);
        }
    }
}
