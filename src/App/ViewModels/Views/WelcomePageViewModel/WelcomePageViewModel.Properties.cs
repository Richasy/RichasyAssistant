﻿// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 欢迎页视图模型.
/// </summary>
public sealed partial class WelcomePageViewModel
{
    [ObservableProperty]
    private int _currentStep;

    [ObservableProperty]
    private int _stepCount;

    [ObservableProperty]
    private bool _isFREStep;

    [ObservableProperty]
    private bool _isLibraryStep;

    [ObservableProperty]
    private KernelType _kernelType;

    [ObservableProperty]
    private TranslateType _translateType;

    [ObservableProperty]
    private SpeechType _speechType;

    [ObservableProperty]
    private DrawType _imageGenerateType;

    [ObservableProperty]
    private bool _isAIStep;

    [ObservableProperty]
    private bool _isAzureOpenAI;

    [ObservableProperty]
    private bool _isOpenAI;

    [ObservableProperty]
    private bool _isLastStep;

    [ObservableProperty]
    private bool _isTranslateStep;

    [ObservableProperty]
    private bool _isAzureTranslate;

    [ObservableProperty]
    private bool _isBaiduTranslate;

    [ObservableProperty]
    private string _azureTranslateKey;

    [ObservableProperty]
    private string _azureTranslateRegion;

    [ObservableProperty]
    private string _baiduTranslateAppId;

    [ObservableProperty]
    private string _baiduTranslateKey;

    [ObservableProperty]
    private bool _isSpeechStep;

    [ObservableProperty]
    private bool _isAzureSpeech;

    [ObservableProperty]
    private bool _isAzureWhisper;

    [ObservableProperty]
    private bool _isOpenAIWhisper;

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
    private bool _isImageStep;

    [ObservableProperty]
    private bool _isAzureImage;

    [ObservableProperty]
    private bool _isOpenAIImage;

    [ObservableProperty]
    private string _azureImageKey;

    [ObservableProperty]
    private string _azureImageEndpoint;

    [ObservableProperty]
    private string _openAIImageKey;

    [ObservableProperty]
    private bool _isPreviousStepShown;

    [ObservableProperty]
    private bool _isNextStepButtonEnabled;

    /// <summary>
    /// 实例.
    /// </summary>
    public static WelcomePageViewModel Instance { get; } = new();

    /// <summary>
    /// 内部内核视图模型.
    /// </summary>
    public InternalKernelViewModel InternalKernel { get; }
}
