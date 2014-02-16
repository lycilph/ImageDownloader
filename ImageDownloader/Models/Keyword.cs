using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Models
{
    public class Keyword : ReactiveObject
    {
        public enum RestrictionType { Include, Exclude };

        private RestrictionType _Type = RestrictionType.Include;
        public RestrictionType Type
        {
            get { return _Type; }
            set { this.RaiseAndSetIfChanged(ref _Type, value); }
        }

        private string _Text = string.Empty;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }
    }
}
