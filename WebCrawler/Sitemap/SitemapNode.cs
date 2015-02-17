using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Panda.Utilities.Extensions;

namespace WebCrawler.Sitemap
{
    [DebuggerDisplay("Name = {Name}, Nodes = {Nodes.Count}, Files = {Files.Count}")]
    public class SitemapNode
    {
        public string Name { get; set; }
        public Dictionary<string, SitemapNode> Nodes { get; set; }
        public List<string> Files { get; set; }

        public SitemapNode(string name)
        {
            Name = name;
            Nodes = new Dictionary<string, SitemapNode>();
            Files = new List<string>();
        }

        public void Add(string path, string item)
        {
            var node_path = path.Replace(Name, "").TrimStart(new[] { '/' });
            var node_path_elements = node_path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (node_path_elements.Count == 1)
            {
                if (!Files.Contains(item))
                    Files.Add(item);
                return;
            }

            var element = node_path_elements.First();
            var node = GetOrCreate(element);
            node.Add(node_path, item);
        }

        private SitemapNode GetOrCreate(string element)
        {
            if (Nodes.ContainsKey(element))
                return Nodes[element];

            var node = new SitemapNode(element);
            Nodes.Add(element, node);
            return node;
        }

        public void Save(string filename)
        {
            JsonExtensions.ZipAndWriteToFile(filename, this);
        }

        public static SitemapNode Load(string filename)
        {
            return JsonExtensions.ReadFromFileAndUnzip<SitemapNode>(filename);
        }
    }
}
