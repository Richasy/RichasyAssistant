// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// 会话项视图模型.
/// </summary>
public sealed partial class ChatSessionItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private FluentSymbol _icon;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _lastMessage;

    [ObservableProperty]
    private string _date;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private ChatSessionType _type;

    [ObservableProperty]
    private bool _isQuickChat;

    [ObservableProperty]
    private bool _isSingleChat;

    [ObservableProperty]
    private bool _isGroupChat;

    [ObservableProperty]
    private string _assistantAvatar;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionItemViewModel"/> class.
    /// </summary>
    /// <param name="session">会话数据.</param>
    public ChatSessionItemViewModel(ChatSession session)
    {
        Id = session.Id;
        Update(session);
        CheckSessionType();
    }

    /// <summary>
    /// 会话Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 更新.
    /// </summary>
    /// <param name="session">会话数据.</param>
    public void Update(ChatSession session = default)
    {
        session ??= ChatDataService.GetSession(Id);
        Title = string.IsNullOrEmpty(session.Title) ? ResourceToolkit.GetLocalizedString(StringNames.NoName) : session.Title;
        var hasSystemPrompt = session.Messages.Any(p => p.Role == ChatMessageRole.System);
        Icon = hasSystemPrompt ? FluentSymbol.ChatSparkle : FluentSymbol.Chat;
        LastMessage = GetLastMessageText(session);
        Date = GetLastMessage(session)?.Time.ToString("MM/dd") ?? string.Empty;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ChatSessionItemViewModel model && Id == model.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    private static string GetLastMessageText(ChatSession session)
    {
        var lastMsg = GetLastMessage(session);
        if (lastMsg == null)
        {
            return ResourceToolkit.GetLocalizedString(StringNames.NoMessage);
        }
        else
        {
            var role = lastMsg.Role == ChatMessageRole.Assistant
                ? ResourceToolkit.GetLocalizedString(StringNames.Assistant)
                : ResourceToolkit.GetLocalizedString(StringNames.Me);

            return $"{role}: {lastMsg.Content}";
        }
    }

    /// <summary>
    /// 获取最后一条消息.
    /// </summary>
    /// <param name="session">会话数据.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    private static ChatMessage GetLastMessage(ChatSession session)
        => session.Messages.OrderByDescending(p => p.Time).FirstOrDefault();

    private void CheckSessionType()
    {
        var session = ChatDataService.GetSession(Id);
        if (session.Assistants.Count == 0)
        {
            Type = ChatSessionType.Quick;
        }
        else if (session.Assistants.Count == 1)
        {
            Type = ChatSessionType.Single;
        }
        else
        {
            Type = ChatSessionType.Group;
        }

        IsQuickChat = Type == ChatSessionType.Quick;
        IsSingleChat = Type == ChatSessionType.Single;
        IsGroupChat = Type == ChatSessionType.Group;

        if (IsSingleChat)
        {
            AssistantAvatar = ResourceToolkit.GetAssistantAvatarPath(session.Assistants.First());
        }
    }
}
