// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.SemanticKernel;
using RichasyAssistant.Libs.Kernel.Plugins;
using RichasyAssistant.Libs.Service;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天内核.
/// </summary>
public sealed partial class ChatKernel
{
    /// <summary>
    /// 生成会话标题.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task<string> TryGenerateTitleAsync()
    {
        var session = ChatDataService.GetSession(SessionId);
        var firstMsg = session.Messages.FirstOrDefault(p => p.Role == Models.Constants.ChatMessageRole.User);
        if (firstMsg is not null)
        {
            var actionFunction = _coreFunctions[SummarizePlugin.TitleGeneratorFunctionName];
            var titleResult = await Kernel.RunAsync(firstMsg.Content, actionFunction);
            var newTitle = titleResult.GetValue<string>();
            if (!string.IsNullOrEmpty(newTitle))
            {
                session.Title = newTitle;
                await ChatDataService.AddOrUpdateSessionAsync(session);
                return newTitle;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 初始化核心插件.
    /// </summary>
    private void InitializeCorePlugins()
    {
        var summaryPlugin = new SummarizePlugin(Kernel);
        var summarizeFunctions = Kernel.ImportFunctions(summaryPlugin);
        foreach (var f in summarizeFunctions)
        {
            _coreFunctions.Add(f.Key, f.Value);
        }
    }
}
