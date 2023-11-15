// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Args;

/// <summary>
/// 内核异常.
/// </summary>
public sealed class KernelException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KernelException"/> class.
    /// </summary>
    public KernelException(KernelExceptionType type)
        : base(type.ToString()) => Type = type;

    /// <summary>
    /// Initializes a new instance of the <see cref="KernelException"/> class.
    /// </summary>
    public KernelException(KernelExceptionType type, Exception ex)
        : base(string.Empty, ex) => Type = type;

    /// <summary>
    /// 异常类型.
    /// </summary>
    public KernelExceptionType Type { get; set; }
}
