// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Configuration;

/// <summary>
/// Azure Open AI 配置面板.
/// </summary>
public sealed partial class AzureOpenAIKernelConfigPanel : InternalKernelConfigPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIKernelConfigPanel"/> class.
    /// </summary>
    public AzureOpenAIKernelConfigPanel() => InitializeComponent();

    private void OnAIKeyBoxLostFocus(object sender, RoutedEventArgs e)
        => ViewModel.TryLoadAIModelSourceCommand.Execute(true);
}

/// <summary>
/// 内核配置面板基类.
/// </summary>
public abstract class InternalKernelConfigPanelBase : ReactiveUserControl<InternalKernelViewModel>
{
}
