// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls.Dialogs;

namespace RichasyAssistant.App.Controls.Settings;

/// <summary>
/// 绘图设置.
/// </summary>
public sealed partial class DrawSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawSettingSection"/> class.
    /// </summary>
    public DrawSettingSection() => InitializeComponent();

    private async void OnAzureDrawEditButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new InternalDrawConfigDialog(ViewModel.InternalDrawService, DrawType.AzureDallE);
        dialog.XamlRoot = XamlRoot;
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.SaveAzureDrawSettingsCommand.Execute(default);
        }
    }

    private async void OnOpenAIDrawButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new InternalDrawConfigDialog(ViewModel.InternalDrawService, DrawType.OpenAIDallE);
        dialog.XamlRoot = XamlRoot;
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.SaveOpenAIDrawSettingsCommand.Execute(default);
        }
    }
}
