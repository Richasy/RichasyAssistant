// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.SemanticKernel;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel.DrawKernel;

/// <summary>
/// 绘图内核.
/// </summary>
public sealed partial class DrawKernel
{
    /// <summary>
    /// 创建绘图内核.
    /// </summary>
    /// <param name="type">类型.</param>
    /// <returns>绘图内核.</returns>
    public static DrawKernel Create()
    {
        var kernel = new DrawKernel();
        LoadDefaultConfiguration(kernel);
        return kernel;
    }

    private static void LoadDefaultConfiguration(DrawKernel kernel)
    {
        var defaultKernel = GlobalSettings.TryGet<DrawType>(SettingNames.DefaultDrawService);
        if (defaultKernel == DrawType.AzureDallE)
        {
            LoadAzureConfiguration(kernel);
        }
        else if (defaultKernel == DrawType.OpenAIDallE)
        {
            LoadOpenAIConfiguration(kernel);
        }
    }

    private static void LoadAzureConfiguration(DrawKernel kernel)
    {
        var accessKey = GlobalSettings.TryGet<string>(SettingNames.AzureImageKey);
        var endpoint = GlobalSettings.TryGet<string>(SettingNames.AzureImageEndpoint);

        kernel.IsConfigValid = !string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(endpoint);
        if (!kernel.IsConfigValid)
        {
            return;
        }

        kernel.Kernel = new KernelBuilder()
            .WithAzureOpenAIImageGenerationService(endpoint, accessKey)
            .Build();
    }

    private static void LoadOpenAIConfiguration(DrawKernel kernel)
    {
        var accessKey = GlobalSettings.TryGet<string>(SettingNames.OpenAIImageKey);
        var endpoint = GlobalSettings.TryGet<string>(SettingNames.OpenAICustomEndpoint);

        kernel.IsConfigValid = !string.IsNullOrEmpty(accessKey);
        if (!kernel.IsConfigValid)
        {
            return;
        }

        var hasCustomEndpoint = !string.IsNullOrEmpty(endpoint) && Uri.TryCreate(endpoint, UriKind.Absolute, out var _);
        var customHttpClient = hasCustomEndpoint
            ? new HttpClient(new ProxyOpenAIHandler(endpoint))
            : default;

        kernel.Kernel = new KernelBuilder()
            .WithOpenAIImageGenerationService(accessKey, httpClient: customHttpClient)
            .Build();
    }
}
