// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Configuration;

/// <summary>
/// Azure Open AI 配置面板.
/// </summary>
public sealed partial class AzureOpenAIConfigPanel : InternalKernelConfigPanelBase
{
#pragma warning disable SA1600 // Elements should be documented
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIConfigPanel"/> class.
    /// </summary>
    public AzureOpenAIConfigPanel() => InitializeComponent();

    private void OnAIKeyBoxLostFocus(object sender, RoutedEventArgs e)
        => ViewModel.TryLoadAIModelSourceCommand.Execute(true);
}

/// <summary>
/// 内核配置面板基类.
/// </summary>
public abstract class InternalKernelConfigPanelBase : ReactiveUserControl<InternalKernelViewModel>
{
}
