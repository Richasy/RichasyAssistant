// Copyright (c) Reader Copilot. All rights reserved.

namespace RichasyAssistant.Models.App.Args;

/// <summary>
/// 内核错误类型.
/// </summary>
public enum KernelExceptionType
{
    /// <summary>
    /// 配置无效.
    /// </summary>
    InvalidConfiguration,

    /// <summary>
    /// 内核未初始化.
    /// </summary>
    KernelNotInitialized,

    /// <summary>
    /// 聊天未初始化.
    /// </summary>
    ChatNotInitialized,

    /// <summary>
    /// 聊天会话未找到.
    /// </summary>
    ChatSessionNotFound,

    /// <summary>
    /// 没有可用的聊天会话.
    /// </summary>
    ChatSessionInvalid,

    /// <summary>
    /// 生成聊天响应失败.
    /// </summary>
    GenerateChatResponseFailed,

    /// <summary>
    /// 聊天响应为空.
    /// </summary>
    EmptyChatResponse,
}
