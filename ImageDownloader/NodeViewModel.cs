using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using ReactiveUI;

namespace ImageDownloader
{
    public class NodeViewModel : ReactiveObject
    {
        private readonly Node node;
        private readonly NodeViewModel parent;
        public List<NodeViewModel> Children { get; private set; }
        public string Text { get; private set; }
        public List<string> Files { get; private set; }

        private bool? _IsChecked = false;
        public bool? IsChecked
        {
            get { return _IsChecked; }
            set { SetIsChecked(value, true, true); }
        }

        public event EventHandler SelectionChanged;

        public NodeViewModel(Node node) : this(node, null) {}
        public NodeViewModel(Node node, NodeViewModel parent)
        {
            this.parent = parent;
            this.node = node;
            Children = node.Nodes.Values.Select(n => new NodeViewModel(n, this)).ToList();
            Files = node.Files.Concat(Children.SelectMany(n => n.Files)).Distinct().ToList();
            Text = string.Format("{0} [{1} images]", node.Name, Files.Count);

            Children.Apply(c => c.SelectionChanged += OnSelectionChanged);
        }

        public int GetSelectedFilesCount()
        {
            return (IsChecked == true ? Files.Count : Children.Sum(c => c.GetSelectedFilesCount()));
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
