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
            try
            {
                var actionFunction = _coreFunctions[SummarizePlugin.TitleGeneratorFunctionName];
                var titleResult = await Kernel.InvokeAsync(actionFunction, new KernelArguments()
                {
                    ["input"] = firstMsg.Content,
                });
                var newTitle = titleResult.GetValue<string>();
                if (!string.IsNullOrEmpty(newTitle))
                {
                    session.Title = newTitle;
                    await ChatDataService.AddOrUpdateSessionAsync(session);
                    return newTitle;
                }
            }
            catch (Exception)
            {
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 初始化核心插件.
    /// </summary>
    private void InitializeCorePlugins()
    {
        var summaryPlugin = new SummarizePlugin();
        var summarizeFunctions = Kernel.ImportPluginFromObject(summaryPlugin);
        foreach (var f in summarizeFunctions)
        {
            _coreFunctions.Add(f.Name, f);
        }
    }
}
