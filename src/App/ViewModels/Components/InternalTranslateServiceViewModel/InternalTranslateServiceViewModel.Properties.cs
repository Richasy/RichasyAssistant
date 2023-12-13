// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 内部翻译服务视图模型.
/// </summary>
public sealed partial class InternalTranslateServiceViewModel
{
    [ObservableProperty]
    private string _azureTranslateKey;

    [ObservableProperty]
    private string _azureTranslateRegion;

    [ObservableProperty]
    private string _baiduTranslateAppId;

    [ObservableProperty]
    private string _baiduTranslateKey;
}
