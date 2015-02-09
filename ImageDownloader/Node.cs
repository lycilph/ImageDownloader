using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using ReactiveUI;

namespace ImageDownloader
{
    public class Node : ReactiveObject
    {
        private enum NodeKind { File, Page }

        private readonly Node parent;
        private readonly NodeKind kind;

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

        public event EventHandler SelectionChanged;

        public Node(SiteMapNode site_map_node, Node parent)
        {
            this.parent = parent;
            kind = NodeKind.Page;

            var page_nodes = site_map_node.Nodes.Values.Select(n => new Node(n, this));
            var file_nodes = site_map_node.Files.Select(f => new Node(f, this));
            Children = page_nodes.Concat(file_nodes).ToList();

            Text = string.Format("{0} [{1} images]", site_map_node.Name, GetFilesCount());
        }

        public Node(string file, Node parent)
        {
            this.parent = parent;
            kind = NodeKind.File;

            Text = file;
            Children = new List<Node>();
        }

        private int GetFilesCount()
        {
            return (kind == NodeKind.File ? 1 : Children.Sum(n => n.GetFilesCount()));
        }

        public int GetSelectedFilesCount()
        {
            return (kind == NodeKind.File && IsChecked == true ? 1 : Children.Sum(n => n.GetSelectedFilesCount()));
        }

        private void OnSelectionChanged(object sender, EventArgs args)
        {
            RaiseSelectionChanged();
        }

        private void RaiseSelectionChanged()
        {
            var handler = SelectionChanged;
            if (handler != null)
                handler(this, new EventArgs());
        }

        private void SetIsChecked(bool? value, bool update_children, bool update_parent)
        {
            if (value == _IsChecked)
                return;
            
            this.RaiseAndSetIfChanged(ref _IsChecked, value, "IsChecked");

            if (update_children && IsChecked.HasValue)
                Children.Apply(n => n.SetIsChecked(IsChecked, true, false));

            if (update_parent && parent != null)
                parent.VerifyCheckState();

            RaiseSelectionChanged();
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
