using ImageDownloader.Core;
using System;

namespace ImageDownloader.Framework.Shell.ViewModels
{
    public interface IShell
    {
        event EventHandler ViewLoaded;

        void ShowTool<TTool>();
        void NewContent();
        void CloseContent(IContent content);
        void Close();
    }
}
