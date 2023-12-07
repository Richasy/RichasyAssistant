// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.Controls.Configuration;

/// <summary>
/// Azure 耳语服务配置面板.
/// </summary>
public sealed partial class AzureWhisperConfigPanel : InternalSpeechConfigPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureWhisperConfigPanel"/> class.
    /// </summary>
    public AzureWhisperConfigPanel() => InitializeComponent();

    private void OnWhisperKeyBoxLostFocus(object sender, RoutedEventArgs e)
        => ViewModel.TryLoadModelSourceCommand.Execute(default);
}
