// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.Constants;

/// <summary>
/// 文件排序方式.
/// </summary>
public enum StorageSortType
{
    /// <summary>
    /// Name (A to Z).
    /// </summary>
    NameAtoZ,

    /// <summary>
    /// Name (Z to A).
    /// </summary>
    NameZtoA,

    /// <summary>
    /// Modified time (New to old).
    /// </summary>
    ModifiedTime,

    /// <summary>
    /// File type.
    /// </summary>
    Type,

    /// <summary>
    /// Size (Large to small).
    /// </summary>
    SizeLargeToSmall,

    /// <summary>
    /// Size (Small to large).
    /// </summary>
    SizeSmallToLarge,
}
