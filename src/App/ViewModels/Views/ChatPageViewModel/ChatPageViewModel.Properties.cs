// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 聊天页面视图模型.
/// </summary>
public sealed partial class ChatPageViewModel
{
    [ObservableProperty]
    private bool _isHistoryEmpty;

    [ObservableProperty]
    private bool _isAssistantsEmpty;

    [ObservableProperty]
    private bool _isInitializing;

    [ObservableProperty]
    private string _initializeErrorText;

    [ObservableProperty]
    private double _listColumnWidth;

    [ObservableProperty]
    private string _listTitle;

    [ObservableProperty]
    private ChatListType _listType;

    [ObservableProperty]
    private bool _isSessionList;

    [ObservableProperty]
    private bool _isAssistantList;

    /// <summary>
    /// 近期会话.
    /// </summary>
    public ObservableCollection<ChatSessionItemViewModel> RecentSessions { get; }

    /// <summary>
    /// 会话详情.
    /// </summary>
    public ChatSessionViewModel SessionDetail { get; }
}
