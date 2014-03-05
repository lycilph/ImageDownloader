using ImageDownloader.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace ImageDownloader.Shell.Utils
{
    public class LayoutStrategy : ILayoutUpdateStrategy
    {
        private enum InsertPosition { Start, End }

        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorable_to_show, ILayoutContainer destination_container)
        {
            LayoutAnchorablePane destination = destination_container as LayoutAnchorablePane;
            if (destination != null && destination.FindParent<LayoutFloatingWindow>() != null)
                return false;

            var tool = anchorable_to_show.Content as ITool;
            if (tool != null)
            {
                var pane_name = tool.DefaultLocation.ToString();
                var tools_pane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.Name == pane_name);
                if (tools_pane == null)
                {
                    switch (tool.DefaultLocation)
                    {
                        case PaneLocation.Left:
                            tools_pane = CreateAnchorablePane(layout, Orientation.Horizontal, pane_name, InsertPosition.Start);
                            break;
                        case PaneLocation.Right:
                            tools_pane = CreateAnchorablePane(layout, Orientation.Horizontal, pane_name, InsertPosition.End);
                            break;
                        case PaneLocation.Bottom:
                            tools_pane = CreateAnchorablePane(layout, Orientation.Vertical, pane_name, InsertPosition.End);
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
                tools_pane.Children.Add(anchorable_to_show);
                return true;
            }

            return false;
        }

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorable_shown)
        {
            // If this is the first anchorable added to this pane, then use the default size.
            var tool = anchorable_shown.Content as ITool;
            if (tool != null)
            {
                var anchorable_pane = anchorable_shown.Parent as LayoutAnchorablePane;
                if (anchorable_pane != null && anchorable_pane.ChildrenCount == 1)
                {
                    switch (tool.DefaultLocation)
                    {
                        case PaneLocation.Left:
                        case PaneLocation.Right:
                            anchorable_pane.DockWidth = new GridLength(tool.DefaultWidth, GridUnitType.Pixel);
                            break;
                        case PaneLocation.Bottom:
                            anchorable_pane.DockHeight = new GridLength(tool.DefaultHeight, GridUnitType.Pixel);
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
        }

        private static LayoutAnchorablePane CreateAnchorablePane(LayoutRoot layout, Orientation orientation, string pane_name, InsertPosition position)
        {
            var parent = layout.Descendents().OfType<LayoutPanel>().First(d => d.Orientation == orientation);
            var tools_pane = new LayoutAnchorablePane { Name = pane_name };
            if (position == InsertPosition.Start)
                parent.InsertChildAt(0, tools_pane);
            else
                parent.Children.Add(tools_pane);
            return tools_pane;
        }
    }
}
