// Copyright (c) Reader Copilot. All rights reserved.

using Microsoft.SemanticKernel.AI.ChatCompletion;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Kernel;

/// <summary>
/// 聊天客户端的聊天部分.
/// </summary>
public sealed partial class ChatClient
{
    /// <summary>
    /// 发送消息.
    /// </summary>
    /// <param name="message">消息.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>聊天信息.</returns>
    public async Task<ChatMessage> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        var session = GetCurrentSession();
        var chat = GetChatCore();
        session.AddMessage(new ChatMessage(ChatMessageRole.User, message));
        await UpdateCurrentSessionPayloadAsync();

        try
        {
            var response = await chat.GenerateMessageAsync(session.GetChatHistory(), session.GetOpenAIRequestSettings(), cancellationToken);

            if (string.IsNullOrEmpty(response))
            {
                throw new KernelException(KernelExceptionType.EmptyChatResponse);
            }

            var assistantMessage = new ChatMessage(ChatMessageRole.Assistant, response);
            session.AddMessage(assistantMessage);
            await UpdateCurrentSessionPayloadAsync();
            return assistantMessage;
        }
        catch (KernelException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new KernelException(KernelExceptionType.GenerateChatResponseFailed, ex);
        }
    }

    /// <summary>
    /// 发送消息，并接收流式响应.
    /// </summary>
    /// <param name="message">用户消息.</param>
    /// <param name="streamHandler">流式消息处理.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    public async Task<ChatMessage> SendMessageAsync(string message, Action<string> streamHandler, CancellationToken cancellationToken = default)
    {
        var session = GetCurrentSession();
        var chat = GetChatCore();
        session.AddMessage(new ChatMessage(ChatMessageRole.User, message));
        await UpdateCurrentSessionPayloadAsync();

        var resMessage = string.Empty;
        try
        {
            var response = chat.GenerateMessageStreamAsync(session.GetChatHistory(), session.GetOpenAIRequestSettings(), cancellationToken);
            await foreach (var item in response)
            {
                resMessage += item;
                streamHandler(item);
            }

            resMessage = resMessage.Trim();
            if (string.IsNullOrEmpty(resMessage))
            {
                throw new KernelException(KernelExceptionType.EmptyChatResponse);
            }

            var assistantMessage = new ChatMessage(ChatMessageRole.Assistant, resMessage);
            session.AddMessage(assistantMessage);
            await UpdateCurrentSessionPayloadAsync();
            return assistantMessage;
        }
        catch (Exception ex)
        {
            throw new KernelException(KernelExceptionType.GenerateChatResponseFailed, ex);
        }
    }
}
