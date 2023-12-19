// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace RichasyAssistant.Libs.Kernel;

internal static class Utils
{
    public static async Task<List<OpenAIDeployment>> GetAzureOpenAIModelsAsync(string key, string endpoint)
    {
        using var client = new HttpClient();
        var url = $"{endpoint.TrimEnd('/')}/openai/deployments?api-version=2022-12-01";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("api-key", key);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<OpenAIDeploymentResponse>(content);
        return responseData.Data;
    }
}

internal sealed class OpenAIDeploymentResponse
{
    [JsonPropertyName("data")]
    public List<OpenAIDeployment> Data { get; set; }
}

internal sealed class OpenAIDeployment
{
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}
