namespace ImageDownloader.Interfaces
{
    public interface IStep
    {
        bool IsEnabled { get; }

        bool CanGotoPrevious { get; }

        bool CanGotoNext { get; }
    }
}
