// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 内部语音服务视图模型.
/// </summary>
public sealed partial class InternalSpeechServiceViewModel
{
    [ObservableProperty]
    private string _azureSpeechKey;

    [ObservableProperty]
    private string _azureSpeechRegion;
}
