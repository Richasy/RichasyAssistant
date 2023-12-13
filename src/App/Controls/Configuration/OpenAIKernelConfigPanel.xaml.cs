// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.Controls.Configuration;

/// <summary>
/// Open AI 配置面板.
/// </summary>
public sealed partial class OpenAIKernelConfigPanel : InternalKernelConfigPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIKernelConfigPanel"/> class.
    /// </summary>
    public OpenAIKernelConfigPanel()
        => InitializeComponent();

    private void OnAIKeyBoxLostFocus(object sender, RoutedEventArgs e)
        => ViewModel.TryLoadAIModelSourceCommand.Execute(false);
}
