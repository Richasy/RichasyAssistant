// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using System.Globalization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
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

        Need response with {{$LANGUAGE}}.

        EXAMPLE INPUT1:

        I said: ""I will record a demo for the new feature by Friday""

        EXAMPLE OUTPUT1:

        Record a demo for the new feature

        EXAMPLE INPUT2:

        I said: ""Hey!""

        EXAMPLE OUTPUT2:

        Say hello.

        CONTENT STARTS HERE.

        {{$INPUT}}

        CONTENT STOPS HERE.

        OUTPUT:
        """;

    private const int MaxTokens = 1024;

    private readonly ISKFunction _conversationActionFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="SummarizePlugin"/> class.
    /// </summary>
    public SummarizePlugin(IKernel kernel)
    {
        _conversationActionFunction = kernel.CreateSemanticFunction(
            TitleGeneratorDefinition,
            pluginName: nameof(SummarizePlugin),
            description: "Generate a conversation title based on the chat information",
            requestSettings: new Microsoft.SemanticKernel.AI.AIRequestSettings
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
    /// <param name="context">上下文.</param>
    /// <returns><see cref="SKContext"/>.</returns>
    [SKFunction]
    [Description("Generate a conversation title based on the chat information")]
    public async Task<SKContext> GenerateTitleAsync(
        [Description("The conversation transcript")] string conversation,
        SKContext context)
    {
        var lan = CultureInfo.CurrentCulture.Name;
        context.Variables.TryAdd("LANGUAGE", lan);
        var firstLine = TextChunker.SplitPlainTextLines(conversation, MaxTokens).FirstOrDefault();
        context.Variables.Update(firstLine);
        await _conversationActionFunction.InvokeAsync(context).ConfigureAwait(false);
        return context;
    }
}
