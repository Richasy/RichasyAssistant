// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 内部内核视图模型.
/// </summary>
public sealed partial class InternalKernelViewModel
{
    [ObservableProperty]
    private string _azureOpenAIAccessKey;

    [ObservableProperty]
    private string _azureOpenAIEndpoint;

    [ObservableProperty]
    private string _azureOpenAIChatModelName;

    [ObservableProperty]
    private string _openAIAccessKey;

    [ObservableProperty]
    private string _openAICustomEndpoint;

    [ObservableProperty]
    private string _openAIOrganization;

    [ObservableProperty]
    private string _openAIChatModelName;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Azure Open AI 的对话模型集合.
    /// </summary>
    public ObservableCollection<string> AzureOpenAIChatModelCollection { get; set; }

    /// <summary>
    /// Open AI 的对话模型集合.
    /// </summary>
    public ObservableCollection<string> OpenAIChatModelCollection { get; set; }
}
