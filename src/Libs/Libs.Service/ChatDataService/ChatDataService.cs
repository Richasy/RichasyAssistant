// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Models.Context;

namespace RichasyAssistant.Libs.Service;

/// <summary>
/// 聊天数据服务.
/// </summary>
public sealed partial class ChatDataService
{
    /// <summary>
    /// 初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="KernelException">错误.</exception>
    public static async Task InitializeAsync()
    {
        if (_dbContext != null)
        {
            return;
        }

        var libPath = GlobalSettings.TryGet<string>(SettingNames.LibraryFolderPath);
        if (string.IsNullOrEmpty(libPath))
        {
            throw new KernelException(KernelExceptionType.LibraryNotInitialized);
        }

        var localDbPath = Path.Combine(libPath, DbName);
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
        _dbContext = context;
        await _dbContext.Database.MigrateAsync();

        await InitializeSessionsAsync();
        await InitializeAssistantsAsync();
        await InitializeExtraKernelsAsync();
    }

    /// <summary>
    /// 初始化自定义内核列表.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task InitializeExtraKernelsAsync()
    {
        if (_extraKernels != null)
        {
            return;
        }

        var filePath = GetExtraKernelFilePath();
        _extraKernels = new List<ServiceMetadata>();
        if (!File.Exists(filePath))
        {
            return;
        }

        var content = await File.ReadAllTextAsync(filePath);
        var data = JsonSerializer.Deserialize<List<ServiceMetadata>>(content);
        if (data != null)
        {
            _extraKernels.AddRange(data);
        }
    }

    /// <summary>
    /// 获取会话列表.
    /// </summary>
    /// <returns>会话列表.</returns>
    public static List<ChatSession> GetSessions()
        => _sessions.OrderByDescending(p => p.Messages.LastOrDefault()?.Time ?? DateTimeOffset.MinValue).ToList();

    /// <summary>
    /// 获取会话.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns>会话负载.</returns>
    public static ChatSession? GetSession(string sessionId)
        => _sessions.FirstOrDefault(p => p.Id == sessionId);

