// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Configuration;

/// <summary>
/// Azure 翻译服务配置面板.
/// </summary>
public sealed partial class AzureTranslateConfigPanel : InternalTranslateConfigPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTranslateConfigPanel"/> class.
    /// </summary>
    public AzureTranslateConfigPanel() => InitializeComponent();
}

/// <summary>
/// 内部翻译服务配置面板.
/// </summary>
public abstract class InternalTranslateConfigPanelBase : ReactiveUserControl<InternalTranslateServiceViewModel>
{
}
