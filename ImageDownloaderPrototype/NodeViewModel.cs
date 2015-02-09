using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Core;

namespace ImageDownloaderPrototype
{
    public class NodeViewModel : INotifyPropertyChanged
    {
        private readonly SiteMapNode site_map_node;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SelectionChanged;

        public NodeViewModel(SiteMapNode site_map_node, NodeViewModel parent)
        {
            this.parent = parent;
            this.site_map_node = site_map_node;
            Children = site_map_node.Nodes.Values.Select(n => new NodeViewModel(n, this)).ToList();
            Files = site_map_node.Files.Concat(Children.SelectMany(n => n.Files)).Distinct().ToList();
            Text = string.Format("{0} [{1} images]", site_map_node.Name, Files.Count);

            Children.Apply(c => c.SelectionChanged += (o, a) => RaiseSelectionChanged());
        }

        public int GetSelectedFilesCount()
        {
            return (IsChecked == true ? Files.Count : Children.Sum(c => c.GetSelectedFilesCount()));
        }

        private void RaiseSelectionChanged()
        {
            var handler = SelectionChanged;
            if (handler != null)
                handler(this, new EventArgs());
        }

        private void RaisePropertyChanged([CallerMemberName] string property_name = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(property_name));
        }

        private void SetIsChecked(bool? value, bool update_children, bool update_parent)
        {
            if (value == _IsChecked)
                return;
            _IsChecked = value;

            if (update_children && IsChecked.HasValue)
                Children.Apply(n => n.SetIsChecked(IsChecked, true, false));

            if (update_parent && parent != null)
                parent.VerifyCheckState();

            RaisePropertyChanged("IsChecked");
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
