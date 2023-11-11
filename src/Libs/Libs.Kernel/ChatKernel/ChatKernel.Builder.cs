// Copyright (c) Richasy Assistant. All rights reserved.

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
    public static ChatKernel Create(string sessionId)
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
                LoadAzureConfiguration(kernel, assistant.Model);
            }
            else if (assistant.Kernel == KernelType.OpenAI)
            {
                LoadOpenAIConfiguration(kernel, assistant.Model);
            }
        }
        else
        {
            // 快速对话，使用默认配置.
            LoadDefaultConfiguration(kernel);
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
        return Create(id);
    }

    private static void LoadDefaultConfiguration(ChatKernel kernel)
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
    }

    private static void LoadAzureConfiguration(ChatKernel kernel, string modelName = default)
    {
        var accessKey = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIAccessKey);
        var endpoint = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIEndpoint);
        var model = string.IsNullOrEmpty(modelName)
            ? GlobalSettings.TryGet<string>(SettingNames.DefaultAzureOpenAIChatModelName)
            : modelName;

        if (string.IsNullOrEmpty(accessKey)
        || string.IsNullOrEmpty(endpoint)
        || string.IsNullOrEmpty(model))
        {
            throw new KernelException(KernelExceptionType.InvalidConfiguration);
        }

        kernel.Kernel = new KernelBuilder()
            .WithAzureOpenAIChatCompletionService(model, endpoint, accessKey)
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
            throw new KernelException(KernelExceptionType.InvalidConfiguration);
        }

        var hasCustomEndpoint = !string.IsNullOrEmpty(customEndpoint) && Uri.TryCreate(customEndpoint, UriKind.Absolute, out var _);
        var customHttpClient = hasCustomEndpoint
            ? new HttpClient(new ProxyOpenAIHandler(customEndpoint))
            : default;

        kernel.Kernel = new KernelBuilder()
            .WithOpenAIChatCompletionService(model, accessKey, org, httpClient: customHttpClient)
            .Build();
    }

    private class ProxyOpenAIHandler : HttpClientHandler
    {
        private readonly string _proxyUrl;

        public ProxyOpenAIHandler(string proxyUrl)
            => _proxyUrl = proxyUrl.TrimEnd('/');

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri != null && request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase))
            {
                var path = request.RequestUri.PathAndQuery.TrimStart('/');
                request.RequestUri = new Uri($"{_proxyUrl}/{path}");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
