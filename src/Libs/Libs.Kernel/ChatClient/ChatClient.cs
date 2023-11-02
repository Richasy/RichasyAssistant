// Copyright (c) Reader Copilot. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
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
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ChatClient"/> class.
    /// </summary>
    ~ChatClient()
        => Dispose(disposing: false);

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
            }

            _disposedValue = true;
        }
    }
}
