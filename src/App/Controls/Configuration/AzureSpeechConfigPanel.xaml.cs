// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Configuration;

/// <summary>
/// Azure 语音服务配置面板.
/// </summary>
public sealed partial class AzureSpeechConfigPanel : InternalSpeechConfigPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureSpeechConfigPanel"/> class.
    /// </summary>
    public AzureSpeechConfigPanel() => InitializeComponent();
}

/// <summary>
/// 内部语音服务配置面板.
/// </summary>
public abstract class InternalSpeechConfigPanelBase : ReactiveUserControl<InternalSpeechServiceViewModel>
{
}
