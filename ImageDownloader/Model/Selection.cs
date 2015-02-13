namespace ImageDownloader.Model
{
    public class Selection
    {
        public enum SelectionKind { Url, CapturedUrl, File }

        public string Text { get; set; }
        public SelectionKind Kind { get; set; }
        //public List<string> Files { get; set; }

        public Selection(string text, SelectionKind kind)
        {
            Text = text;
            Kind = kind;
            //Files = new List<string>();
        }
    }
}
