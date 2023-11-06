// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 迷你页面视图模型.
/// </summary>
public sealed partial class MiniPageViewModel
{
    [ObservableProperty]
    private string _searchText;

    [ObservableProperty]
    private bool _isSearchResultEmpty;

    [ObservableProperty]
    private bool _isHistoryEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isInitialized;

    [ObservableProperty]
    private bool _isInSession;

    [ObservableProperty]
    private string _errorText;

    /// <summary>
    /// 实例.
    /// </summary>
    public static MiniPageViewModel Instance { get; } = new MiniPageViewModel();

    /// <summary>
    /// 会话.
    /// </summary>
    public ChatSessionViewModel Session { get; }

    /// <summary>
    /// 近期会话.
    /// </summary>
    public ObservableCollection<ChatSessionItemViewModel> RecentSessions { get; }
}
