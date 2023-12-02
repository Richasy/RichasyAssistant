// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天内核.
/// </summary>
public sealed partial class ChatKernel
{
    private ChatKernel()
    {
        _coreFunctions = new Dictionary<string, ISKFunction>();
    }

    /// <summary>
    /// 发送消息.
    /// </summary>
    /// <param name="message">消息.</param>
    /// <param name="userMsgHandler">用户消息添加处理.</param>
    /// <param name="ignoreUserMessage">是否不将传入消息作为用户消息添加到历史记录.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>聊天信息.</returns>
    public async Task<Models.App.Kernel.ChatMessage> SendMessageAsync(string message, Action<Models.App.Kernel.ChatMessage> userMsgHandler, bool ignoreUserMessage = false, CancellationToken cancellationToken = default)
    {
        var chat = GetChatCore();
        var userMsg = new Models.App.Kernel.ChatMessage(ChatMessageRole.User, message);
        if (!ignoreUserMessage)
        {
            await ChatDataService.AddMessageAsync(userMsg, SessionId);
            userMsgHandler?.Invoke(userMsg);
        }

        try
        {
            var response = await chat.GenerateMessageAsync(GetHistory(), GetOpenAIRequestSettings(), cancellationToken);

            if (string.IsNullOrEmpty(response))
            {
                throw new KernelException(KernelExceptionType.EmptyChatResponse);
            }

            var assistantMessage = new Models.App.Kernel.ChatMessage(ChatMessageRole.Assistant, response);
            await ChatDataService.AddMessageAsync(assistantMessage, SessionId);
            return assistantMessage;
        }
        catch (KernelException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException)
            {
                throw new KernelException(KernelExceptionType.ChatResponseCancelled, ex);
            }

            throw new KernelException(KernelExceptionType.GenerateChatResponseFailed, ex);
        }
    }

    /// <summary>
    /// 发送消息，并接收流式响应.
    /// </summary>
    /// <param name="message">用户消息.</param>
    /// <param name="userMsgHandler">用户消息添加处理.</param>
    /// <param name="streamHandler">流式消息处理.</param>
    /// <param name="ignoreUserMessage">是否不将传入消息作为用户消息添加到历史记录.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    public async Task<Models.App.Kernel.ChatMessage> SendMessageAsync(string message, Action<Models.App.Kernel.ChatMessage> userMsgHandler, Action<string> streamHandler, bool ignoreUserMessage = false, CancellationToken cancellationToken = default)
    {
        var chat = GetChatCore();
        var userMsg = new Models.App.Kernel.ChatMessage(ChatMessageRole.User, message);

        if (!ignoreUserMessage)
        {
            await ChatDataService.AddMessageAsync(userMsg, SessionId);
            userMsgHandler?.Invoke(userMsg);
        }

        var resMessage = string.Empty;
        try
        {
            var response = chat.GenerateMessageStreamAsync(GetHistory(), GetOpenAIRequestSettings(), cancellationToken);
            await foreach (var item in response)
            {
                resMessage += item;
                streamHandler?.Invoke(item);
            }

            resMessage = resMessage.Trim();
            if (string.IsNullOrEmpty(resMessage))
            {
                throw new KernelException(KernelExceptionType.EmptyChatResponse);
            }

            var assistantMessage = new Models.App.Kernel.ChatMessage(ChatMessageRole.Assistant, resMessage);
            await ChatDataService.AddMessageAsync(assistantMessage, SessionId);
            return assistantMessage;
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException)
            {
                throw new KernelException(KernelExceptionType.ChatResponseCancelled, ex);
            }

            throw new KernelException(KernelExceptionType.GenerateChatResponseFailed, ex);
        }
    }
}
