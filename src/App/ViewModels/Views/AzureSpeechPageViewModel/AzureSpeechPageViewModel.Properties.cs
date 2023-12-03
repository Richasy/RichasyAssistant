// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Libs.Kernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// Azure 语音服务视图模型.
/// </summary>
public sealed partial class AzureSpeechPageViewModel
{
    private readonly AzureSpeechKernel _kernel;

    [ObservableProperty]
    private AzureTextToSpeechViewModel _textToSpeech;

    [ObservableProperty]
    private AzureSpeechRecognizeViewModel _speechRecognition;

    [ObservableProperty]
    private bool _isInitializing;

    [ObservableProperty]
    private bool _isConfigInvalid;

    [ObservableProperty]
    private bool _needDependencies;

    [ObservableProperty]
    private string _errorDescription;
}
