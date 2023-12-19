// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 内部绘图服务视图模型.
/// </summary>
public sealed partial class InternalDrawServiceViewModel
{
    [ObservableProperty]
    private string _azureImageKey;

    [ObservableProperty]
    private string _azureImageEndpoint;

    [ObservableProperty]
    private string _openAIImageKey;

    [ObservableProperty]
    private string _openAICustomEndpoint;

    [ObservableProperty]
    private Metadata _azureDrawModel;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// Azure 绘画模型集合.
    /// </summary>
    public ObservableCollection<Metadata> AzureDrawModelCollection { get; set; }
}
