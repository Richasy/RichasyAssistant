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
    /// 获取支持的模型.
    /// </summary>
    /// <param name="type">绘图内核类型.</param>
    /// <returns>绘图模型.</returns>
    public static async Task<Metadata> GetSupportModelsAsync(DrawType type)
    {
        if (type == DrawType.AzureDallE)
        {
            var key = GlobalSettings.TryGet<string>(SettingNames.AzureImageKey);
            var endpoint = GlobalSettings.TryGet<string>(SettingNames.AzureImageEndpoint);
            var models = await Utils.GetAzureOpenAIModelsAsync(key, endpoint);
            if (models?.Any() ?? false)
            {
                var dalle3 = models.FirstOrDefault(p => p.Model == "dall-e-3");
                if (dalle3 != null)
                {
                    return new Metadata(dalle3.Model, dalle3.Id);
                }
            }
        }

        return default;
    }

    /// <summary>
    /// 绘制图片.
    /// </summary>
    /// <param name="prompt">提示词.</param>
    /// <param name="size">图片大小.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>数据信息.</returns>
    public async Task<AiImage> DrawAsync(string prompt, OpenAIImageSize size, CancellationToken cancellationToken)
    {
        // TODO: 暂未支持 Open AI.
        var service = Kernel.GetRequiredService<ITextToImageService>();
        var width = size switch
        {
            OpenAIImageSize.Landscape => 1792,
            _ => 1024,
        };

        var height = size switch
        {
            OpenAIImageSize.Portrait => 1792,
            _ => 1024,
        };

        var url = await service.GenerateImageAsync(prompt, width, height, cancellationToken: cancellationToken);

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
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

        // 链接将在1小时后过期，所以需要下载到本地.
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
