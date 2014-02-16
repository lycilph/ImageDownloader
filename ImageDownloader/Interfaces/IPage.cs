namespace ImageDownloader.Interfaces
{
    public enum PageType { ProjectSelection, EditProject, RunProject };

    public interface IPage
    {
        PageType Page { get; }
    }
}
