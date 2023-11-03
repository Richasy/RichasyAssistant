﻿// Copyright (c) Reader Copilot. All rights reserved.

using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天客户端的属性.
/// </summary>
public sealed partial class ChatClient
{
    private readonly List<ChatSession> _sessions;
    private readonly List<SystemPrompt> _prompts;
    private bool _disposedValue;
    private string? _currentSessionId;

    /// <summary>
    /// 内核类型.
    /// </summary>
    public KernelType? Type { get; private set; }
}
