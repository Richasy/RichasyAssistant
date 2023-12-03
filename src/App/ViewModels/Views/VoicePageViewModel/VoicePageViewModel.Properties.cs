// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 语音页面视图模型.
/// </summary>
public sealed partial class VoicePageViewModel
{
    [ObservableProperty]
    private string _poweredBy;

    [ObservableProperty]
    private SpeechType _speechType;
}
