using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader.Utils
{
    public interface OrderMetadata
    {
        [DefaultValue(int.MaxValue)]
        int Order { get; }
    }
}
