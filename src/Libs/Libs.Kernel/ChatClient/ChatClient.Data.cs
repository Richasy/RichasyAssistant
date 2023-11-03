// Copyright (c) Reader Copilot. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RichasyAssistant.Libs.Database;
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
    public static async Task AddOrUpdateSessionPayloadAsync(SessionPayload payload)
    {
        var context = GlobalVariables.TryGet<ChatDbContext>(VariableNames.ChatDbContext)
            ?? throw new KernelException(KernelExceptionType.ChatDbNotInitialized);
        var sourceSession = await context.Sessions.Include(p => p.Messages).Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == payload.Id);
        if (sourceSession == null)
        {
            await context.Sessions.AddAsync(payload);
        }
        else
        {
            if (sourceSession.Title != payload.Title)
            {
                sourceSession.Title = payload.Title;
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
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 初始化本地数据库.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task InitializeLocalDatabaseAsync()
    {
        var libPath = GlobalSettings.TryGet<string>(SettingNames.LibraryFolderPath);
        if (string.IsNullOrEmpty(libPath))
        {
            throw new KernelException(KernelExceptionType.LibraryNotInitialized);
        }

        var localDbPath = Path.Combine(libPath, "chat.db");
        if (!File.Exists(localDbPath))
        {
            var defaultChatDbPath = GlobalSettings.TryGet<string>(SettingNames.DefaultChatDbPath);
            if (string.IsNullOrEmpty(defaultChatDbPath)
                || !File.Exists(defaultChatDbPath))
            {
                throw new KernelException(KernelExceptionType.ChatDbInitializeFailed);
            }

            await Task.Run(() =>
            {
                File.Copy(defaultChatDbPath, localDbPath, true);
            });
        }

        var context = new ChatDbContext(localDbPath);
        GlobalVariables.Set(VariableNames.ChatDbContext, context);

        try
        {
            _sessions.Clear();
            var sessions = await context.Sessions.AsNoTracking().Include(p => p.Messages).Include(p => p.Options).ToListAsync();
            foreach (var session in sessions)
            {
                var data = new ChatSession(session.Id, session.Messages, session.Options);
                _sessions.Add(data);
            }
        }
        catch (Exception ex)
        {
            var kex = new KernelException(KernelExceptionType.ChatDbInitializeFailed, ex);
            GlobalVariables.TryRemove(VariableNames.ChatDbContext);
            throw kex;
        }
    }

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
        var context = GlobalVariables.TryGet<ChatDbContext>(VariableNames.ChatDbContext)
            ?? throw new KernelException(KernelExceptionType.ChatDbNotInitialized);
        var sourceSession = await context.Sessions.Include(p => p.Messages).FirstOrDefaultAsync(p => p.Id == _currentSessionId)
            ?? throw new KernelException(KernelExceptionType.ChatSessionNotFound);
        var removeCount = sourceSession.Messages.RemoveAll(p => p.Id == messageId);
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
        var context = GlobalVariables.TryGet<ChatDbContext>(VariableNames.ChatDbContext)
            ?? throw new KernelException(KernelExceptionType.ChatDbNotInitialized);
        var sourceSession = await context.Sessions.FirstOrDefaultAsync(p => p.Id == sessionId);
        if (sourceSession != null)
        {
            context.Sessions.Remove(sourceSession);
        }
    }
}
