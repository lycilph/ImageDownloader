using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Model
{
    public interface ICache
    {
        string Get(string url);
    }
}
