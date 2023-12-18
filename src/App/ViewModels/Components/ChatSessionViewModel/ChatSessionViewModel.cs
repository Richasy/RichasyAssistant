// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Specialized;
using System.Threading;
using Microsoft.UI.Dispatching;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;

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

    private static bool RunCustomKernelIfNotStarted(string kernelId)
    {
        var kernel = ChatDataService.GetExtraKernel(kernelId);
        if (kernel != null)
        {
            var service = ExtraServiceViewModel.Instance.CustomKernels.FirstOrDefault(p => p.Data.Equals(kernel));
            if (service == null || !service.IsRunning)
            {
                ExtraServiceViewModel.Instance.LaunchKernelServiceCommand.Execute(kernel);
                return false;
            }

            return true;
        }

        return false;
    }

    [RelayCommand]
    private async Task InitializeAsync(ChatSessionItemViewModel item)
    {
        if (_itemRef != null && IsInSettings)
        {
            await ExitSettingsAsync();
        }

        _itemRef = item;
        Type = item.Type;
        IsQuickChat = item.IsQuickChat;
        IsSingleChat = item.IsSingleChat;
        IsGroupChat = item.IsGroupChat;
        AssistantId = item.AssistantId;
        CheckMessageKernelRunning();
        TryClear(Messages);
        UserInput = string.Empty;
        IsChatAvailable = true;
        var sessionData = ChatDataService.GetSession(item.Id);
        ErrorText = string.Empty;
        Name = string.IsNullOrEmpty(sessionData.Title)
            ? ResourceToolkit.GetLocalizedString(StringNames.NoName)
            : sessionData.Title;

        if (sessionData.Messages?.Any() == true)
        {
            foreach (var message in sessionData.Messages.Distinct())
            {
                if (message.Role == ChatMessageRole.System)
                {
                    continue;
                }

                var vm = message.Role == ChatMessageRole.Assistant
                    ? new ChatMessageItemViewModel(
                            message,
                            RegenerateMessageAsync,
                            UpdateMessageAsync,
                            DeleteMessageAsync)
                    : new ChatMessageItemViewModel(
                        message,
                        default,
                        UpdateMessageAsync,
                        DeleteMessageAsync);
                Messages.Add(vm);
            }
        }

        try
        {
            _kernel = await ChatKernel.CreateAsync(item.Id);
        }
        catch (Exception ex)
        {
            LogException(ex);
            IsChatAvailable = false;
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

        await SendMessageInternalAsync(UserInput);
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
            _ = _dispatcherQueue.TryEnqueue(async () =>
            {
                TempMessage = string.Empty;
                var lastUserMsg = Messages.LastOrDefault(p => p.IsUser);
                if (lastUserMsg is not null)
                {
                    _ = Messages.Remove(lastUserMsg);
                    UserInput = lastUserMsg.Content;
                    await ChatDataService.DeleteMessageAsync(_kernel.SessionId, lastUserMsg.Data.Id);
                }
            });
        }

        RequestFocusInput?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task ClearMessageAsync()
    {
        var msgIds = Messages.Where(p => p.IsUser || p.IsAssistant).Select(p => p.Data.Id).ToArray();
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

        if (string.IsNullOrEmpty(Name))
        {
            Name = ResourceToolkit.GetLocalizedString(StringNames.NoName);
        }

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

    private async Task SendMessageInternalAsync(string msg, bool addUserMsg = true)
    {
        CancelMessage();

        if (!CheckMessageKernelRunning())
        {
            AppViewModel.Instance.ShowTip(StringNames.WaitServiceLaunching, InfoType.Warning);
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();

        ErrorText = string.Empty;
        UserInput = string.Empty;
        TempMessage = string.Empty;
        var isStream = SettingsToolkit.ReadLocalSetting(SettingNames.UseStreamOutput, true);
        if (isStream)
        {
            var response = await _kernel.SendMessageAsync(
                msg,
                userMsg =>
                {
                    _ = _dispatcherQueue.TryEnqueue(async () =>
                    {
                        Messages.Add(new ChatMessageItemViewModel(
                            userMsg,
                            default,
                            UpdateMessageAsync,
                            DeleteMessageAsync));
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
                    _ = _dispatcherQueue.TryEnqueue(() =>
                    {
                        TempMessage = text.TrimStart();
                    });
                },
                !addUserMsg,
                _cancellationTokenSource.Token);
            Messages.Add(new ChatMessageItemViewModel(
                response,
                RegenerateMessageAsync,
                UpdateMessageAsync,
                DeleteMessageAsync));
            _itemRef.Update();
            TempMessage = string.Empty;
        }
        else
        {
            var response = await _kernel.SendMessageAsync(
                msg,
                userMsg =>
                {
                    _ = _dispatcherQueue.TryEnqueue(async () =>
                    {
                        Messages.Add(new ChatMessageItemViewModel(
                            userMsg,
                            default,
                            UpdateMessageAsync,
                            DeleteMessageAsync));
                        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
                        _itemRef.Update();
                        if (Messages.Count <= 2)
                        {
                            await CheckAutoGenerateTitleAsync();
                        }
                    });
                },
                !addUserMsg,
                _cancellationTokenSource.Token);
            Messages.Add(new ChatMessageItemViewModel(
                response,
                RegenerateMessageAsync,
                UpdateMessageAsync,
                DeleteMessageAsync));
            _itemRef.Update();
        }

        RequestFocusInput?.Invoke(this, EventArgs.Empty);
        _cancellationTokenSource = null;
    }

    private async void RegenerateMessageAsync(ChatMessage msg)
    {
        IsResponding = true;
        try
        {
            var lastUserMsg = Messages.LastOrDefault(p => p.IsUser);
            var lastAssistantMsg = Messages.LastOrDefault(p => p.IsAssistant);
            Messages.Remove(lastAssistantMsg);
            await ChatDataService.DeleteMessageAsync(_kernel.SessionId, msg.Id);
            await SendMessageInternalAsync(lastUserMsg.Content, false);
        }
        catch (Exception ex)
        {
            LogException(ex);
        }

        IsResponding = false;
    }

    private async void UpdateMessageAsync(ChatMessage msg)
        => await ChatDataService.UpdateMessageAsync(msg, _kernel.SessionId);

    private async void DeleteMessageAsync(ChatMessage msg)
    {
        var source = Messages.FirstOrDefault(p => p.Data.Equals(msg));
        Messages.Remove(source);
        await ChatDataService.DeleteMessageAsync(_kernel.SessionId, msg.Id);
    }

    private void ResetLastMessage()
    {
        var lastIndex = Messages.Count - 1;
        for (var i = 0; i < Messages.Count; i++)
        {
            Messages[i].IsLastMessage = i == lastIndex;
        }
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

    private bool CheckMessageKernelRunning()
    {
        var session = ChatDataService.GetSession(_itemRef.Id);
        var canSend = true;
        if (session.Assistants.Count != 0)
        {
            foreach (var item in session.Assistants)
            {
                var assistant = ChatDataService.GetAssistant(item);
                if (assistant.Kernel == KernelType.Custom)
                {
                    var isRunning = RunCustomKernelIfNotStarted(assistant.Model);
                    if (!isRunning)
                    {
                        canSend = false;
                    }
                }
            }
        }
        else
        {
            var defaultService = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultKernel, KernelType.AzureOpenAI);
            if (defaultService == KernelType.Custom)
            {
                var kernelId = SettingsToolkit.ReadLocalSetting(SettingNames.CustomKernelId, string.Empty);
                canSend = RunCustomKernelIfNotStarted(kernelId);
            }
        }

        return canSend;
    }

    private void CheckChatEmpty()
        => IsChatEmpty = Messages.Count == 0;

    private void OnMessageCountChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        }

        ResetLastMessage();
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
