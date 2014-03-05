namespace ImageDownloader.Core
{
    public interface ITool : ILayoutItem
    {
        PaneLocation DefaultLocation { get; }
        double DefaultWidth { get; }
        double DefaultHeight { get; }
        bool IsVisible { get; set; }
    }
}
