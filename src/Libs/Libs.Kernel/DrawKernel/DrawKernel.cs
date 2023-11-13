// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.SemanticKernel.AI.ImageGeneration;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel.DrawKernel;

/// <summary>
/// 绘图内核.
/// </summary>
public sealed partial class DrawKernel
{
    /// <summary>
    /// 绘制图片.
    /// </summary>
    /// <param name="prompt">提示词.</param>
    /// <param name="size">图片大小.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>数据信息.</returns>
    public async Task<string> DrawAsync(string prompt, OpenAIImageSize size, CancellationToken cancellationToken)
    {
        var service = Kernel.GetService<IImageGeneration>();
        var width = size switch
        {
            OpenAIImageSize.Small => 256,
            OpenAIImageSize.Medium => 512,
            OpenAIImageSize.Large => 1024,
            _ => throw new NotImplementedException(),
        };

        var url = await service.GenerateImageAsync(prompt, width, width, cancellationToken);

        return Uri.TryCreate(url, UriKind.Absolute, out _)
            ? url
            : throw new KernelException(KernelExceptionType.GenerateImageFailed);
    }
}
