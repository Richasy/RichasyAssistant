// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 额外服务配置.
/// </summary>
public class ExtraServiceConfig : ServiceMetadata
{
    /// <summary>
    /// 访问路径.
    /// </summary>
    [JsonPropertyName("base_url")]
    public string BaseUrl { get; set; }

    /// <summary>
    /// 脚本路径.
    /// </summary>
    [JsonPropertyName("run_script")]
    public string RunScript { get; set; }

    /// <summary>
    /// 初始化脚本路径.
    /// </summary>
    [JsonPropertyName("initial_script")]
    public string InitialScript { get; set; }
}
