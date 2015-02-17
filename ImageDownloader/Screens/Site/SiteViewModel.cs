using System;
using System.Reactive.Linq;
using ImageDownloader.Controllers;
using ReactiveUI;

namespace ImageDownloader.Screens.Site
{
    public sealed class SiteViewModel : BaseViewModel
    {
        private Node _CurrentNode;
        public Node CurrentNode
        {
            get { return _CurrentNode; }
            set { this.RaiseAndSetIfChanged(ref _CurrentNode, value); }
        }

        private ReactiveList<Node> _Nodes;
        public ReactiveList<Node> Nodes
        {
            get { return _Nodes; }
            set { this.RaiseAndSetIfChanged(ref _Nodes, value); }
        }

        private Node _CurrentSelectedNode;
        public Node CurrentSelectedNode
        {
            get { return _CurrentSelectedNode; }
            set { this.RaiseAndSetIfChanged(ref _CurrentSelectedNode, value); }
        }

        private ReactiveList<Node> _SelectedNodes = new ReactiveList<Node>();
        public ReactiveList<Node> SelectedNodes
        {
            get { return _SelectedNodes; }
            set { this.RaiseAndSetIfChanged(ref _SelectedNodes, value); }
        }

        private int _CurrentFocus;
        public int CurrentFocus
        {
            get { return _CurrentFocus; }
            set { this.RaiseAndSetIfChanged(ref _CurrentFocus, value); }
        }

        //private readonly ObservableAsPropertyHelper<bool> _CanNext;
        //public bool CanNext { get { return _CanNext.Value; } }

        public SiteViewModel(ApplicationController controller) : base(controller)
        {
            DisplayName = "Site";
            Option = new SiteOptionViewModel(controller);

            SelectedNodes.CountChanged.Subscribe(x =>
            {
                controller.MainStatusText = (SelectedNodes.Count > 0 ? "Selected files: " + SelectedNodes.Count : string.Empty);
                controller.AuxiliaryStatusText = string.Empty;
            });

            //_CanNext = SelectedNodes.CountChanged.Select(c => c > 0).ToProperty(this, x => x.CanNext);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Nodes = new ReactiveList<Node>
            {
                //new Node(site_controller.Sitemap, string.Empty, null, Node.NodeKind.Page, SelectedNodes)
            };
        }

        public void Delete()
        {
            if (CurrentSelectedNode != null && CurrentFocus == 1)
            {
                CurrentSelectedNode.IsChecked = false;
            }
        }
    }
}
