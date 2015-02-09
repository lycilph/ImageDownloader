using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core
{
    [DebuggerDisplay("Name = {Name}, Nodes = {Nodes.Count}, Images = {Images.Count}")]
    public class SiteMapNode
    {
        public string Name { get; set; }
        public Dictionary<string, SiteMapNode> Nodes { get; set; }
        public List<string> Files { get; set; }

        public SiteMapNode(string name)
        {
            Name = name;
            Nodes = new Dictionary<string, SiteMapNode>();
            Files = new List<string>();
        }

        public void Add(string path, string item)
        {
            var node_path = path.Replace(Name, "").TrimStart(new[] { '/' });
            var node_path_elements = node_path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (node_path_elements.Count == 1)
            {
                Files.Add(item);
                return;
            }

            var element = node_path_elements.First();
            var node = GetOrCreate(element);
            node.Add(node_path, item);
        }

        private SiteMapNode GetOrCreate(string element)
        {
            if (Nodes.ContainsKey(element))
                return Nodes[element];

            var node = new SiteMapNode(element);
            Nodes.Add(element, node);
            return node;
        }
    }
}