    /// <summary>
    /// 添加或更新会话.
    /// </summary>
    /// <param name="payload">负载.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task AddOrUpdateSessionAsync(ChatSession payload)
    {
        var sourceSession = await _dbContext.Sessions.Include(p => p.Messages).Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == payload.Id);
        if (sourceSession == null)
        {
            await _dbContext.Sessions.AddAsync(payload);
            _sessions.Add(payload);
        }
        else
        {
            var cacheSession = _sessions.FirstOrDefault(p => p.Id == payload.Id);
            DoSame(cacheSession, sourceSession, p => p.Title = payload.Title);
            foreach (var msg in payload.Messages)
            {
                var exist = sourceSession.Messages.Any(p => p.Id == msg.Id);
                if (!exist)
                {
                    if (!cacheSession.Messages.Contains(msg))
                    {
                        cacheSession.Messages.Add(msg);
                    }

                    sourceSession.Messages.Add(msg);
                }
            }

            DoSame(cacheSession, sourceSession, p => p.Options.PresencePenalty = payload.Options.PresencePenalty);
            DoSame(cacheSession, sourceSession, p => p.Options.FrequencyPenalty = payload.Options.FrequencyPenalty);
            DoSame(cacheSession, sourceSession, p => p.Options.MaxResponseTokens = payload.Options.MaxResponseTokens);
            DoSame(cacheSession, sourceSession, p => p.Options.Temperature = payload.Options.Temperature);
            DoSame(cacheSession, sourceSession, p => p.Options.TopP = payload.Options.TopP);
            _dbContext.Sessions.Update(sourceSession);
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 更新会话选项.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <param name="options">选项.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task UpdateSessionOptionsAsync(string sessionId, SessionOptions options)
    {
        var cacheSession = _sessions.FirstOrDefault(p => p.Id == sessionId);
        var sourceSession = await _dbContext.Sessions.Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == sessionId);
        if (cacheSession != null && sourceSession != null)
        {
            DoSame(cacheSession, sourceSession, p => p.Options.PresencePenalty = options.PresencePenalty);
            DoSame(cacheSession, sourceSession, p => p.Options.FrequencyPenalty = options.FrequencyPenalty);
            DoSame(cacheSession, sourceSession, p => p.Options.MaxResponseTokens = options.MaxResponseTokens);
            DoSame(cacheSession, sourceSession, p => p.Options.Temperature = options.Temperature);
            DoSame(cacheSession, sourceSession, p => p.Options.TopP = options.TopP);
            _dbContext.Sessions.Update(sourceSession);

            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 添加会话消息.
    /// </summary>
    /// <param name="message">消息内容.</param>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task AddMessageAsync(ChatMessage message, string sessionId)
    {
        var cacheSession = _sessions.FirstOrDefault(p => p.Id == sessionId);
        if (cacheSession == null)
        {
            return;
        }

        var sourceSession = await _dbContext.Sessions.Include(p => p.Messages).FirstOrDefaultAsync(p => p.Id == sessionId);
        if (sourceSession == null)
        {
            return;
        }

        if (!cacheSession.Messages.Contains(message))
        {
            cacheSession.Messages.Add(message);
        }

        sourceSession.Messages.Add(message);
        _dbContext.Sessions.Update(sourceSession);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 更新会话消息.
    /// </summary>
    /// <param name="message">消息内容.</param>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task UpdateMessageAsync(ChatMessage message, string sessionId)
    {
        var cacheSession = _sessions.FirstOrDefault(p => p.Id == sessionId);
        if (cacheSession == null)
        {
            return;
        }

        var sourceSession = await _dbContext.Sessions.Include(p => p.Messages).FirstOrDefaultAsync(p => p.Id == sessionId);
        if (sourceSession == null)
        {
            return;
        }

        var cacheMsg = sourceSession.Messages.FirstOrDefault(p => p.Id == message.Id);
        var sourceMsg = sourceSession.Messages.FirstOrDefault(p => p.Id == message.Id);
        if (message != null)
        {
            DoSame(sourceMsg, cacheMsg, p => p.Content = message.Content);
            _dbContext.Sessions.Update(sourceSession);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 删除会话消息.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <param name="messageId">消息标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task DeleteMessageAsync(string sessionId, string messageId)
    {
        var cacheSession = _sessions.FirstOrDefault(p => p.Id == sessionId);
        if (cacheSession == null)
        {
            return;
        }

        var sourceSession = await _dbContext.Sessions.Include(p => p.Messages).FirstOrDefaultAsync(p => p.Id == sessionId);
        if (sourceSession == null)
        {
            return;
        }

        var message = sourceSession.Messages.FirstOrDefault(p => p.Id == messageId);
        if (message != null)
        {
            DoSame(cacheSession, sourceSession, p => p.Messages.Remove(message));
            _dbContext.Sessions.Update(sourceSession);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 清除会话消息.
    /// </summary>
    /// <param name="sessionId">会话 Id.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task ClearMessageAsync(string sessionId)
    {
        var cacheSession = _sessions.FirstOrDefault(p => p.Id == sessionId);
        cacheSession?.Messages.Clear();

        var session = await _dbContext.Sessions.Include(p => p.Messages).FirstOrDefaultAsync(p => p.Id == sessionId);
        if (session != null)
        {
            session.Messages.Clear();
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 删除会话.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task DeleteSessionAsync(string sessionId)
    {
        _sessions.RemoveAll(p => p.Id == sessionId);

        var session = await _dbContext.Sessions.Include(p => p.Messages).FirstOrDefaultAsync(p => p.Id == sessionId);
        if (session != null)
        {
            _dbContext.Sessions.Remove(session);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 获取助手列表.
    /// </summary>
    /// <returns>助手列表.</returns>
    public static List<Assistant> GetAssistants()
        => _assistants;

    /// <summary>
    /// 获取助手.
    /// </summary>
    /// <param name="assistantId">助手标识符.</param>
    /// <returns>助手信息.</returns>
    public static Assistant? GetAssistant(string assistantId)
        => _assistants.FirstOrDefault(p => p.Id == assistantId);

    /// <summary>
    /// 添加或更新助手.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task AddOrUpdateAssistantAsync(Assistant assistant)
    {
        var sourceAssistant = await _dbContext.Assistants.FirstOrDefaultAsync(p => p.Id == assistant.Id);
        if (sourceAssistant == null)
        {
            await _dbContext.Assistants.AddAsync(assistant);
            _assistants.Add(assistant);
        }
        else
        {
            var localAssistant = _assistants.FirstOrDefault(p => p.Id == assistant.Id);
            DoSame(localAssistant, sourceAssistant, p => p.Name = assistant.Name);
            DoSame(localAssistant, sourceAssistant, p => p.Description = assistant.Description);
            DoSame(localAssistant, sourceAssistant, p => p.Instruction = assistant.Instruction);
            DoSame(localAssistant, sourceAssistant, p => p.Remark = assistant.Remark);
            _dbContext.Assistants.Update(sourceAssistant);
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 移除助手.
    /// </summary>
    /// <param name="assistantId">助手标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task DeleteAssistantAsync(string assistantId)
    {
        _assistants.RemoveAll(p => p.Id == assistantId);

        var assistant = await _dbContext.Assistants.FirstOrDefaultAsync(p => p.Id == assistantId);
        if (assistant != null)
        {
            _dbContext.Assistants.Remove(assistant);
            await _dbContext.SaveChangesAsync();
        }

        _sessions.RemoveAll(p => p.Assistants.Count == 1 && p.Assistants.Contains(assistantId));
        var session = _dbContext.Sessions.Where(p => p.Assistants.Count == 1 && p.Assistants.Contains(assistantId));
        _dbContext.Sessions.RemoveRange(session);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 获取自定义内核列表.
    /// </summary>
    /// <returns>列表.</returns>
    public static List<ServiceMetadata> GetExtraKernels()
        => _extraKernels;

    /// <summary>
    /// 获取自定义内核.
    /// </summary>
    /// <param name="kernelId">内核标识符.</param>
    /// <returns>内核数据.</returns>
    public static ServiceMetadata GetExtraKernel(string kernelId)
        => _extraKernels.FirstOrDefault(p => p.Id == kernelId);

    /// <summary>
    /// 添加或更新自定义内核.
    /// </summary>
    /// <param name="kernel">内核指针.</param>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="Exception">内核可能已经存在.</exception>
    public static async Task AddOrUpdateExtraKernelAsync(ServiceMetadata kernel)
    {
        if (_extraKernels.Contains(kernel))
        {
            var sourceKernel = _extraKernels.First(p => p.Id == kernel.Id);
            sourceKernel.Name = kernel.Name;
        }
        else
        {
            _extraKernels.Add(kernel);
        }

        var filePath = GetExtraKernelFilePath();
        var content = JsonSerializer.Serialize(_extraKernels);
        await File.WriteAllTextAsync(filePath, content);
    }

    /// <summary>
    /// 删除自定义内核.
    /// </summary>
    /// <param name="kernelId">自定义内核 Id.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task DeleteKernelAsync(string kernelId)
    {
        var localKernel = _extraKernels.FirstOrDefault(p => p.Id == kernelId);
        if (localKernel != null)
        {
            _extraKernels.Remove(localKernel);
            var filePath = GetExtraKernelFilePath();
            var content = JsonSerializer.Serialize(_extraKernels);
            await File.WriteAllTextAsync(filePath, content);
        }
    }

    /// <summary>
    /// 初始化会话.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private static async Task InitializeSessionsAsync()
    {
        try
        {
            _sessions = await _dbContext.Sessions.AsNoTracking().Include(p => p.Messages).Include(p => p.Options).ToListAsync();
        }
        catch (Exception ex)
        {
            var kex = new KernelException(KernelExceptionType.SessionInitializeFailed, ex);
            throw kex;
        }
    }

    /// <summary>
    /// 初始化助手列表.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    private static async Task InitializeAssistantsAsync()
    {
        try
        {
            _assistants = await _dbContext.Assistants.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            var kex = new KernelException(KernelExceptionType.SessionInitializeFailed, ex);
            throw kex;
        }
    }

    private static string GetExtraKernelFilePath()
    {
        var libPath = GlobalSettings.TryGet<string>(SettingNames.LibraryFolderPath);
        return string.IsNullOrEmpty(libPath)
            ? throw new KernelException(KernelExceptionType.LibraryNotInitialized)
            : Path.Combine(libPath, ExtraKernelFileName);
    }

    private static void DoSame<T>(T obj1, T obj2, Action<T> handler)
    {
        handler(obj1);
        handler(obj2);
    }
}
