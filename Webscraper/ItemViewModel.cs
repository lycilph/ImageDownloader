using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper
{
    public class ItemViewModel : ObservableObject
    {
        private bool _IsSelected = false;
        [JsonIgnore]
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected == value) return;
                _IsSelected = value;
                NotifyPropertyChanged();
            }
        }

        private string _DisplayText = string.Empty;
        [JsonIgnore]
        public string DisplayText
        {
            get { return _DisplayText; }
            set
            {
                if (_DisplayText == value) return;
                _DisplayText = value;
                NotifyPropertyChanged();
            }
        }

        private string _Text = string.Empty;
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text == value) return;
                _Text = value;
                NotifyPropertyChanged();
            }
        }

        public ItemViewModel() : this(string.Empty, string.Empty) {}
        public ItemViewModel(string text) : this(text, string.Empty) {}
        public ItemViewModel(string text, string display_text)
        {
            Text = text;
            DisplayText = display_text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
