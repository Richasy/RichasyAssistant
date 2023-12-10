// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 自定义内核配置.
/// </summary>
public sealed class CustomKernelConfig : ExtraServiceConfig
{
    /// <summary>
    /// 是否是工具.
    /// </summary>
    [JsonPropertyName("is_tool")]
    public bool IsTool { get; set; }
}
