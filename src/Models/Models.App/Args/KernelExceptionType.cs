// Copyright (c) Richasy Assistant. All rights reserved.

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
    /// 聊天响应已取消.
    /// </summary>
    ChatResponseCancelled,

    /// <summary>
    /// 聊天响应为空.
    /// </summary>
    EmptyChatResponse,

    /// <summary>
    /// 资料库尚未初始化.
    /// </summary>
    LibraryNotInitialized,

    /// <summary>
    /// 聊天数据库初始化失败.
    /// </summary>
    ChatDbNotInitialized,

    /// <summary>
    /// 聊天数据库初始化失败.
    /// </summary>
    ChatDbInitializeFailed,

    /// <summary>
    /// 会话列表初始化失败.
    /// </summary>
    SessionInitializeFailed,

    /// <summary>
    /// 提示词列表初始化失败.
    /// </summary>
    PromptInitializeFailed,

    /// <summary>
    /// 系统提示词未找到.
    /// </summary>
    SystemPromptNotFound,
}
