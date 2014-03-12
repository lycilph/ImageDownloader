namespace ImageDownloader.Tools.Output.ViewModels
{
    interface IOutput
    {
        void Write(string text);
        void Clear();
    }
}
