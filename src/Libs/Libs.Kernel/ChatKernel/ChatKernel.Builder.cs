// Copyright (c) Richasy Assistant. All rights reserved.

using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.SemanticKernel;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天内核.
/// </summary>
public sealed partial class ChatKernel
{
    /// <summary>
    /// 从已有的会话中创建内核.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <returns><see cref="ChatKernel"/>.</returns>
    public static async Task<ChatKernel> CreateAsync(string sessionId)
    {
        var kernel = new ChatKernel();
        kernel.SessionId = sessionId;
        var sessionData = ChatDataService.GetSession(sessionId);
        if (sessionData.Assistants?.Any() == true)
        {
            // 根据助理创建内核.
            // TODO: 目前仅支持一对一.
            var assistant = ChatDataService.GetAssistant(sessionData.Assistants.First());
            if (assistant.Kernel == KernelType.AzureOpenAI)
            {
                LoadAzureConfiguration(kernel, new Metadata(assistant.Model, assistant.Remark));
            }
            else if (assistant.Kernel == KernelType.OpenAI)
            {
                LoadOpenAIConfiguration(kernel, assistant.Model);
            }
            else
            {
                await LoadCustomConfigurationAsync(kernel, assistant.Model);
            }
        }
        else
        {
            // 快速对话，使用默认配置.
            await LoadDefaultConfigurationAsync(kernel);
        }

        return kernel;
    }

    /// <summary>
    /// 创建新的聊天.
    /// </summary>
    /// <param name="assistant">选择的聊天助理.</param>
    /// <returns>聊天内核.</returns>
    public static async Task<ChatKernel> CreateAsync(Assistant assistant = default)
    {
        var id = Guid.NewGuid().ToString();
        var session = new ChatSession
        {
            Id = id,
            Title = string.Empty,
            Messages = new List<ChatMessage>(),
            Assistants = new List<string>(),
            Options = new SessionOptions
            {
                SessionId = id,
                TopP = GlobalSettings.TryGet<double>(SettingNames.DefaultTopP),
                Temperature = GlobalSettings.TryGet<double>(SettingNames.DefaultTemperature),
                MaxResponseTokens = GlobalSettings.TryGet<int>(SettingNames.DefaultMaxResponseTokens),
                FrequencyPenalty = GlobalSettings.TryGet<double>(SettingNames.DefaultFrequencyPenalty),
                PresencePenalty = GlobalSettings.TryGet<double>(SettingNames.DefaultPresencePenalty),
            },
        };

        if (assistant != null)
        {
            session.Assistants.Add(assistant.Id);
        }

        await ChatDataService.AddOrUpdateSessionAsync(session);
        return await CreateAsync(id);
    }

