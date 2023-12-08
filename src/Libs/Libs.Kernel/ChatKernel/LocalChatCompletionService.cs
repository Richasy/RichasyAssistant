// Copyright (c) Richasy Assistant. All rights reserved.

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

internal class LocalChatCompletionService : IChatCompletionService, IDisposable
{
    private readonly string _basicChatPath;
    private readonly string _streamChatPath;
    private readonly HttpClient _httpClient;

    public LocalChatCompletionService(CustomKernelConfig config)
    {
        ModelId = config.Id;
        BaseUrl = config.BaseUrl;
        IsTool = config.IsTool;
        SupportStreamOutput = config.SupportStreamOutput;
        _basicChatPath = config.BasicChat;
        _streamChatPath = config.StreamChat;
        _httpClient = new HttpClient();
    }

    public string ModelId { get; }

    public string BaseUrl { get; }

    public bool SupportStreamOutput { get; }

    public bool IsTool { get; set; }

    public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

    public void Dispose() => ((IDisposable)_httpClient).Dispose();

    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings, Microsoft.SemanticKernel.Kernel? kernel, CancellationToken cancellationToken)
    {
        var request = GetRequestInternal(chatHistory, executionSettings);
        var url = BaseUrl + _basicChatPath;
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Content = JsonContent.Create(request);

        using var response = await _httpClient.SendWithSuccessCheckAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringWithExceptionMappingAsync().ConfigureAwait(false);
        var responseObj = JsonSerializer.Deserialize<CustomKernelChatResponse>(body)
            ?? throw new KernelException("Unexpected response from model")
            {
                Data = { { "ResponseData", body } },
            };

        var result = new List<ChatMessageContent>
        {
            new ChatMessageContent(
                IsTool ? AuthorRole.Tool : AuthorRole.Assistant,
                responseObj.Message,
                ModelId,
                responseObj.Extension),
        };

        return result;
    }

    public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings, Microsoft.SemanticKernel.Kernel? kernel, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var request = GetRequestInternal(chatHistory, executionSettings);
        var url = new Uri(BaseUrl + _streamChatPath);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
        httpRequest.Content = JsonContent.Create(request);

        using var response = await _httpClient.SendWithSuccessCheckAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        using var bodyStream = await response.Content.ReadAsStreamAndTranslateExceptionAsync().ConfigureAwait(false);
        using var reader = new StreamReader(bodyStream, Encoding.UTF8);
        var line = string.Empty;
        while ((line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) != null)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            if (line.StartsWith("data:"))
            {
                var msg = JsonSerializer.Deserialize<CustomKernelChatResponse>(line[5..].Trim());
                if (msg.IsFinish)
                {
                    break;
                }
                else
                {
                    yield return new StreamingChatMessageContent(IsTool ? AuthorRole.Tool : AuthorRole.Assistant, msg.Message);
                }
            }
        }
    }

    private static CustomKernelChatRequest GetRequestInternal(ChatHistory chatHistory, PromptExecutionSettings? executionSettings)
    {
        var request = new CustomKernelChatRequest();
        request.Message = chatHistory.LastOrDefault(p => p.Role == AuthorRole.User)?.Content;
        var history = new List<ChatMessage>();
        for (var i = 0; i < chatHistory.Count - 1; i++)
        {
            var msg = chatHistory[i];
            var role = ChatMessageRole.User;
            if (msg.Role == AuthorRole.System)
            {
                role = ChatMessageRole.System;
            }
            else if (msg.Role == AuthorRole.Assistant)
            {
                role = ChatMessageRole.Assistant;
            }
            else if (msg.Role == AuthorRole.Tool)
            {
                role = ChatMessageRole.Tool;
            }

            var customMsg = new ChatMessage(role, msg.Content, default, msg.ModelId);
            history.Add(customMsg);
        }

        var options = new SessionOptions();
        var requestSettings = executionSettings as OpenAIPromptExecutionSettings;
        options.SessionId = executionSettings.ModelId;
        options.Temperature = requestSettings.Temperature;
        options.MaxResponseTokens = requestSettings.MaxTokens ?? 1024;
        options.TopP = requestSettings.TopP;
        options.FrequencyPenalty = requestSettings.FrequencyPenalty;
        options.PresencePenalty = requestSettings.PresencePenalty;

        request.History = history;
        request.Options = options;

        return request;
    }
}
