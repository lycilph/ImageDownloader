using ImageDownloader.Interfaces;
using System.ComponentModel.Composition;

namespace ImageDownloader.Utils
{
    [Export(typeof(ICache))]
    public class Cache : ICache
    {
    }
}
