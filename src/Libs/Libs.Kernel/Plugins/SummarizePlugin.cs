// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using System.Globalization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.Text;

namespace RichasyAssistant.Libs.Kernel.Plugins;

/// <summary>
/// 总结插件.
/// </summary>
public sealed class SummarizePlugin
{
    /// <summary>
    /// 获取会话行为的方法名称.
    /// </summary>
    public const string TitleGeneratorFunctionName = "GenerateTitle";

    private const string TitleGeneratorDefinition = """
        You are a title generator. You will get the first chat message in the chat history, please generate a title for this conversation,
        which must be concise and express the core meaning of the first chat message.

        Need response with {{$LANGUAGE}}, Try to keep it as simple as possible and don't require punctuation.

        EXAMPLE INPUT1:

        I said: "I will record a demo for the new feature by Friday"

        EXAMPLE OUTPUT1:

        Record a demo for the new feature

        EXAMPLE INPUT2:

        I said: "Hey!"

        EXAMPLE OUTPUT2:

        Say hello

        CONTENT STARTS HERE.

        {{$INPUT}}

        CONTENT STOPS HERE.

        OUTPUT:
        """;

    private const int MaxTokens = 1024;

    private readonly KernelFunction _conversationActionFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="SummarizePlugin"/> class.
    /// </summary>
    public SummarizePlugin()
    {
        _conversationActionFunction = KernelFunctionFactory.CreateFromPrompt(
            TitleGeneratorDefinition,
            functionName: nameof(SummarizePlugin),
            description: "Generate a conversation title based on the chat information",
            executionSettings: new PromptExecutionSettings
            {
                ExtensionData = new Dictionary<string, object>
                {
                    { "Temperature", 0.1 },
                    { "TopP", 0.5 },
                    { "MaxTokens", MaxTokens },
                },
            });
    }

    /// <summary>
    /// 根据聊天信息生成会话标题.
    /// </summary>
    /// <param name="conversation">会话内容.</param>
    /// <returns>标题.</returns>
    [KernelFunction(TitleGeneratorFunctionName)]
    [Description("Generate a conversation title based on the chat information")]
    public async Task<string> GenerateTitleAsync(
        [Description("The conversation transcript")] string conversation,
        Microsoft.SemanticKernel.Kernel kernel)
    {
        var lan = CultureInfo.CurrentCulture.Name;
#pragma warning disable SKEXP0055 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
        var firstLine = TextChunker.SplitPlainTextLines(conversation, MaxTokens).FirstOrDefault();
#pragma warning restore SKEXP0055 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
        var arguments = new KernelArguments
        {
            { "LANGUAGE", lan },
            { KernelArguments.InputParameterName, firstLine },
        };

        var result = await _conversationActionFunction.InvokeAsync(kernel, arguments).ConfigureAwait(false);
        return result.GetValue<string>() ?? string.Empty;
    }
}
