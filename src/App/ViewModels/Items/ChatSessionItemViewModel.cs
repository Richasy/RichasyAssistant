// Copyright (c) Richasy Assistant. All rights reserved.

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

    private ChatSession _payload;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionItemViewModel"/> class.
    /// </summary>
    /// <param name="payload">会话数据.</param>
    public ChatSessionItemViewModel(ChatSession payload)
        => Update(payload);

    /// <summary>
    /// 更新.
    /// </summary>
    /// <param name="payload">会话数据.</param>
    public void Update(ChatSession payload)
    {
        _payload = payload;
        Title = string.IsNullOrEmpty(payload.Title) ? GetDefaultTitle(payload) : payload.Title;
        var hasSystemPrompt = payload.Messages.Any(p => p.Role == ChatMessageRole.System);
        Icon = hasSystemPrompt ? FluentSymbol.ChatSparkle : FluentSymbol.Chat;
        LastMessage = GetLastMessageText(payload);
        Date = GetLastMessage(payload)?.Time.ToString("MM/dd") ?? string.Empty;
    }

    /// <summary>
    /// 获取原始数据.
    /// </summary>
    /// <returns>会话数据.</returns>
    public ChatSession GetData()
        => _payload;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ChatSessionItemViewModel model && EqualityComparer<ChatSession>.Default.Equals(_payload, model._payload);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_payload);

    private static string GetDefaultTitle(ChatSession payload)
    {
        var lastMsg = GetLastMessage(payload);
        return lastMsg is null
            ? ResourceToolkit.GetLocalizedString(StringNames.Session) + " - " + payload.Id
            : ResourceToolkit.GetLocalizedString(StringNames.Session) + " - " + lastMsg.Time.ToString("yyyyMMddHHmmss");
    }

    private static string GetLastMessageText(ChatSession payload)
    {
        var lastMsg = GetLastMessage(payload);
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
    /// <param name="payload">会话数据.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    private static ChatMessage GetLastMessage(ChatSession payload)
        => payload.Messages.OrderByDescending(p => p.Time).FirstOrDefault();
}
