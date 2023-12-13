// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json.Serialization;

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 模型元数据.
/// </summary>
public class ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceMetadata"/> class.
    /// </summary>
    public ServiceMetadata()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceMetadata"/> class.
    /// </summary>
    public ServiceMetadata(string id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// 标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ServiceMetadata metadata && Id == metadata.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
