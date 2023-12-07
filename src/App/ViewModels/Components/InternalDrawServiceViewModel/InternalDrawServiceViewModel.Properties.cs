// Copyright (c) Richasy Assistant. All rights reserved.

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
}
