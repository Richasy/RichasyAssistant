// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.SemanticKernel;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天内核.
/// </summary>
public sealed partial class ChatKernel
{
    private readonly Dictionary<string, KernelFunction> _coreFunctions;
    private Microsoft.SemanticKernel.Kernel _kernel;

    /// <summary>
    /// 会话标识符.
    /// </summary>
    public string SessionId { get; private set; }

    /// <summary>
    /// 语义内核.
    /// </summary>
    public Microsoft.SemanticKernel.Kernel Kernel
    {
        get => _kernel;
        set
        {
            _kernel = value;
            InitializeCorePlugins();
        }
    }

    /// <summary>
    /// 会话信息.
    /// </summary>
    public ChatSession Session => ChatDataService.GetSession(SessionId);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatKernel kernel && SessionId == kernel.SessionId;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(SessionId);
}
