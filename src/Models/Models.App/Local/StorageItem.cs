// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Local;

/// <summary>
/// 存储项.
/// </summary>
public sealed class StorageItem
{
    /// <summary>
    /// File name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// File path.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// File create time.
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// File last modified time.
    /// </summary>
    public DateTime LastModifiedTime { get; set; }

    /// <summary>
    /// File byte length.
    /// </summary>
    public long ByteLength { get; set; }

    /// <summary>
    /// 是否为文件夹.
    /// </summary>
    /// <returns>结果.</returns>
    public bool IsFolder()
        => Directory.Exists(Path);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is StorageItem item && Path == item.Path;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Path);
}
