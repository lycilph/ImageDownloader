using Caliburn.Micro;
using System;
using System.Windows.Input;

namespace ImageDownloader.Core
{
    public interface ILayoutItem : IScreen
    {
        Guid Id { get; }
        string ContentId { get; }
        bool IsSelected { get; set; }
    }
}
