// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Args;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel
{
    [ObservableProperty]
    private Window _activatedWindow;

    /// <summary>
    /// 在有新的提示请求时触发.
    /// </summary>
    public event EventHandler<AppTipNotification> RequestShowTip;

    /// <summary>
    /// 实例.
    /// </summary>
    public static AppViewModel Instance { get; } = new();
}
