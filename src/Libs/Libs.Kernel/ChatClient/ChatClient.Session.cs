// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天客户端的数据管理部分.
/// </summary>
public sealed partial class ChatClient
{
    /// <summary>
    /// 添加或更新会话负载.
    /// </summary>
    /// <param name="payload">负载.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task AddOrUpdateSessionPayloadAsync(SessionPayload payload)
    {
        var context = GetDbContext();
        var sourceSession = await context.Sessions.Include(p => p.Messages).Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == payload.Id);
        if (sourceSession == null)
        {
            await context.Sessions.AddAsync(payload);
            var chatSession = new ChatSession(payload.Id, payload.Title, payload.Messages, payload.Options);
            _sessions.Add(chatSession);
        }
        else
        {
            var localSession = _sessions.FirstOrDefault(p => p.SessionId == payload.Id);
            if (sourceSession.Title != payload.Title)
            {
                sourceSession.Title = payload.Title;
                localSession.Title = payload.Title;
            }

            foreach (var message in payload.Messages)
            {
                var exist = sourceSession.Messages.Any(p => p.Equals(message));
                if (!exist)
                {
                    sourceSession.Messages.Add(message);
                }
            }

            sourceSession.Options.PresencePenalty = payload.Options.PresencePenalty;
            sourceSession.Options.FrequencyPenalty = payload.Options.FrequencyPenalty;
            sourceSession.Options.MaxResponseTokens = payload.Options.MaxResponseTokens;
            sourceSession.Options.Temperature = payload.Options.Temperature;
            sourceSession.Options.TopP = payload.Options.TopP;
            context.Sessions.Update(sourceSession);

            localSession?.UpdateHistory(payload.Messages);
            localSession?.UpdateOptions(payload.Options);
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 初始化会话.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task InitializeSessionsAsync()
    {
        var context = GetDbContext();

        try
        {
            _sessions.Clear();
            var sessions = await context.Sessions.AsNoTracking().Include(p => p.Messages).Include(p => p.Options).ToListAsync();
            foreach (var session in sessions)
            {
                var data = new ChatSession(session.Id, session.Title, session.Messages, session.Options);
                _sessions.Add(data);
            }
        }
        catch (Exception ex)
        {
            var kex = new KernelException(KernelExceptionType.SessionInitializeFailed, ex);
            throw kex;
        }
    }

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

        if (sessionId == _currentSessionId)
        {
            return;
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
    /// 移除消息.
    /// </summary>
    /// <param name="messageId">消息标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemoveMessageAsync(string messageId)
    {
        // 从已加载的历史记录中移除消息.
        var cacheSession = GetCurrentSession();
        cacheSession.RemoveMessage(messageId);

        // 数据库中移除对应的消息.
        var context = GetDbContext();
        var sourceSession = await context.Sessions.Include(p => p.Messages).FirstOrDefaultAsync(p => p.Id == _currentSessionId)
            ?? throw new KernelException(KernelExceptionType.ChatSessionNotFound);
        var removeCount = sourceSession.Messages.RemoveAll(p => p.Id == messageId);
        if (removeCount > 0)
        {
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 移除消息列表.
    /// </summary>
    /// <param name="messageIds">消息标识符列表.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemoveMessagesAsync(params string[] messageIds)
    {
        // 从已加载的历史记录中移除消息.
        var cacheSession = GetCurrentSession();
        foreach (var id in messageIds)
        {
            cacheSession.RemoveMessage(id);
        }

        // 数据库中移除对应的消息.
        var context = GetDbContext();
        var sourceSession = await context.Sessions.Include(p => p.Messages).FirstOrDefaultAsync(p => p.Id == _currentSessionId)
            ?? throw new KernelException(KernelExceptionType.ChatSessionNotFound);
        var removeCount = sourceSession.Messages.RemoveAll(p => messageIds.Contains(p.Id));
        if (removeCount > 0)
        {
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 移除会话.
    /// </summary>
    /// <param name="sessionId">会话 Id.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemoveSessionAsync(string sessionId)
    {
        if (_currentSessionId == sessionId)
        {
            _currentSessionId = default;
        }

        _sessions.RemoveAll(p => p.SessionId == sessionId);
        var context = GetDbContext();
        var sourceSession = await context.Sessions.FirstOrDefaultAsync(p => p.Id == sessionId);
        if (sourceSession != null)
        {
            context.Sessions.Remove(sourceSession);
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

    /// <summary>
    /// 更新当前会话标题.
    /// </summary>
    /// <param name="title">标题.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task UpdateSessionTitleAsync(string title, string sessionId = null)
    {
        var session = (string.IsNullOrEmpty(sessionId)
            ? GetCurrentSession()
            : _sessions.FirstOrDefault(p => p.SessionId == sessionId))
            ?? throw new KernelException(KernelExceptionType.ChatSessionNotFound);

        session.Title = title;
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
