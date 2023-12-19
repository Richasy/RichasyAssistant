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
        var model = GlobalSettings.TryGet<string>(SettingNames.DefaultAzureDrawModel);

        kernel.IsConfigValid = !string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(model);
        if (!kernel.IsConfigValid)
        {
            return;
        }

        kernel.Kernel = Microsoft.SemanticKernel.Kernel
            .CreateBuilder()
            .AddAzureOpenAITextToImage(model, endpoint, accessKey, apiVersion: "2023-12-01-preview")
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

        kernel.Kernel = Microsoft.SemanticKernel.Kernel
            .CreateBuilder()
            .AddOpenAITextToImage(accessKey, httpClient: customHttpClient)
            .Build();
    }
}
