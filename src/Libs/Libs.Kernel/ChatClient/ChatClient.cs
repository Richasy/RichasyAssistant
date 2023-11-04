// Copyright (c) Reader Copilot. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using RichasyAssistant.Libs.Database;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天客户端.
/// </summary>
public sealed partial class ChatClient : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClient"/> class.
    /// </summary>
    public ChatClient()
    {
        _sessions = new List<ChatSession>();
        _prompts = new List<SystemPrompt>();
        _coreFunctions = new Dictionary<string, ISKFunction>();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ChatClient"/> class.
    /// </summary>
    ~ChatClient()
        => Dispose(disposing: false);

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

        await InitializeSessionsAsync();
        await InitializePromptsAsync();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private static IKernel GetKernel()
    {
        var kernel = GlobalVariables.TryGet<IKernel>(VariableNames.ChatKernel)
            ?? throw new KernelException(KernelExceptionType.KernelNotInitialized);

        return kernel;
    }

    private static IChatCompletion GetChatCore()
    {
        var kernel = GetKernel();
        var chatCore = kernel.GetService<IChatCompletion>()
            ?? throw new KernelException(KernelExceptionType.ChatNotInitialized);
        return chatCore;
    }

    /// <summary>
    /// 获取数据库上下文.
    /// </summary>
    /// <returns>数据库上下文.</returns>
    private static ChatDbContext GetDbContext()
        => GlobalVariables.TryGet<ChatDbContext>(VariableNames.ChatDbContext)
            ?? throw new KernelException(KernelExceptionType.ChatDbNotInitialized);

    private ChatSession GetCurrentSession()
    {
        if (string.IsNullOrEmpty(_currentSessionId))
        {
            throw new KernelException(KernelExceptionType.ChatSessionInvalid);
        }

        var session = _sessions.FirstOrDefault(p => p.SessionId == _currentSessionId)
            ?? throw new KernelException(KernelExceptionType.ChatSessionNotFound);

        return session;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                GlobalVariables.TryRemove(VariableNames.ChatKernel);
                GlobalVariables.TryRemove(VariableNames.ChatDbContext);
            }

            _disposedValue = true;
        }
    }
}
