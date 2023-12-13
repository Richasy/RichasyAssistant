// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// Whisper 内核.
/// </summary>
public sealed partial class WhisperKernel
{
    /// <summary>
    /// 语义内核.
    /// </summary>
    public Microsoft.SemanticKernel.Kernel Kernel { get; set; }
}
