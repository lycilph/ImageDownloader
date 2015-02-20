using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace WebCrawler.Extensions
{
    public static class HtmlExtensions
    {
        public static HtmlDocument Load(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        public static List<string> GetLinks(this HtmlDocument doc, string element, string attribute)
        {
            var xpath = string.Format("//{0}[@{1}]", element, attribute);
            var nodes = doc.DocumentNode.SelectNodes(xpath);
            return (nodes == null ?
                new List<string>() :
                new List<string>(nodes.Select(n => n.GetAttributeValue(attribute, string.Empty))));
        }

        //public static List<string> GetAnchorLinks(this HtmlDocument doc)
        //{
        //    var nodes = doc.DocumentNode.SelectNodes("//a[@href]");
        //    return (nodes == null ?
        //        new List<string>() :
        //        new List<string>(nodes.Select(n => n.GetAttributeValue("href", string.Empty))));
        //}

        //public static List<string> GetFrameLinks(this HtmlDocument doc)
        //{
        //    var nodes = doc.DocumentNode.SelectNodes("//frame[@src]");
        //    return (nodes == null ?
        //        new List<string>() :
        //        new List<string>(nodes.Select(n => n.GetAttributeValue("src", string.Empty))));
        //}

        //public static List<string> GetImageLinks(this HtmlDocument doc)
        //{
        //    var nodes = doc.DocumentNode.SelectNodes("//img[@src]");
        //    return (nodes == null ?
        //        new List<string>() :
        //        new List<string>(nodes.Select(n => n.GetAttributeValue("src", string.Empty))));
        //}
    }
}
