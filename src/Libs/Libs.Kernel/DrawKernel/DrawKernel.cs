// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.SemanticKernel.TextToImage;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
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
    public async Task<AiImage> DrawAsync(string prompt, OpenAIImageSize size, CancellationToken cancellationToken)
    {
        var service = Kernel.GetRequiredService<ITextToImageService>();
        var width = size switch
        {
            OpenAIImageSize.Small => 256,
            OpenAIImageSize.Medium => 512,
            OpenAIImageSize.Large => 1024,
            _ => throw new NotImplementedException(),
        };

        var url = await service.GenerateImageAsync(prompt, width, width, cancellationToken: cancellationToken);

        if(!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            // TODO: Maybe base64.
            throw new KernelException(KernelExceptionType.GenerateImageFailed);
        }

        var now = DateTimeOffset.Now;
        var id = now.ToUnixTimeMilliseconds().ToString();
        var libPath = GlobalSettings.TryGet<string>(SettingNames.LibraryFolderPath);
        var fileName = Path.Combine("Images", id + ".png");
        var path = Path.Combine(libPath, fileName);
        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var client = new HttpClient();
        var bytes = await client.GetByteArrayAsync(url);
        await File.WriteAllBytesAsync(path, bytes);

        var aiImage = new AiImage
        {
            Id = now.ToUnixTimeMilliseconds().ToString(),
            Prompt = prompt,
            Link = fileName,
            Time = now,
            Width = width,
            Height = width,
        };

        await DrawDataService.AddImageAsync(aiImage);
        return aiImage;
    }
}
