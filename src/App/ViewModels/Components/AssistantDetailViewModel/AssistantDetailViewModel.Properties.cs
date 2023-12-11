// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 助手详情视图模型.
/// </summary>
public sealed partial class AssistantDetailViewModel
{
    private readonly List<ServiceMetadata> _azureOpenAIModels;
    private readonly List<ServiceMetadata> _openAIModels;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _instruction;

    [ObservableProperty]
    private BitmapImage _avatar;

    [ObservableProperty]
    private bool _useDefaultKernel;

    [ObservableProperty]
    private bool _isConfigInvalid;

    [ObservableProperty]
    private Assistant _source;

    /// <summary>
    /// 显示的模型列表.
    /// </summary>
    public ObservableCollection<ServiceMetadata> DisplayModels { get; }

    /// <summary>
    /// 所有内核列表.
    /// </summary>
    public ObservableCollection<ServiceMetadata> AllKernels { get; }
}
