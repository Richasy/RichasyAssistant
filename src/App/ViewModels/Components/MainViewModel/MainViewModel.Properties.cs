// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Models.App.Args;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 主视图模型.
/// </summary>
public sealed partial class MainViewModel
{
    /// <summary>
    /// 请求导航.
    /// </summary>
    public event EventHandler<AppNavigateEventArgs> RequestNavigate;

    /// <summary>
    /// 实例.
    /// </summary>
    public static MainViewModel Instance { get; } = new();

    /// <summary>
    /// 导航项集合.
    /// </summary>
    public ObservableCollection<NavigateItemViewModel> NavigateItems { get; }

    /// <summary>
    /// 设置项.
    /// </summary>
    public NavigateItemViewModel SettingsItem { get; }

    /// <summary>
    /// 当前功能.
    /// </summary>
    public FeatureType CurrentFeature { get; private set; }
}
