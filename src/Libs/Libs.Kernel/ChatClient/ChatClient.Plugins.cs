// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.SemanticKernel;
using RichasyAssistant.Libs.Kernel.Plugins;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天客户端的插件部分.
/// </summary>
public sealed partial class ChatClient
{
    /// <summary>
    /// 初始化核心插件.
    /// </summary>
    public void InitializeCorePlugins()
    {
        var kernel = GetKernel();
        var summaryPlugin = new SummarizePlugin(kernel);
        var summarizeFunctions = kernel.ImportFunctions(summaryPlugin);
        foreach (var f in summarizeFunctions)
        {
            _coreFunctions.Add(f.Key, f.Value);
        }
    }

    /// <summary>
    /// 生成会话标题.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task<string> TryGenerateTitleAsync()
    {
        var session = GetCurrentSession();
        var firstMsg = session.GetPayload().Messages.FirstOrDefault(p => p.Role == Models.Constants.ChatMessageRole.User);
        if (firstMsg is not null)
        {
            var kernel = GetKernel();
            var actionFunction = _coreFunctions[SummarizePlugin.TitleGeneratorFunctionName];
            var titleResult = await kernel.RunAsync(firstMsg.Content, actionFunction);
            var newTitle = titleResult.GetValue<string>();
            if(!string.IsNullOrEmpty(newTitle))
            {
                await UpdateSessionTitleAsync(newTitle);
                return newTitle;
            }
        }

        return string.Empty;
    }
}
