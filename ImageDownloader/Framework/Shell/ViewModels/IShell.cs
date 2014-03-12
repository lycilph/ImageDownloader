using ImageDownloader.Core;

namespace ImageDownloader.Framework.Shell.ViewModels
{
    public interface IShell
    {
        void ShowTool<TTool>();
        void NewContent();
        void CloseContent(IContent content);
        void Close();
    }
}
