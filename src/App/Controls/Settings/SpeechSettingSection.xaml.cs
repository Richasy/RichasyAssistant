// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls.Dialogs;

namespace RichasyAssistant.App.Controls.Settings;

/// <summary>
/// 语音设置面板.
/// </summary>
public sealed partial class SpeechSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpeechSettingSection"/> class.
    /// </summary>
    public SpeechSettingSection() => InitializeComponent();

    private async void OnAzureSpeechEditButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var dialog = new InternalSpeechConfigDialog(ViewModel.InternalSpeech, SpeechType.Azure);
        dialog.XamlRoot = XamlRoot;
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.SaveAzureSpeechSettingsCommand.Execute(default);
        }
    }
}
