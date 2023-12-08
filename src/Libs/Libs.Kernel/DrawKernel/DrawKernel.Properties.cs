// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Libs.Kernel.DrawKernel;

/// <summary>
/// 绘图内核.
/// </summary>
public sealed partial class DrawKernel
{
    /// <summary>
    /// 内核.
    /// </summary>
    public Microsoft.SemanticKernel.Kernel Kernel { get; private set; }

    /// <summary>
    /// 配置是否有效.
    /// </summary>
    public bool IsConfigValid { get; private set; }
}
