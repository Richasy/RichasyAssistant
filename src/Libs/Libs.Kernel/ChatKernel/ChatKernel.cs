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
        _coreFunctions = new Dictionary<string, KernelFunction>();
    }

    /// <summary>
    /// 发送消息.
    /// </summary>
    /// <param name="message">消息.</param>
    /// <param name="userMsgHandler">用户消息添加处理.</param>
    /// <param name="ignoreUserMessage">是否不将传入消息作为用户消息添加到历史记录.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>聊天信息.</returns>
    public async Task<ChatMessage> SendMessageAsync(string message, Action<ChatMessage> userMsgHandler, bool ignoreUserMessage = false, CancellationToken cancellationToken = default)
    {
        var chat = GetChatCore();
        var userMsg = new ChatMessage(ChatMessageRole.User, message);
        if (!ignoreUserMessage)
        {
            await ChatDataService.AddMessageAsync(userMsg, SessionId);
            userMsgHandler?.Invoke(userMsg);
        }

        try
        {
            var response = await chat.GetChatMessageContentAsync(GetHistory(), GetOpenAIRequestSettings(), cancellationToken: cancellationToken);

            if (string.IsNullOrEmpty(response))
            {
                throw new Models.App.Args.KernelException(KernelExceptionType.EmptyChatResponse);
            }

            var assistantMessage = new ChatMessage(ChatMessageRole.Assistant, response);
            ApplyAssistantIdIfExist(assistantMessage);
            await ChatDataService.AddMessageAsync(assistantMessage, SessionId);
            return assistantMessage;
        }
        catch (Models.App.Args.KernelException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException)
            {
                throw new Models.App.Args.KernelException(KernelExceptionType.ChatResponseCancelled, ex);
            }

            throw new Models.App.Args.KernelException(KernelExceptionType.GenerateChatResponseFailed, ex);
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
    public async Task<ChatMessage> SendMessageAsync(string message, Action<ChatMessage> userMsgHandler, Action<string> streamHandler, bool ignoreUserMessage = false, CancellationToken cancellationToken = default)
    {
        var chat = GetChatCore();
        var userMsg = new ChatMessage(ChatMessageRole.User, message);

        if (!ignoreUserMessage)
        {
            await ChatDataService.AddMessageAsync(userMsg, SessionId);
            userMsgHandler?.Invoke(userMsg);
        }

        var resMessage = string.Empty;
        try
        {
            await Task.Run(async () =>
            {
                var response = chat.GetStreamingChatMessageContentsAsync(GetHistory(), GetOpenAIRequestSettings(), cancellationToken: cancellationToken);
                await foreach (var item in response)
                {
                    resMessage += item;
                    streamHandler?.Invoke(resMessage);
                }
            });

            resMessage = resMessage.Trim();
            if (string.IsNullOrEmpty(resMessage))
            {
                throw new Models.App.Args.KernelException(KernelExceptionType.EmptyChatResponse);
            }

            var assistantMessage = new ChatMessage(ChatMessageRole.Assistant, resMessage);
            ApplyAssistantIdIfExist(assistantMessage);
            await ChatDataService.AddMessageAsync(assistantMessage, SessionId);
            return assistantMessage;
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException)
            {
                throw new Models.App.Args.KernelException(KernelExceptionType.ChatResponseCancelled, ex);
            }

            throw new Models.App.Args.KernelException(KernelExceptionType.GenerateChatResponseFailed, ex);
        }
    }

    private void ApplyAssistantIdIfExist(ChatMessage message)
    {
        if (Session.Assistants.Count == 1)
        {
            message.AssistantId = Session.Assistants[0];
        }
    }
}
