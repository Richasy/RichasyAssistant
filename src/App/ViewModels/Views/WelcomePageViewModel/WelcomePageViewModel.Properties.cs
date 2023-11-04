// Copyright (c) Richasy Assistant. All rights reserved.

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
    private bool _isAIStep;

    [ObservableProperty]
    private bool _isAzureOpenAI;

    [ObservableProperty]
    private bool _isOpenAI;

    [ObservableProperty]
    private string _azureOpenAIAccessKey;

    [ObservableProperty]
    private string _azureOpenAIEndpoint;

    [ObservableProperty]
    private string _azureOpenAIChatModelName;

    [ObservableProperty]
    private string _azureOpenAICompletionModelName;

    [ObservableProperty]
    private string _azureOpenAIEmbeddingModelName;

    [ObservableProperty]
    private string _openAIAccessKey;

    [ObservableProperty]
    private string _openAICustomEndpoint;

    [ObservableProperty]
    private string _openAIOrganization;

    [ObservableProperty]
    private string _openAIChatModelName;

    [ObservableProperty]
    private string _openAICompletionModelName;

    [ObservableProperty]
    private string _openAIEmbeddingModelName;

    /// <summary>
    /// 实例.
    /// </summary>
    public static WelcomePageViewModel Instance { get; } = new();
}
