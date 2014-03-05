using Caliburn.Micro;
using System;

namespace ImageDownloader.Core
{
    public interface ILayoutItem : IScreen
    {
        Guid Id { get; }
        string ContentId { get; }
        bool IsSelected { get; set; }
    }
}
