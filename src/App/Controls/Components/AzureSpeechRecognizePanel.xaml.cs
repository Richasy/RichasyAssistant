// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// Azure 语音识别面板.
/// </summary>
public sealed partial class AzureSpeechRecognizePanel : AzureSpeechRecognizePanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureSpeechRecognizePanel"/> class.
    /// </summary>
    public AzureSpeechRecognizePanel()
    {
        InitializeComponent();
    }
}

/// <summary>
/// Azure 语音识别面板基类.
/// </summary>
public abstract class AzureSpeechRecognizePanelBase : ReactiveUserControl<AzureSpeechRecognizeViewModel>
{
}
