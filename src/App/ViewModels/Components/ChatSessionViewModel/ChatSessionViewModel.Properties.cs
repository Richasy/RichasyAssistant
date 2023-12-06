// Copyright (c) Richasy Assistant. All rights reserved.

using System.Threading;
using Microsoft.UI.Dispatching;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 聊天会话视图模型.
/// </summary>
public sealed partial class ChatSessionViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;
    private ChatSessionItemViewModel _itemRef;
    private ChatKernel _kernel;
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

    [ObservableProperty]
    private bool _isReady;

    [ObservableProperty]
    private double _maxTokens;

    [ObservableProperty]
    private double _topP;

    [ObservableProperty]
    private double _temperature;

    [ObservableProperty]
    private double _frequencyPenalty;

    [ObservableProperty]
    private double _presencePenalty;

    [ObservableProperty]
    private bool _isInSettings;

    [ObservableProperty]
    private bool _isChatAvailable;

    /// <summary>
    /// 请求滚动到底部.
    /// </summary>
    public event EventHandler RequestScrollToBottom;

    /// <summary>
    /// 请求聚焦于输入框.
    /// </summary>
    public event EventHandler RequestFocusInput;

    /// <summary>
    /// 消息列表.
    /// </summary>
    public ObservableCollection<ChatMessageItemViewModel> Messages { get; }

    /// <summary>
    /// 会话标识符.
    /// </summary>
    public string SessionId => _itemRef?.Id ?? string.Empty;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ChatSessionViewModel model && EqualityComparer<ChatKernel>.Default.Equals(_kernel, model._kernel);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_kernel);
}
