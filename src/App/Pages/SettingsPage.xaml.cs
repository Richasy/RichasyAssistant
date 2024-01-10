// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 设置页面..
/// </summary>
public sealed partial class SettingsPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();
        ViewModel = new SettingsPageViewModel();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);

    private void OnJoinGroupButtonClick(object sender, RoutedEventArgs e)
        => FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
}

/// <summary>
/// 设置页面的基类.
/// </summary>
public abstract class SettingsPageBase : PageBase<SettingsPageViewModel>
{
}
