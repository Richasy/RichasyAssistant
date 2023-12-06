// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 设置页面的视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : ViewModelBase
{
    private const int BuildYear = 2023;

    [ObservableProperty]
    private ElementTheme _appTheme;

    [ObservableProperty]
    private string _appThemeText;

    [ObservableProperty]
    private bool _useMarkdownRenderer;

    [ObservableProperty]
    private string _appVersion;

    [ObservableProperty]
    private string _copyright;

    [ObservableProperty]
    private string _libraryPath;

    [ObservableProperty]
    private KernelMetadata _chatKernel;

    /// <summary>
    /// 对话模型服务列表.
    /// </summary>
    public ObservableCollection<KernelMetadata> ChatKernels { get; }

    /// <summary>
    /// 内部内核视图模型.
    /// </summary>
    public InternalKernelViewModel InternalKernel { get; }
}
