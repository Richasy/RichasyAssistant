// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.Constants;

/// <summary>
/// 聊天会话类型.
/// </summary>
public enum ChatSessionType
{
    /// <summary>
    /// 快速会话，使用默认聊天服务.
    /// </summary>
    Quick,

    /// <summary>
    /// 单人会话，使用指定的聊天助理.
    /// </summary>
    Single,

    /// <summary>
    /// 群组会话.
    /// </summary>
    Group,
}
