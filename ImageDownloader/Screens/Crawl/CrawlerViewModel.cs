﻿using ReactiveUI;

namespace ImageDownloader.Screens.Crawl
{
    public class CrawlerViewModel : ReactiveObject
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }
    }
}
