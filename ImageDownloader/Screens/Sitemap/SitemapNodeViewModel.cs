using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using NLog;
using ReactiveUI;
using WebCrawler.Sitemap;
using LogManager = NLog.LogManager;

namespace ImageDownloader.Screens.Sitemap
{
    public class SitemapNodeViewModel : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public enum NodeKind { File, Page }

        private readonly SitemapNodeViewModel parent;
        private readonly NodeKind kind;
        private readonly ReactiveList<SitemapNodeViewModel> download_list;
        private readonly string original_node_name;
        private readonly string extension;

        public List<SitemapNodeViewModel> Children { get; private set; }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            private set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

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

        private bool _IsExcluded;
        public bool IsExcluded
        {
            get { return _IsExcluded; }
            set { this.RaiseAndSetIfChanged(ref _IsExcluded, value); }
        }

        public SitemapNodeViewModel(SitemapNode site_map_node, string file, SitemapNodeViewModel parent, NodeKind kind, ReactiveList<SitemapNodeViewModel> download_list)
        {
            this.parent = parent;
            this.download_list = download_list;
            this.kind = kind;

            switch (kind)
            {
                case NodeKind.File:
                {
                    Text = file;
                    // ReSharper disable once PossibleNullReferenceException
                    extension = Path.GetExtension(Text).TrimStart(new[] { '.' }).ToLowerInvariant();
                    Children = new List<SitemapNodeViewModel>();
                    break;
                }
                case NodeKind.Page:
                {
                    var page_nodes = site_map_node.Nodes.Values.Select(n => new SitemapNodeViewModel(n, string.Empty, this, NodeKind.Page, download_list));
                    var file_nodes = site_map_node.Files.Select(f => new SitemapNodeViewModel(null, f, this, NodeKind.File, download_list));
                    Children = page_nodes.Concat(file_nodes).ToList();
                    original_node_name = site_map_node.Name;
                    Text = string.Format("{0} [{1} image(s)]", original_node_name, GetFilesCount());
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException("kind");
            }
        }

        private int GetFilesCount()
        {
            return (kind == NodeKind.File ?
                    (IsExcluded ? 0 : 1) :
                    Children.Sum(n => n.GetFilesCount()));
        }

        private void SetIsChecked(bool? value, bool update_children, bool update_parent)
        {
            if (value == _IsChecked)
                return;
            
            // ReSharper disable once ExplicitCallerInfoArgument
            this.RaiseAndSetIfChanged(ref _IsChecked, value, "IsChecked");

            if (kind == NodeKind.File)
            {
                if (IsChecked == true && IsExcluded == false)
                    download_list.Add(this);
                
                if (IsChecked == false)
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

        public void UpdateExclusions(ReactiveList<string> strings, ReactiveList<string> extensions)
        {
            Children.Apply(c => c.UpdateExclusions(strings, extensions));

            switch (kind)
            {
                case NodeKind.File:
                {
                    IsExcluded = strings.Any(s => Text.ToLowerInvariant().Contains(s)) || extensions.Contains(extension);
                    IsChecked = (IsChecked == true && !IsExcluded);
                    break;
                }
                case NodeKind.Page:
                {
                    IsExcluded = Children.All(c => c.IsExcluded);
                    Text = string.Format("{0} [{1} image(s)]", original_node_name, GetFilesCount());
                    break;
                }
                default:
                    throw new Exception("Unknown NodeKind");
            }
        }
    }
}
