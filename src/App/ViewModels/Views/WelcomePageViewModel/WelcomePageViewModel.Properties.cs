// Copyright (c) Richasy Assistant. All rights reserved.

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
    private bool _isSpeechStep;

    [ObservableProperty]
    private bool _isAzureSpeech;

    [ObservableProperty]
    private bool _isImageStep;

    [ObservableProperty]
    private bool _isAzureImage;

    [ObservableProperty]
    private bool _isOpenAIImage;

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

    /// <summary>
    /// 内部绘图服务视图模型.
    /// </summary>
    public InternalDrawServiceViewModel InternalDrawService { get; }

    /// <summary>
    /// 内部翻译服务视图模型.
    /// </summary>
    public InternalTranslateServiceViewModel InternalTranslate { get; }

    /// <summary>
    /// 内部语音服务视图模型.
    /// </summary>
    public InternalSpeechServiceViewModel InternalSpeech { get; }
}
