// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// 聊天消息条目视图模型.
/// </summary>
public sealed partial class ChatMessageItemViewModel : ViewModelBase
{
    private readonly ChatMessage _data;

    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private bool _isUser;

    [ObservableProperty]
    private bool _isAssistant;

    [ObservableProperty]
    private string _time;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemViewModel"/> class.
    /// </summary>
    public ChatMessageItemViewModel(ChatMessage message)
    {
        _data = message;
        Content = message.Content;
        IsAssistant = message.Role == ChatMessageRole.Assistant;
        IsUser = message.Role == ChatMessageRole.User;
        Time = message.Time.ToString("MM/dd HH:mm:ss");
    }

    /// <summary>
    /// 获取数据.
    /// </summary>
    /// <returns>消息数据.</returns>
    public ChatMessage GetData()
        => _data;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ChatMessageItemViewModel model && EqualityComparer<ChatMessage>.Default.Equals(_data, model._data);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_data);
}
