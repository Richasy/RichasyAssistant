// Copyright (c) Richasy Assistant. All rights reserved.

using System.Threading;
using Microsoft.UI.Dispatching;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 聊天会话视图模型.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;
    private SessionPayload _sourceSession;
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _userInput;

    [ObservableProperty]
    private string _tempMessage;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isResponding;

    [ObservableProperty]
    private bool _isChatEmpty;

    /// <summary>
    /// 请求滚动到底部.
    /// </summary>
    public event EventHandler RequestScrollToBottom;

    /// <summary>
    /// 消息列表.
    /// </summary>
    public ObservableCollection<ChatMessageItemViewModel> Messages { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ChatSessionViewModel model && EqualityComparer<SessionPayload>.Default.Equals(_sourceSession, model._sourceSession);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_sourceSession);
}
