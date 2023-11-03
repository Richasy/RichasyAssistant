// Copyright (c) Reader Copilot. All rights reserved.

using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天会话.
/// </summary>
public sealed partial class ChatSession
{
    private List<ChatMessage> _messages;
    private SessionOptions? _requestSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSession"/> class.
    /// </summary>
    public ChatSession(string systemPrompt, SessionOptions options)
    {
        _messages = new List<ChatMessage>();
        SessionId = Guid.NewGuid().ToString("N");
        options.SessionId = SessionId;
        _requestSettings = options;

        if (!string.IsNullOrEmpty(systemPrompt))
        {
            _messages.Add(new ChatMessage(ChatMessageRole.System, systemPrompt));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSession"/> class.
    /// </summary>
    public ChatSession(
        string sessionId,
        List<ChatMessage> messages,
        SessionOptions options)
    {
        SessionId = sessionId;
        UpdateHistory(messages);
        UpdateOptions(options);
    }

    /// <summary>
    /// 会话标识符.
    /// </summary>
    public string SessionId { get; }

    /// <summary>
    /// 添加新消息.
    /// </summary>
    /// <param name="message">消息内容.</param>
    public void AddMessage(ChatMessage message)
        => _messages.Add(message);

    /// <summary>
    /// 移除消息.
    /// </summary>
    /// <param name="messageId">消息 Id.</param>
    public void RemoveMessage(string messageId)
        => _messages.RemoveAll(p => p.Id == messageId);

    /// <summary>
    /// 更新系统提示.
    /// </summary>
    /// <param name="prompt">系统提示词.</param>
    public void UpdateSystemPrompt(string prompt)
    {
        if (_messages != null)
        {
            var source = _messages.FirstOrDefault(p => p.Role == ChatMessageRole.System);
            if (source == null)
            {
                _messages.Insert(0, new ChatMessage(ChatMessageRole.System, prompt));
            }
            else
            {
                source.Content = prompt;
            }
        }
    }

    /// <summary>
    /// 更新请求设置.
    /// </summary>
    /// <param name="options">设置.</param>
    public void UpdateOptions(SessionOptions options)
    {
        options.SessionId = SessionId;
        _requestSettings = options;
    }

    /// <summary>
    /// 更新历史记录.
    /// </summary>
    /// <param name="messages">历史消息列表.</param>
    public void UpdateHistory(List<ChatMessage> messages)
        => _messages = messages;

    /// <summary>
    /// 获取聊天历史记录.
    /// </summary>
    /// <returns><see cref="ChatHistory"/>.</returns>
    public ChatHistory GetChatHistory()
    {
        var history = new ChatHistory();
        foreach (var item in _messages)
        {
            var role = item.Role == ChatMessageRole.System
                ? AuthorRole.System
                : item.Role == ChatMessageRole.Assistant
                    ? AuthorRole.Assistant
                    : AuthorRole.User;
            history.AddMessage(role, item.Content);
        }

        return history;
    }

    /// <summary>
    /// 获取 OpenAI 请求设置.
    /// </summary>
    /// <returns><see cref="OpenAIRequestSettings"/>.</returns>
    public OpenAIRequestSettings GetOpenAIRequestSettings()
    {
        var settings = new OpenAIRequestSettings
        {
            Temperature = _requestSettings.Temperature,
            MaxTokens = _requestSettings.MaxResponseTokens,
            TopP = _requestSettings.TopP,
            FrequencyPenalty = _requestSettings.FrequencyPenalty,
            PresencePenalty = _requestSettings.PresencePenalty,
        };

        if (_messages?.FirstOrDefault()?.Role == ChatMessageRole.System)
        {
            settings.ChatSystemPrompt = _messages.First().Content;
        }

        return settings;
    }

    /// <summary>
    /// 获取当前会话的负载.
    /// </summary>
    /// <returns><see cref="SessionPayload"/>.</returns>
    public SessionPayload GetPayload()
    {
        return new SessionPayload
        {
            Id = SessionId,
            Title = string.Empty,
            Messages = _messages ?? new List<ChatMessage>(),
            Options = _requestSettings ?? new SessionOptions
            {
                SessionId = SessionId,
                TopP = GlobalSettings.TryGet<double>(SettingNames.DefaultTopP),
                Temperature = GlobalSettings.TryGet<double>(SettingNames.DefaultTemperature),
                MaxResponseTokens = GlobalSettings.TryGet<int>(SettingNames.DefaultMaxResponseTokens),
                FrequencyPenalty = GlobalSettings.TryGet<double>(SettingNames.DefaultFrequencyPenalty),
                PresencePenalty = GlobalSettings.TryGet<double>(SettingNames.DefaultPresencePenalty),
            },
        };
    }
}
