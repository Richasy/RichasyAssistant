// Copyright (c) Reader Copilot. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天客户端的提示词部分.
/// </summary>
public sealed partial class ChatClient
{
    /// <summary>
    /// 添加或更新提示词.
    /// </summary>
    /// <param name="prompt">提示词.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task AddOrUpdatePromptAsync(SystemPrompt prompt)
    {
        var context = GetDbContext();
        var sourcePrompt = await context.SystemPrompts.FirstOrDefaultAsync(p => p.Id == prompt.Id);
        if (sourcePrompt == null)
        {
            await context.SystemPrompts.AddAsync(prompt);
            _prompts.Add(prompt);
        }
        else
        {
            sourcePrompt.Name = prompt.Name;
            sourcePrompt.Prompt = prompt.Prompt;
            context.SystemPrompts.Update(sourcePrompt);

            var localPrompt = _prompts.FirstOrDefault(p => p.Id == prompt.Id);
            localPrompt.Prompt = prompt.Prompt;
            localPrompt.Name = prompt.Name;
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 初始化提示词列表.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task InitializePromptsAsync()
    {
        try
        {
            _prompts.Clear();
            var context = GetDbContext();
            var prompts = await context.SystemPrompts.ToListAsync();
            _prompts.AddRange(prompts);
        }
        catch (Exception ex)
        {
            var kex = new KernelException(KernelExceptionType.PromptInitializeFailed, ex);
            throw kex;
        }
    }

    /// <summary>
    /// 获取提示词列表.
    /// </summary>
    /// <returns>提示词列表.</returns>
    public List<SystemPrompt> GetPrompts()
        => _prompts;

    /// <summary>
    /// 移除提示词.
    /// </summary>
    /// <param name="promptId">提示词标识符.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task RemovePromptAsync(string promptId)
    {
        // 从已加载的提示词列表中移除.
        _prompts.RemoveAll(p => p.Id == promptId);

        // 从数据库中移除对应的提示词.
        var context = GetDbContext();
        var prompt = await context.SystemPrompts.FirstOrDefaultAsync(p => p.Id == promptId)
            ?? throw new KernelException(KernelExceptionType.SystemPromptNotFound);
        var removedData = context.SystemPrompts.Remove(prompt);
        if (removedData != null)
        {
            await context.SaveChangesAsync();
        }
    }
}
