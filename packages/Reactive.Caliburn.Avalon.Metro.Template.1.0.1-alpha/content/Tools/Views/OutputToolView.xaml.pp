﻿<UserControl x:Class="$rootnamespace$.Tools.Views.OutputToolView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
             xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
             xmlns:utils="clr-namespace:$rootnamespace$.Tools.Utils"
             mc:Ignorable="d" 
             d:DesignHeight="300" d:DesignWidth="300">
    <DockPanel>
        <StackPanel DockPanel.Dock="Left">
            <Button x:Name="Clear"
                    ToolTip="Clears the log"
                    Height="36"
                    Style="{StaticResource MetroCircleButtonStyle}">
                <Image Source="..\..\Images\appbar.delete.png"/>
            </Button>
        </StackPanel>
        
        <ScrollViewer Margin="10,5">
            <ItemsControl x:Name="Messages" Background="Transparent">
                <i:Interaction.Behaviors>
                    <utils:BringIntoViewBehavior/>
                </i:Interaction.Behaviors>
            </ItemsControl>
        </ScrollViewer>
    </DockPanel>
</UserControl>
