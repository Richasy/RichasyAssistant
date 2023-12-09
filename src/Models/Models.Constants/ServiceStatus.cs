// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.Constants;

/// <summary>
/// 服务状态.
/// </summary>
public enum ServiceStatus
{
    /// <summary>
    /// 未启动.
    /// </summary>
    NotStarted,

    /// <summary>
    /// 正在运行.
    /// </summary>
    Running,

    /// <summary>
    /// 正在启动.
    /// </summary>
    Launching,

    /// <summary>
    /// 启动失败.
    /// </summary>
    Failed,
}
