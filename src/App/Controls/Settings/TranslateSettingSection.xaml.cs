// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls.Dialogs;

namespace RichasyAssistant.App.Controls.Settings;

/// <summary>
/// 翻译服务配置.
/// </summary>
public sealed partial class TranslateSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateSettingSection"/> class.
    /// </summary>
    public TranslateSettingSection() => InitializeComponent();

    private async void OnAzureTranslateEditButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new InternalTranslateConfigDialog(ViewModel.InternalTranslate, true);
        dialog.XamlRoot = XamlRoot;
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.SaveAzureTranslateSettingsCommand.Execute(default);
        }
    }

    private async void OnBaiduTranslateButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new InternalTranslateConfigDialog(ViewModel.InternalTranslate, false);
        dialog.XamlRoot = XamlRoot;
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.SaveBaiduTranslateSettingsCommand.Execute(default);
        }
    }
}