    /// <summary>
    /// 默认内核是否有效.
    /// </summary>
    /// <returns>是否有效.</returns>
    public static bool IsDefaultKernelValid()
    {
        var defaultKernel = GlobalSettings.TryGet<KernelType>(SettingNames.DefaultKernel);
        if (defaultKernel == KernelType.AzureOpenAI)
        {
            return !string.IsNullOrEmpty(GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIAccessKey))
                && !string.IsNullOrEmpty(GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIEndpoint))
                && !string.IsNullOrEmpty(GlobalSettings.TryGet<string>(SettingNames.DefaultAzureOpenAIChatModel));
        }
        else if (defaultKernel == KernelType.OpenAI)
        {
            return !string.IsNullOrEmpty(GlobalSettings.TryGet<string>(SettingNames.OpenAIAccessKey))
                && !string.IsNullOrEmpty(GlobalSettings.TryGet<string>(SettingNames.DefaultOpenAIChatModelName));
        }
        else
        {
            var customModelId = GlobalSettings.TryGet<string>(SettingNames.CustomKernelId);
            return !string.IsNullOrEmpty(customModelId);
        }
    }

    private static async Task LoadDefaultConfigurationAsync(ChatKernel kernel)
    {
        var defaultKernel = GlobalSettings.TryGet<KernelType>(SettingNames.DefaultKernel);
        if (defaultKernel == KernelType.AzureOpenAI)
        {
            LoadAzureConfiguration(kernel);
        }
        else if (defaultKernel == KernelType.OpenAI)
        {
            LoadOpenAIConfiguration(kernel);
        }
        else
        {
            await LoadCustomConfigurationAsync(kernel);
        }
    }

    private static void LoadAzureConfiguration(ChatKernel kernel, Metadata model = default)
    {
        var accessKey = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIAccessKey);
        var endpoint = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIEndpoint);

        if (model == null)
        {
            var modelJson = GlobalSettings.TryGet<string>(SettingNames.DefaultAzureOpenAIChatModel);
            if (!string.IsNullOrEmpty(modelJson))
            {
                model = JsonSerializer.Deserialize<Metadata>(modelJson);
            }
        }

        if (string.IsNullOrEmpty(accessKey)
        || string.IsNullOrEmpty(endpoint)
        || model == null)
        {
            throw new Models.App.Args.KernelException(KernelExceptionType.InvalidConfiguration);
        }

        kernel.Kernel = new KernelBuilder()
            .AddAzureOpenAIChatCompletion(model.Id, model.Value, endpoint, accessKey)
            .Build();
    }

    private static void LoadOpenAIConfiguration(ChatKernel kernel, string modelName = default)
    {
        var accessKey = GlobalSettings.TryGet<string>(SettingNames.OpenAIAccessKey);
        var org = GlobalSettings.TryGet<string>(SettingNames.OpenAIOrganization);
        var customEndpoint = GlobalSettings.TryGet<string>(SettingNames.OpenAICustomEndpoint);
        var model = string.IsNullOrEmpty(modelName)
            ? GlobalSettings.TryGet<string>(SettingNames.DefaultOpenAIChatModelName)
            : modelName;

        if (string.IsNullOrEmpty(accessKey)
            || string.IsNullOrEmpty(model))
        {
            throw new Models.App.Args.KernelException(KernelExceptionType.InvalidConfiguration);
        }

        var hasCustomEndpoint = !string.IsNullOrEmpty(customEndpoint) && Uri.TryCreate(customEndpoint, UriKind.Absolute, out var _);
        var customHttpClient = hasCustomEndpoint
            ? GetProxyClient(customEndpoint)
            : default;

        kernel.Kernel = new KernelBuilder()
            .AddOpenAIChatCompletion(model, accessKey, org, httpClient: customHttpClient)
            .Build();
    }

    private static async Task LoadCustomConfigurationAsync(ChatKernel kernel, string modelId = default)
    {
        if (string.IsNullOrEmpty(modelId))
        {
            modelId = GlobalSettings.TryGet<string>(SettingNames.CustomKernelId);
        }

        if (string.IsNullOrEmpty(modelId))
        {
            throw new Models.App.Args.KernelException(KernelExceptionType.InvalidConfiguration);
        }

        var libPath = GlobalSettings.TryGet<string>(SettingNames.LibraryFolderPath);
        var modelFolder = Path.Combine(libPath, "Extensions", "Kernel", modelId);
        if (!Directory.Exists(modelFolder))
        {
            throw new Models.App.Args.KernelException(KernelExceptionType.InvalidConfiguration);
        }

        var configPath = Path.Combine(modelFolder, "config.json");
        var config = JsonSerializer.Deserialize<CustomKernelConfig>(await File.ReadAllTextAsync(configPath));
        var proxyClient = GetProxyClient(config.BaseUrl);
        kernel.Kernel = new KernelBuilder()
            .AddOpenAIChatCompletion(config.Id, "RichasyAssistant", httpClient: proxyClient)
            .Build();
    }

    private static HttpClient GetProxyClient(string baseUrl)
    {
        var httpClient = new HttpClient(new ProxyOpenAIHandler(baseUrl));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
        return httpClient;
    }
}
