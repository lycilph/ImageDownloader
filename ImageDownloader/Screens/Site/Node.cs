using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using ImageDownloader.Sitemap;
using ReactiveUI;

namespace ImageDownloader.Screens.Site
{
    public class Node : ReactiveObject
    {
        public enum NodeKind { File, Page }

        private readonly Node parent;
        private readonly NodeKind kind;
        private readonly ReactiveList<Node> download_list;

        public string Text { get; private set; }
        public List<Node> Children { get; private set; }

        public string Image
        {
            get { return (kind == NodeKind.File ? Text : null); }
        }

        private bool? _IsChecked = false;
        public bool? IsChecked
        {
            get { return _IsChecked; }
            set { SetIsChecked(value, true, true); }
        }

        public Node(SitemapNode site_map_node, string file, Node parent, NodeKind kind, ReactiveList<Node> download_list)
        {
            this.parent = parent;
            this.download_list = download_list;
            this.kind = kind;

            switch (kind)
            {
                case NodeKind.File:
                    {
                        Text = file;
                        Children = new List<Node>();
                        break;
                    }
                case NodeKind.Page:
                    {
                        var page_nodes = site_map_node.Nodes.Values.Select(n => new Node(n, string.Empty, this, NodeKind.Page, download_list));
                        var file_nodes = site_map_node.Files.Select(f => new Node(null, f, this, NodeKind.File, download_list));
                        Children = page_nodes.Concat(file_nodes).ToList();
                        Text = string.Format("{0} [{1} images]", site_map_node.Name, GetFilesCount());
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException("kind");
            }
        }

        private int GetFilesCount()
        {
            return (kind == NodeKind.File ? 1 : Children.Sum(n => n.GetFilesCount()));
        }

        private void SetIsChecked(bool? value, bool update_children, bool update_parent)
        {
            if (value == _IsChecked)
                return;
            
            // ReSharper disable once ExplicitCallerInfoArgument
            this.RaiseAndSetIfChanged(ref _IsChecked, value, "IsChecked");

            if (kind == NodeKind.File)
            {
                if (IsChecked == true)
                    download_list.Add(this);
                else
                    download_list.Remove(this);
            }

            if (update_children && IsChecked.HasValue)
                Children.Apply(n => n.SetIsChecked(IsChecked, true, false));

            if (update_parent && parent != null)
                parent.VerifyCheckState();
        }

        private void VerifyCheckState()
        {
            if (!Children.Any())
                return;

            var state = Children.First().IsChecked;
            if (Children.Skip(1).Any(n => n.IsChecked != state))
                state = null;

            SetIsChecked(state, false, true);
        }
    }
}
