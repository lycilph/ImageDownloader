﻿<metro:MetroWindow x:Class="$rootnamespace$.Shell.Views.ShellView"
                   xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                   xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                   xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
                   xmlns:metro="http://metro.mahapps.com/winfx/xaml/controls"
                   xmlns:shared="http://metro.mahapps.com/winfx/xaml/shared"
                   xmlns:cal="http://www.caliburnproject.org"
                   xmlns:avalon="http://schemas.xceed.com/wpf/xaml/avalondock"
                   xmlns:avalonDockControls="clr-namespace:Xceed.Wpf.AvalonDock.Controls;assembly=Xceed.Wpf.AvalonDock"
                   xmlns:utils="clr-namespace:$rootnamespace$.Shell.Utils"
                   GlowBrush="{DynamicResource AccentColorBrush}"
                   WindowStartupLocation="CenterScreen"
                   WindowTransitionsEnabled="False"
                   Height="600"
                   Width="800">
    <Window.Resources>
        <avalon:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>

        <Style TargetType="avalonDockControls:AnchorablePaneTitle" BasedOn="{StaticResource {x:Type avalonDockControls:AnchorablePaneTitle}}">
            <Setter Property="BorderThickness" Value="0,3,0,0"/>
        </Style>
    </Window.Resources>

    <i:Interaction.Behaviors>
        <shared:BorderlessWindowBehavior AllowsTransparency="False"/>
        <shared:GlowWindowBehavior/>
    </i:Interaction.Behaviors>

    <metro:MetroWindow.WindowCommands>
        <metro:WindowCommands>
            <ItemsControl x:Name="FlyoutCommands">
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <StackPanel Orientation="Horizontal"/>
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
            </ItemsControl>
            <Button x:Name="ShowAbout" Content="About"/>
        </metro:WindowCommands>
    </metro:MetroWindow.WindowCommands>
    
    <Grid>
        <avalon:DockingManager Grid.Row="1"
                               AllowMixedOrientation="True"
                               AnchorablesSource="{Binding Tools}"
                               DocumentsSource="{Binding Content}"
                               ActiveContent="{Binding ActiveItem, Mode=TwoWay}">
            <avalon:DockingManager.LayoutUpdateStrategy>
                <utils:LayoutStrategy/>
            </avalon:DockingManager.LayoutUpdateStrategy>

            <avalon:DockingManager.LayoutItemTemplate>
                <DataTemplate>
                    <ContentControl cal:View.Model="{Binding}" IsTabStop="False" />
                </DataTemplate>
            </avalon:DockingManager.LayoutItemTemplate>

            <avalon:DockingManager.LayoutItemContainerStyleSelector>
                <utils:PanesStyleSelector>
                    <utils:PanesStyleSelector.ToolStyle>
                        <Style TargetType="{x:Type avalon:LayoutAnchorableItem}">
                            <Setter Property="Title" Value="{Binding Model.DisplayName}"/>
                            <Setter Property="ContentId" Value="{Binding Model.ContentId}"/>
                            <Setter Property="IsSelected" Value="{Binding Model.IsSelected}"/>
                            <Setter Property="Visibility" Value="{Binding Model.IsVisible, Mode=TwoWay, Converter={StaticResource BoolToVisibilityConverter}, ConverterParameter={x:Static Visibility.Hidden}}"/>
                        </Style>
                    </utils:PanesStyleSelector.ToolStyle>
                    <utils:PanesStyleSelector.ContentStyle>
                        <Style TargetType="{x:Type avalon:LayoutItem}">
                            <Setter Property="Title" Value="{Binding Model.DisplayName}"/>
                            <Setter Property="ContentId" Value="{Binding Model.ContentId}"/>
                            <Setter Property="IsSelected" Value="{Binding Model.IsSelected}"/>
                        </Style>
                    </utils:PanesStyleSelector.ContentStyle>
                </utils:PanesStyleSelector>
            </avalon:DockingManager.LayoutItemContainerStyleSelector>

            <avalon:LayoutRoot>
                <avalon:LayoutPanel Orientation="Horizontal">
                    <avalon:LayoutPanel Orientation="Vertical">
                        <avalon:LayoutDocumentPane/>
                    </avalon:LayoutPanel>
                </avalon:LayoutPanel>
            </avalon:LayoutRoot>
        </avalon:DockingManager>
    </Grid>
</metro:MetroWindow>
