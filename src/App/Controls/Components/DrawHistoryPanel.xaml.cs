// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// 绘图历史面板.
/// </summary>
public sealed partial class DrawHistoryPanel : DrawHistoryPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawHistoryPanel"/> class.
    /// </summary>
    public DrawHistoryPanel() => InitializeComponent();

    private void OnHistoryDeleteItemClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement).DataContext as AiImageItemViewModel;
        ViewModel.DeleteImageCommand.Execute(vm);
    }

    private void OnHistoryItemClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement).DataContext as AiImageItemViewModel;
        ViewModel.SetImageCommand.Execute(vm);
    }
}

/// <summary>
/// 绘图历史面板基类.
/// </summary>
public abstract class DrawHistoryPanelBase : ReactiveUserControl<DrawPageViewModel>
{
}
