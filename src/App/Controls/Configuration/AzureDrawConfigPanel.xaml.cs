// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Configuration;

/// <summary>
/// Azure 绘图配置面板.
/// </summary>
public sealed partial class AzureDrawConfigPanel : InternalDrawConfigPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureDrawConfigPanel"/> class.
    /// </summary>
    public AzureDrawConfigPanel() => InitializeComponent();
}

/// <summary>
/// 绘图配置面板基类.
/// </summary>
public abstract class InternalDrawConfigPanelBase : ReactiveUserControl<InternalDrawServiceViewModel>
{
}
