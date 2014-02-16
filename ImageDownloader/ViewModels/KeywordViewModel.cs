using ImageDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.ViewModels
{
    public class KeywordViewModel : ViewModelBase<Keyword>
    {
        public KeywordViewModel(Keyword keyword) : base(keyword) {}
    }
}
