// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Kernel;

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
    private Metadata _azureOpenAIChatModel;

    [ObservableProperty]
    private string _openAIAccessKey;

    [ObservableProperty]
    private string _openAICustomEndpoint;

    [ObservableProperty]
    private string _openAIOrganization;

    [ObservableProperty]
    private Metadata _openAIChatModel;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Azure Open AI 的对话模型集合.
    /// </summary>
    public ObservableCollection<Metadata> AzureOpenAIChatModelCollection { get; set; }

    /// <summary>
    /// Open AI 的对话模型集合.
    /// </summary>
    public ObservableCollection<Metadata> OpenAIChatModelCollection { get; set; }
}
