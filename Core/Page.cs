using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Core
{
    [DebuggerDisplay("Url = {Url}, PageLinks = {PageLinks.Count}, OtherLinks = {OtherLinks.Count}, Images = {Images.Count}")]
    public class Page
    {
        public string Url { get; set; }
        public List<string> PageLinks { get; set; }
        public List<string> OtherLinks { get; set; }
        public List<string> Images { get; set; }

        public Page(string url)
        {
            Url = url;
            PageLinks = new List<string>();
            OtherLinks = new List<string>();
            Images = new List<string>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Page url: " + Url);
            PageLinks.Apply(l => sb.AppendLine("PageLink url: " + l));
            OtherLinks.Apply(l => sb.AppendLine("OtherLink url: " + l));
            Images.Apply(l => sb.AppendLine("Image url: " + l));
            return sb.ToString();
        }
    }
}
