// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls.Dialogs;

namespace RichasyAssistant.App.Controls.Settings;

/// <summary>
/// 模型设置.
/// </summary>
public sealed partial class KernelSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KernelSettingSection"/> class.
    /// </summary>
    public KernelSettingSection() => InitializeComponent();

    private async void OnAzureOpenAIEditButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new InternalKernelConfigDialog(ViewModel.InternalKernel, true);
        dialog.XamlRoot = XamlRoot;
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.SaveAzureOpenAISettingsCommand.Execute(default);
        }
    }

    private async void OnOpenAIEditButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new InternalKernelConfigDialog(ViewModel.InternalKernel, false);
        dialog.XamlRoot = XamlRoot;
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.SaveOpenAISettingsCommand.Execute(default);
        }
    }
}
