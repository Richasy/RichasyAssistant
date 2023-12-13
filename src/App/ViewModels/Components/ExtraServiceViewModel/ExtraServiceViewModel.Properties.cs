// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 额外服务视图模型.
/// </summary>
public sealed partial class ExtraServiceViewModel
{
    [ObservableProperty]
    private bool _hasService;

    [ObservableProperty]
    private ServiceType _serviceType;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isKernel;

    [ObservableProperty]
    private bool _isSpeech;

    [ObservableProperty]
    private bool _isTranslate;

    [ObservableProperty]
    private bool _isDraw;

    [ObservableProperty]
    private bool _isEmpty;

    /// <summary>
    /// 实例.
    /// </summary>
    public static ExtraServiceViewModel Instance { get; } = new ExtraServiceViewModel();

    /// <summary>
    /// 自定义内核项集合.
    /// </summary>
    public ObservableCollection<ExtraServiceItemViewModel> CustomKernels { get; }

    /// <summary>
    /// 自定义语音服务项集合.
    /// </summary>
    public ObservableCollection<ExtraServiceItemViewModel> CustomSpeechServices { get; }

    /// <summary>
    /// 自定义绘图服务项集合.
    /// </summary>
    public ObservableCollection<ExtraServiceItemViewModel> CustomDrawServices { get; }

    /// <summary>
    /// 自定义翻译服务项集合.
    /// </summary>
    public ObservableCollection<ExtraServiceItemViewModel> CustomTranslateServices { get; }
}
