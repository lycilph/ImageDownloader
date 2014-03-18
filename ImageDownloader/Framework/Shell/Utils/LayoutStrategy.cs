using ImageDownloader.Core;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace ImageDownloader.Framework.Shell.Utils
{
    public class LayoutStrategy : ILayoutUpdateStrategy
    {
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorable_to_show, ILayoutContainer destination_container)
        {
            LayoutAnchorablePane destination = destination_container as LayoutAnchorablePane;
            if (destination != null && destination.FindParent<LayoutFloatingWindow>() != null)
                return false;

            // If parent != null, this has already been handled
            if (anchorable_to_show.Parent != null)
                return false;

            var tool = anchorable_to_show.Content as ITool;
            if (tool == null)
                return false;

            anchorable_to_show.CanAutoHide = tool.CanAutoHide;

            if (tool.DefaultLocation == PaneLocation.Content)
            {
                AddToDocumentPane(layout, anchorable_to_show);
                return true;
            }

            var anchor_group = GetAnchorGroup(layout, tool.DefaultLocation);
            anchor_group.Children.Add(anchorable_to_show);

            anchorable_to_show.AutoHideMinWidth = tool.DefaultSize;
            anchorable_to_show.AutoHideMinHeight = tool.DefaultSize;

            anchorable_to_show.PropertyChanged += AnchorablePropertyChanged;
            return true;
        }

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorable_shown)
        {
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
        }

        private static LayoutAnchorSide GetAnchorSide(LayoutRoot layout, PaneLocation location)
        {
            switch (location)
            {
                case PaneLocation.Left:
                    return layout.LeftSide;
                case PaneLocation.Right:
                    return layout.RightSide;
                case PaneLocation.Bottom:
                    return layout.BottomSide;
                case PaneLocation.Top:
                    return layout.TopSide;
                case PaneLocation.Content:
                default:
                    throw new InvalidOperationException();
            }
        }

        private static LayoutAnchorGroup GetAnchorGroup(LayoutRoot layout, PaneLocation location)
        {
            var side = GetAnchorSide(layout, location);
            if (side.ChildrenCount > 0)
                return side.Children.First();
            else
            {
                var anchor_group = new LayoutAnchorGroup();
                side.Children.Add(anchor_group);
                return anchor_group;
            }
        }

        private void AnchorablePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var la = sender as LayoutAnchorable;
            var tool = la.Content as ITool;
            if (e.PropertyName == "Parent" && la.Parent is LayoutAnchorablePane)
            {
                var pane = la.Parent as LayoutAnchorablePane;

                if (string.IsNullOrWhiteSpace(pane.Name) && pane.Parent != null)
                {
                    pane.Name = "initialized";
                    pane.DockWidth = new GridLength(tool.DefaultSize, GridUnitType.Pixel);
                    pane.DockHeight = new GridLength(tool.DefaultSize, GridUnitType.Pixel);
                }
            }
        }
     
        private void AddToDocumentPane(LayoutRoot layout, LayoutAnchorable anchorable_to_show)
        {
            var pane = layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (pane == null)
                throw new InvalidOperationException();

            pane.Children.Add(anchorable_to_show);
        }
    }
}
