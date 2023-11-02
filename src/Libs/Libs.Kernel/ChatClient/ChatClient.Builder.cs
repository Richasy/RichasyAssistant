// Copyright (c) Reader Copilot. All rights reserved.

using Microsoft.SemanticKernel;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天客户端的构建.
/// </summary>
public sealed partial class ChatClient
{
    /// <summary>
    /// 使用 Azure OpenAI 的服务配置.
    /// </summary>
    /// <returns><see cref="ChatClient"/>.</returns>
    public ChatClient UseAzure()
    {
        var accessKey = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIAccessKey);
        var endpoint = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIEndpoint);
        var chatModelName = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIChatModelName);
        var completionModelName = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAICompletionModelName);

        if (string.IsNullOrEmpty(accessKey)
            || string.IsNullOrEmpty(endpoint)
            || (string.IsNullOrEmpty(chatModelName) && string.IsNullOrEmpty(completionModelName)))
        {
            throw new KernelException(KernelExceptionType.InvalidConfiguration);
        }

        var builder = new KernelBuilder();
        if (!string.IsNullOrEmpty(chatModelName))
        {
            builder.WithAzureChatCompletionService(chatModelName, endpoint, accessKey);
        }

        if (!string.IsNullOrEmpty(completionModelName))
        {
            builder.WithAzureTextCompletionService(completionModelName, endpoint, accessKey);
        }

        var kernel = builder.Build();
        GlobalVariables.Set(VariableNames.ChatKernel, kernel);
        Type = KernelType.AzureOpenAI;

        return this;
    }

    /// <summary>
    /// 使用 Open AI 的服务配置.
    /// </summary>
    /// <returns><see cref="ChatClient"/>.</returns>
    public ChatClient UseOpenAI()
    {
        var accessKey = GlobalSettings.TryGet<string>(SettingNames.OpenAIAccessKey);
        var org = GlobalSettings.TryGet<string>(SettingNames.OpenAIOrganization);
        var customEndpoint = GlobalSettings.TryGet<string>(SettingNames.OpenAICustomEndpoint);
        var chatModelName = GlobalSettings.TryGet<string>(SettingNames.OpenAIChatModelName);
        var completionModelName = GlobalSettings.TryGet<string>(SettingNames.OpenAICompletionModelName);

        if (string.IsNullOrEmpty(accessKey)
            || (string.IsNullOrEmpty(chatModelName) && string.IsNullOrEmpty(completionModelName)))
        {
            throw new KernelException(KernelExceptionType.InvalidConfiguration);
        }

        var hasCustomEndpoint = !string.IsNullOrEmpty(customEndpoint) && Uri.TryCreate(customEndpoint, UriKind.Absolute, out var _);
        var customHttpClient = hasCustomEndpoint
            ? new HttpClient(new ProxyOpenAIHandler(customEndpoint))
            : default;

        var builder = new KernelBuilder();
        if (!string.IsNullOrEmpty(chatModelName))
        {
            builder.WithOpenAIChatCompletionService(chatModelName, accessKey, org, httpClient: customHttpClient);
        }

        if (!string.IsNullOrEmpty(completionModelName))
        {
            builder.WithAzureTextCompletionService(completionModelName, accessKey, org, httpClient: customHttpClient);
        }

        var kernel = builder.Build();
        GlobalVariables.Set(VariableNames.ChatKernel, kernel);
        Type = KernelType.OpenAI;

        return this;
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
