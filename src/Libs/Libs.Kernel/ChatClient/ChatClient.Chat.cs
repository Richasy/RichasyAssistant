// Copyright (c) Reader Copilot. All rights reserved.

using Microsoft.SemanticKernel.AI.ChatCompletion;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天客户端的聊天部分.
/// </summary>
public sealed partial class ChatClient
{
    /// <summary>
    /// 创建新的聊天.
    /// </summary>
    /// <returns>会话数据.</returns>
    public async Task<SessionPayload> CreateNewSessionAsync(string sysPrompt = default, SessionOptions options = default)
    {
        options ??= new SessionOptions
        {
            FrequencyPenalty = GlobalSettings.TryGet<double>(SettingNames.DefaultFrequencyPenalty),
            PresencePenalty = GlobalSettings.TryGet<double>(SettingNames.DefaultPresencePenalty),
            Temperature = GlobalSettings.TryGet<double>(SettingNames.DefaultTemperature),
            TopP = GlobalSettings.TryGet<double>(SettingNames.DefaultTopP),
            MaxResponseTokens = GlobalSettings.TryGet<int>(SettingNames.DefaultMaxResponseTokens),
        };

        var newSession = new ChatSession(sysPrompt, options);
        _sessions.Add(newSession);

        var payload = newSession.GetPayload();
        await AddOrUpdateSessionPayloadAsync(payload);

        return newSession.GetPayload();
    }

    /// <summary>
    /// 切换会话.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    public void SwitchSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
        {
            throw new KernelException(KernelExceptionType.ChatSessionInvalid);
        }

        _currentSessionId = sessionId;
        _ = GetCurrentSession();
    }

    /// <summary>
    /// 获取所有会话信息.
    /// </summary>
    /// <returns>会话负载列表.</returns>
    public List<SessionPayload> GetSessions()
        => _sessions.Select(p => p.GetPayload()).ToList();

    /// <summary>
    /// 获取会话.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns>会话负载.</returns>
    public SessionPayload? GetSession(string sessionId)
        => _sessions.FirstOrDefault(p => p.SessionId == sessionId)?.GetPayload();

    /// <summary>
    /// 发送消息.
    /// </summary>
    /// <param name="message">消息.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>聊天信息.</returns>
    public async Task<ChatMessage> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        var session = GetCurrentSession();
        var chat = GetChatCore();
        session.AddMessage(new ChatMessage(ChatMessageRole.User, message));
        await UpdateCurrentSessionPayloadAsync();

        try
        {
            var response = await chat.GenerateMessageAsync(session.GetChatHistory(), session.GetOpenAIRequestSettings(), cancellationToken);

            if (string.IsNullOrEmpty(response))
            {
                throw new KernelException(KernelExceptionType.EmptyChatResponse);
            }

            var assistantMessage = new ChatMessage(ChatMessageRole.Assistant, response);
            session.AddMessage(assistantMessage);
            await UpdateCurrentSessionPayloadAsync();
            return assistantMessage;
        }
        catch (KernelException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new KernelException(KernelExceptionType.GenerateChatResponseFailed, ex);
        }
    }

    /// <summary>
    /// 发送消息，并接收流式响应.
    /// </summary>
    /// <param name="message">用户消息.</param>
    /// <param name="streamHandler">流式消息处理.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    public async Task<ChatMessage> SendMessageAsync(string message, Action<string> streamHandler, CancellationToken cancellationToken = default)
    {
        var session = GetCurrentSession();
        var chat = GetChatCore();
        session.AddMessage(new ChatMessage(ChatMessageRole.User, message));
        await UpdateCurrentSessionPayloadAsync();

        var resMessage = string.Empty;
        try
        {
            var response = chat.GenerateMessageStreamAsync(session.GetChatHistory(), session.GetOpenAIRequestSettings(), cancellationToken);
            await foreach (var item in response)
            {
                resMessage += item;
                streamHandler(item);
            }

            resMessage = resMessage.Trim();
            if (string.IsNullOrEmpty(resMessage))
            {
                throw new KernelException(KernelExceptionType.EmptyChatResponse);
            }

            var assistantMessage = new ChatMessage(ChatMessageRole.Assistant, resMessage);
            session.AddMessage(assistantMessage);
            await UpdateCurrentSessionPayloadAsync();
            return assistantMessage;
        }
        catch (Exception ex)
        {
            throw new KernelException(KernelExceptionType.GenerateChatResponseFailed, ex);
        }
    }

    /// <summary>
    /// 更新当前会话配置.
    /// </summary>
    /// <param name="options">会话配置.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task UpdateOptionsAsync(SessionOptions options)
    {
        var currentSession = GetCurrentSession();
        currentSession.UpdateOptions(options);
        await UpdateCurrentSessionPayloadAsync();
    }

    private async Task UpdateCurrentSessionPayloadAsync()
    {
        var session = GetCurrentSession();
        if (session == null)
        {
            return;
        }

        var payload = session.GetPayload();
        await AddOrUpdateSessionPayloadAsync(payload);
    }
}
