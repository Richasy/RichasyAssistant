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

    [ObservableProperty]
    private string _azureWhisperKey;

    [ObservableProperty]
    private string _azureWhisperEndpoint;

    [ObservableProperty]
    private string _azureWhisperModelName;

    [ObservableProperty]
    private string _openAIWhisperKey;

    [ObservableProperty]
    private string _openAICustomEndpoint;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Azure 耳语模型列表.
    /// </summary>
    public ObservableCollection<string> AzureWhisperModels { get; }
}
