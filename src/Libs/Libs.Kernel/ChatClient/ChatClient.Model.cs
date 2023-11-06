// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天客户端的模型部分.
/// </summary>
public sealed partial class ChatClient
{
    /// <summary>
    /// 获取支持的模型.
    /// </summary>
    /// <param name="type">当前使用的内核类型.</param>
    /// <returns>模型列表.</returns>
    public static async Task<(IEnumerable<string> ChatModels, IEnumerable<string> TextCompletions, IEnumerable<string> Embeddings)> GetSupportModelsAsync(KernelType type)
    {
        using var client = new HttpClient();
        if (type == KernelType.AzureOpenAI)
        {
            var endpoint = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIEndpoint);
            var key = GlobalSettings.TryGet<string>(SettingNames.AzureOpenAIAccessKey);
            var url = $"{endpoint.TrimEnd('/')}/openai/deployments?api-version=2022-12-01";

            var aoaiChatModels = new List<string>();
            var aoaiCompletionModels = new List<string>();
            var aoaiEmbeddingsModels = new List<string>();

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

                    switch (mt)
                    {
                        case "chat":
                            aoaiChatModels.Add(item.Id);
                            break;
                        case "embedding":
                            aoaiEmbeddingsModels.Add(item.Id);
                            break;
                        case "text":
                            aoaiCompletionModels.Add(item.Id);
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
            var oaiChatModels = new List<string>();
            var oaiCompletionModels = new List<string>();
            var oaiEmbeddingsModels = new List<string>();

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

                    switch (mt)
                    {
                        case "chat":
                            oaiChatModels.Add(item.Id);
                            break;
                        case "embedding":
                            oaiEmbeddingsModels.Add(item.Id);
                            break;
                        case "text":
                            oaiCompletionModels.Add(item.Id);
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
