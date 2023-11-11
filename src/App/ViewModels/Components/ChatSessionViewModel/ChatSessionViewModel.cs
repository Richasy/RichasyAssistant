// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Specialized;
using System.Threading;
using Microsoft.UI.Dispatching;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Args;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 聊天会话视图模型.
/// </summary>
public sealed partial class ChatSessionViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionViewModel"/> class.
    /// </summary>
    public ChatSessionViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        Messages = new ObservableCollection<ChatMessageItemViewModel>();
        Messages.CollectionChanged += OnMessageCountChanged;

        AttachIsRunningToAsyncCommand(
                p => IsResponding = p,
                SendMessageCommand);

        AttachExceptionHandlerToAsyncCommand(HandleException, SendMessageCommand);
    }

    [RelayCommand]
    private void Initialize(ChatKernel kernel)
    {
        TryClear(Messages);
        UserInput = string.Empty;
        _kernel = kernel;
        UserInput = string.Empty;
        ErrorText = string.Empty;
        Name = string.IsNullOrEmpty(_kernel.Session.Title)
            ? ResourceToolkit.GetLocalizedString(StringNames.NewSession)
            : _kernel.Session.Title;

        if (kernel.Session.Messages?.Any() == true)
        {
            foreach (var message in kernel.Session.Messages)
            {
                var vm = new ChatMessageItemViewModel(message);
                Messages.Add(vm);
            }
        }

        CheckChatEmpty();
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrEmpty(UserInput))
        {
            return;
        }

        CancelMessage();
        _cancellationTokenSource = new CancellationTokenSource();

        ErrorText = string.Empty;
        var msg = UserInput;
        UserInput = string.Empty;
        TempMessage = string.Empty;
        var isStream = SettingsToolkit.ReadLocalSetting(SettingNames.StreamOutput, true);
        if (isStream)
        {
            var response = await _kernel.SendMessageAsync(
                msg,
                userMsg =>
                {
                    _dispatcherQueue.TryEnqueue(async () =>
                    {
                        Messages.Add(new ChatMessageItemViewModel(userMsg));
                        if (Messages.Count <= 2)
                        {
                            var needGenerateTitle = SettingsToolkit.ReadLocalSetting(SettingNames.IsAutoGenerateSessionTitle, true)
                                && string.IsNullOrEmpty(_kernel.Session.Title);
                            if (needGenerateTitle)
                            {
                                var title = await _kernel.TryGenerateTitleAsync();
                                if (!string.IsNullOrEmpty(title))
                                {
                                    Name = title;
                                }
                            }
                        }
                    });
                },
                text =>
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        TempMessage += text;
                    });
                },
                _cancellationTokenSource.Token);
            Messages.Add(new ChatMessageItemViewModel(response));
            TempMessage = string.Empty;
        }
        else
        {
            var response = await _kernel.SendMessageAsync(
                msg,
                userMsg =>
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        Messages.Add(new ChatMessageItemViewModel(userMsg));
                        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
                    });
                },
                _cancellationTokenSource.Token);
            Messages.Add(new ChatMessageItemViewModel(response));
        }

        _cancellationTokenSource = null;
    }

    [RelayCommand]
    private void CancelMessage()
    {
        if (_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception)
            {
            }

            _cancellationTokenSource = default;
            _dispatcherQueue.TryEnqueue(async () =>
            {
                TempMessage = string.Empty;
                var lastUserMsg = Messages.LastOrDefault(p => p.IsUser);
                Messages.Remove(lastUserMsg);
                UserInput = lastUserMsg.Content;
                await ChatDataService.DeleteMessageAsync(_kernel.SessionId, lastUserMsg.GetData().Id);
            });
        }
    }

    [RelayCommand]
    private async Task ClearMessageAsync()
    {
        var msgIds = Messages.Where(p => p.IsUser || p.IsAssistant).Select(p => p.GetData().Id).ToArray();
        TryClear(Messages);
        UserInput = string.Empty;
        await ChatDataService.ClearMessageAsync(_kernel.SessionId);
    }

    private void HandleException(Exception ex)
    {
        CancelMessage();
        if (ex is KernelException kex)
        {
            switch (kex.Type)
            {
                case KernelExceptionType.EmptyChatResponse:
                    ErrorText = ResourceToolkit.GetLocalizedString(StringNames.EmptyResponse);
                    break;
                case KernelExceptionType.GenerateChatResponseFailed:
                    ErrorText = ResourceToolkit.GetLocalizedString(StringNames.GenerateResponseFailed);
                    break;
                case KernelExceptionType.ChatResponseCancelled:
                    ErrorText = ResourceToolkit.GetLocalizedString(StringNames.GenerateResponseCancelled);
                    break;
                default:
                    break;
            }
        }
    }

    private void CheckChatEmpty()
        => IsChatEmpty = Messages.Count == 0;

    private void OnMessageCountChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        }

        CheckChatEmpty();
    }
}
