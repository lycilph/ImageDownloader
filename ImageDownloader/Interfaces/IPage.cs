namespace ImageDownloader.Interfaces
{
    public enum PageType { ProjectSelection, EditProject, RunProject, ShowResults };

    public interface IPage
    {
        PageType Page { get; }

        void Edit();
    }
}
