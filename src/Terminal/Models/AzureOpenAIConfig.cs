// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Terminal.Models;

internal sealed class AzureOpenAIConfig
{
    public string AccessKey { get; set; }

    public string Endpoint { get; set; }

    public string ChatModel { get; set; }

    public string CompletionModel { get; set; }

    public string EmbeddingModel { get; set; }
}
