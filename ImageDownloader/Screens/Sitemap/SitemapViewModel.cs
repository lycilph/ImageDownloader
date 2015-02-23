using System;
using System.ComponentModel.Composition;
using System.Linq;
using ImageDownloader.Controllers;
using ImageDownloader.Screens.Sitemap.Option;
using Panda.ApplicationCore;
using ReactiveUI;

namespace ImageDownloader.Screens.Sitemap
{
    [Export(typeof(StepScreenBase))]
    [Export(typeof(SitemapViewModel))]
    [ExportOrder(4)]
    public sealed class SitemapViewModel : StepScreenBase
    {
        private readonly SiteController site_controller;
        private readonly SitemapOptionViewModel option_view_model;

        private SitemapNodeViewModel _CurrentNode;
        public SitemapNodeViewModel CurrentNode
        {
            get { return _CurrentNode; }
            set { this.RaiseAndSetIfChanged(ref _CurrentNode, value); }
        }

        private ReactiveList<SitemapNodeViewModel> _Nodes;
        public ReactiveList<SitemapNodeViewModel> Nodes
        {
            get { return _Nodes; }
            set { this.RaiseAndSetIfChanged(ref _Nodes, value); }
        }

        private SitemapNodeViewModel _CurrentSelectedNode;
        public SitemapNodeViewModel CurrentSelectedNode
        {
            get { return _CurrentSelectedNode; }
            set { this.RaiseAndSetIfChanged(ref _CurrentSelectedNode, value); }
        }

        private ReactiveList<SitemapNodeViewModel> _SelectedNodes = new ReactiveList<SitemapNodeViewModel>();
        public ReactiveList<SitemapNodeViewModel> SelectedNodes
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

        private bool _CanNext;
        public override bool CanNext
        {
            get { return _CanNext; }
            protected set { this.RaiseAndSetIfChanged(ref _CanNext, value); }
        }

        public override bool CanPrevious
        {
            get { return true; }
            protected set { throw new NotSupportedException(); }
        }

        [ImportingConstructor]
        public SitemapViewModel(SiteController site_controller, StatusController status_controller, SitemapOptionViewModel option_view_model)
        {
            DisplayName = "Sitemap";
            this.site_controller = site_controller;
            this.option_view_model = option_view_model;

            Option = option_view_model;

            SelectedNodes.CountChanged.Subscribe(x =>
            {
                status_controller.MainStatusText = (SelectedNodes.Count > 0 ? "Selected files: " + SelectedNodes.Count : string.Empty);
                status_controller.AuxiliaryStatusText = string.Empty;
                CanNext = SelectedNodes.Count > 0;
            });
        }

        protected override async void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Option.Deactivate(close);

            site_controller.SelectedFiles = SelectedNodes.Select(n => n.Text).ToList();
            await site_controller.UpdateSiteOptions();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            SelectedNodes.Clear();
            Nodes = new ReactiveList<SitemapNodeViewModel>
            {
                new SitemapNodeViewModel(site_controller.Sitemap, string.Empty, null, SitemapNodeViewModel.NodeKind.Page, SelectedNodes)
            };
            option_view_model.SetRootNode(Nodes.First());
            Option.Activate();
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
