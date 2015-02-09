using System.Collections.Generic;

namespace ImageDownloader
{
    public class Selection
    {
        public enum SelectionKind { Web, File }

        public string Name { get; set; }
        public SelectionKind Kind { get; set; }
        public List<string> Files { get; set; }

        public Selection(string name, SelectionKind kind)
        {
            Name = name;
            Kind = kind;
            Files = new List<string>();
        }
    }
}
