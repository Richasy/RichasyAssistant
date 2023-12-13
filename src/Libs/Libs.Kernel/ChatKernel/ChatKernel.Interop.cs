// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using RichasyAssistant.Libs.Locator;
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
    /// 获取支持的模型.
    /// </summary>
    /// <param name="type">当前使用的内核类型.</param>
    /// <returns>模型列表.</returns>
    public static async Task<(IEnumerable<Metadata> ChatModels, IEnumerable<Metadata> TextCompletions, IEnumerable<Metadata> Embeddings)> GetSupportModelsAsync(KernelType type)
    {
        using var client = new HttpClient();
        if (type == KernelType.AzureOpenAI)
        {
            var endpoint = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIEndpoint);
            var key = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIAccessKey);
            var url = $"{endpoint.TrimEnd('/')}/openai/deployments?api-version=2022-12-01";

            var aoaiChatModels = new List<Metadata>();
            var aoaiCompletionModels = new List<Metadata>();
            var aoaiEmbeddingsModels = new List<Metadata>();

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("api-key", key);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<OpenAIDeploymentResponse>(content);
            if (responseData.Data?.Any() ?? false)
            {
                foreach (var item in responseData.Data)
                {
                    var mt = JudgeModelType(item.Model);
                    if (string.IsNullOrEmpty(mt))
                    {
                        continue;
                    }

                    var metadata = new Metadata(item.Model, item.Id);
                    switch (mt)
                    {
                        case "chat":
                            aoaiChatModels.Add(metadata);
                            break;
                        case "embedding":
                            aoaiEmbeddingsModels.Add(metadata);
                            break;
                        case "text":
                            aoaiCompletionModels.Add(metadata);
                            break;
                    }
                }
            }

            return (aoaiChatModels, aoaiCompletionModels, aoaiEmbeddingsModels);
        }
        else if (type == KernelType.OpenAI)
        {
            var key = GlobalSettings.TryGet<string>(SettingNames.OpenAIAccessKey);
            var url = $"https://api.openai.com/v1/models";
            var oaiChatModels = new List<Metadata>();
            var oaiCompletionModels = new List<Metadata>();
            var oaiEmbeddingsModels = new List<Metadata>();

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<OpenAIDeploymentResponse>(content);
            if (responseData.Data?.Any() ?? false)
            {
                foreach (var item in responseData.Data)
                {
                    var mt = JudgeModelType(item.Id);
                    if (string.IsNullOrEmpty(mt))
                    {
                        continue;
                    }

                    var metadata = new Metadata(item.Id, item.Id);
                    switch (mt)
                    {
                        case "chat":
                            oaiChatModels.Add(metadata);
                            break;
                        case "embedding":
                            oaiEmbeddingsModels.Add(metadata);
                            break;
                        case "text":
                            oaiCompletionModels.Add(metadata);
                            break;
                    }
                }
            }

            return (oaiChatModels, oaiCompletionModels, oaiEmbeddingsModels);
        }

        return default;
    }

    private static string JudgeModelType(string modelName)
    {
        if (modelName.Contains("embedding", StringComparison.OrdinalIgnoreCase) || modelName.Contains("search", StringComparison.OrdinalIgnoreCase))
        {
            return "embedding";
        }
        else if (modelName.Contains("gpt"))
        {
            return "chat";
        }
        else if (modelName.Contains("text-"))
        {
            return "text";
        }

        return string.Empty;
    }

    private IChatCompletionService GetChatCore()
    {
        var chatCore = Kernel.GetRequiredService<IChatCompletionService>()
            ?? throw new KernelException(KernelExceptionType.ChatNotInitialized);
        return chatCore;
    }

    private ChatHistory GetHistory()
    {
        var history = new ChatHistory();
        foreach (var item in Session.Messages.Distinct())
        {
            var role = item.Role == ChatMessageRole.System
                ? AuthorRole.System
                : item.Role == ChatMessageRole.Assistant
                    ? AuthorRole.Assistant
                    : AuthorRole.User;
            history.AddMessage(role, item.Content);
        }

        return history;
    }

    private OpenAIPromptExecutionSettings GetOpenAIRequestSettings()
    {
        var settings = new OpenAIPromptExecutionSettings
        {
            Temperature = Session.Options.Temperature,
            MaxTokens = Session.Options.MaxResponseTokens,
            TopP = Session.Options.TopP,
            FrequencyPenalty = Session.Options.FrequencyPenalty,
            PresencePenalty = Session.Options.PresencePenalty,
        };

        if (Session.Messages?.FirstOrDefault()?.Role == ChatMessageRole.System)
        {
            settings.ChatSystemPrompt = Session.Messages.First().Content;
        }

        return settings;
    }

    private sealed class OpenAIDeploymentResponse
    {
        [JsonPropertyName("data")]
        public List<OpenAIDeployment> Data { get; set; }
    }

    private sealed class OpenAIDeployment
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
