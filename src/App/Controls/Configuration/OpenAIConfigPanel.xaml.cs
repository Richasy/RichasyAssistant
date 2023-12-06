// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.Controls.Configuration;

/// <summary>
/// Open AI 配置面板.
/// </summary>
public sealed partial class OpenAIConfigPanel : InternalKernelConfigPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIConfigPanel"/> class.
    /// </summary>
    public OpenAIConfigPanel()
        => InitializeComponent();

    private void OnAIKeyBoxLostFocus(object sender, RoutedEventArgs e)
        => ViewModel.TryLoadAIModelSourceCommand.Execute(false);
}
