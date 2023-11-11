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
    private async Task InitializeAsync(ChatSessionItemViewModel item)
    {
        if (_itemRef != null && IsInSettings)
        {
            await ExitSettingsAsync();
        }

        _itemRef = item;
        TryClear(Messages);
        UserInput = string.Empty;
        _kernel = ChatKernel.Create(item.Id);
        UserInput = string.Empty;
        ErrorText = string.Empty;
        Name = string.IsNullOrEmpty(_kernel.Session.Title)
            ? ResourceToolkit.GetLocalizedString(StringNames.NoName)
            : _kernel.Session.Title;

        if (_kernel.Session.Messages?.Any() == true)
        {
            foreach (var message in _kernel.Session.Messages)
            {
                var vm = new ChatMessageItemViewModel(message);
                Messages.Add(vm);
            }
        }

        IsReady = true;
        CheckChatEmpty();
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
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
                        _itemRef.Update();
                        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
                        if (Messages.Count <= 2)
                        {
                            await CheckAutoGenerateTitleAsync();
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
            _itemRef.Update();
            TempMessage = string.Empty;
        }
        else
        {
            var response = await _kernel.SendMessageAsync(
                msg,
                userMsg =>
                {
                    _dispatcherQueue.TryEnqueue(async () =>
                    {
                        Messages.Add(new ChatMessageItemViewModel(userMsg));
                        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
                        _itemRef.Update();
                        if (Messages.Count <= 2)
                        {
                            await CheckAutoGenerateTitleAsync();
                        }
                    });
                },
                _cancellationTokenSource.Token);
            Messages.Add(new ChatMessageItemViewModel(response));
            _itemRef.Update();
        }

        RequestFocusInput?.Invoke(this, EventArgs.Empty);
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

        RequestFocusInput?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task ClearMessageAsync()
    {
        var msgIds = Messages.Where(p => p.IsUser || p.IsAssistant).Select(p => p.GetData().Id).ToArray();
        TryClear(Messages);
        UserInput = string.Empty;
        await ChatDataService.ClearMessageAsync(_kernel.SessionId);
        _itemRef.Update();
        RequestFocusInput?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void EnterSettings()
    {
        var session = ChatDataService.GetSession(_itemRef.Id);
        var options = session.Options;
        Name = session.Title;
        MaxTokens = options.MaxResponseTokens;
        TopP = options.TopP;
        Temperature = options.Temperature;
        FrequencyPenalty = options.FrequencyPenalty;
        PresencePenalty = options.PresencePenalty;
        IsInSettings = true;
    }

    [RelayCommand]
    private async Task ExitSettingsAsync()
    {
        var session = ChatDataService.GetSession(_itemRef.Id);
        var options = session.Options;
        options.MaxResponseTokens = Convert.ToInt32(MaxTokens);
        options.Temperature = Temperature;
        options.TopP = TopP;
        options.FrequencyPenalty = FrequencyPenalty;
        options.PresencePenalty = PresencePenalty;

        session.Title = Name;
        await ChatDataService.AddOrUpdateSessionAsync(session);

        _itemRef.Update();
        Name = string.IsNullOrEmpty(_kernel.Session.Title)
            ? ResourceToolkit.GetLocalizedString(StringNames.NoName)
            : _kernel.Session.Title;
        IsInSettings = false;
    }

    [RelayCommand]
    private void Reset()
    {
        _itemRef = null;
        _kernel = null;
        IsReady = false;
        IsChatEmpty = true;
        IsInSettings = false;
        ErrorText = string.Empty;
        UserInput = string.Empty;
        TempMessage = string.Empty;
        Name = string.Empty;
        MaxTokens = 0;
        TopP = 0;
        Temperature = 0;
        FrequencyPenalty = 0;
        PresencePenalty = 0;
        TryClear(Messages);
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

    private async Task CheckAutoGenerateTitleAsync()
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

    partial void OnNameChanged(string value)
    {
        if (_itemRef != null)
        {
            _itemRef.Title = value;
        }
    }
}
