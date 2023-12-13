// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// 额外服务按钮.
/// </summary>
public sealed partial class ExtraServiceButton : ExtraServiceControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtraServiceButton"/> class.
    /// </summary>
    public ExtraServiceButton()
    {
        InitializeComponent();
        ViewModel = ExtraServiceViewModel.Instance;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(default);

    private void OnFlyoutOpened(object sender, object e)
        => RootButton.IsChecked = true;

    private void OnFlyoutClosed(object sender, object e)
        => RootButton.IsChecked = false;

    private void OnRootButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(RootButton);
}

/// <summary>
/// <see cref="ExtraServiceButton"/> 的基类.
/// </summary>
public abstract class ExtraServiceControlBase : ReactiveUserControl<ExtraServiceViewModel>
{
}
