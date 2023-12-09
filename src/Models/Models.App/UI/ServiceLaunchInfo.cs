// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App.UI;

/// <summary>
/// 服务启动信息.
/// </summary>
public sealed class ServiceLaunchInfo
{
    /// <summary>
    /// 启动结果.
    /// </summary>
    [JsonPropertyName("status")]
    public ServiceLaunchResult Status { get; set; }

    /// <summary>
    /// 错误消息.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}
