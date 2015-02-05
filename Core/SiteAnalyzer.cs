using System.Linq;

namespace Core
{
    public static class SiteAnalyzer
    {
        public static Node CreateSiteMap(Site site)
        {
            var images = site.Pages.Values.SelectMany(p => p.Images);
            var files = site.Pages.Values.SelectMany(p => p.OtherLinks);
            var items = images.Concat(files).Distinct().ToList();

            var node = new Node(site.Url);
            items.Apply(i => node.Add(i, i));
            return node;
        }
    }
}
