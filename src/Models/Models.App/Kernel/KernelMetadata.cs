// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Kernel;

/// <summary>
/// 模型元数据.
/// </summary>
public class KernelMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KernelMetadata"/> class.
    /// </summary>
    public KernelMetadata()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KernelMetadata"/> class.
    /// </summary>
    public KernelMetadata(string id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// 标识符.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    public string Name { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is KernelMetadata metadata && Id == metadata.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
