﻿// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 聊天页面视图模型.
/// </summary>
public sealed partial class ChatPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPageViewModel"/> class.
    /// </summary>
    public ChatPageViewModel()
    {
        RecentSessions = new ObservableCollection<ChatSessionItemViewModel>();
        Assistants = new ObservableCollection<AssistantItemViewModel>();
        SessionDetail = new ChatSessionViewModel();
        AssistantDetail = new AssistantDetailViewModel(this);

        ListColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.ChatListColumnWidth, 300d);
        ListType = SettingsToolkit.ReadLocalSetting(SettingNames.ChatListType, ChatListType.Session);

        CheckChatListType();
        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand);
        AttachExceptionHandlerToAsyncCommand(ShowError, InitializeCommand);
        AttachExceptionHandlerToAsyncCommand(NotifyError, CreateSessionCommand, DeleteSessionCommand);
    }

    private static void NotifyError(Exception ex)
        => AppViewModel.Instance.ShowTip(ex.Message, InfoType.Error);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        await ChatDataService.InitializeAsync();
        var sessions = ChatDataService.GetSessions();
        var assistants = ChatDataService.GetAssistants();
        TryClear(RecentSessions);
        TryClear(Assistants);
        foreach (var item in sessions)
        {
            RecentSessions.Add(new ChatSessionItemViewModel(item));
        }

        foreach (var item in assistants)
        {
            Assistants.Add(new AssistantItemViewModel(item));
        }

        var lastOpenSession = SettingsToolkit.ReadLocalSetting(SettingNames.LastOpenSessionId, string.Empty);
        if (!string.IsNullOrEmpty(lastOpenSession) && RecentSessions.Any(p => p.Id == lastOpenSession))
        {
            var session = RecentSessions.FirstOrDefault(p => p.Id == lastOpenSession);
            OpenSession(session);
        }

        IsHistoryEmpty = RecentSessions.Count == 0;
        IsAssistantsEmpty = Assistants.Count == 0;
        IsDefaultChatAvailable = ChatKernel.IsDefaultKernelValid();
    }

    [RelayCommand]
    private async Task CreateSessionAsync(Assistant assistant = default)
    {
        var kernel = await ChatKernel.CreateAsync(assistant);
        var sessionVM = new ChatSessionItemViewModel(kernel.Session)
        {
            Title = ResourceToolkit.GetLocalizedString(StringNames.NewSession),
        };
        RecentSessions.Insert(0, sessionVM);
        IsHistoryEmpty = RecentSessions.Count == 0;

        CheckSelectedSession(sessionVM.Id);
        SessionDetail.InitializeCommand.Execute(sessionVM);
        ListType = ChatListType.Session;
    }

    [RelayCommand]
    private void OpenSession(ChatSessionItemViewModel session)
    {
        CheckSelectedSession(session.Id);
        SessionDetail.InitializeCommand.Execute(session);
    }

    [RelayCommand]
    private async Task DeleteSessionAsync(ChatSessionItemViewModel session)
    {
        await ChatDataService.DeleteSessionAsync(session.Id);
        _ = RecentSessions.Remove(session);
        IsHistoryEmpty = RecentSessions.Count == 0;

        if (SessionDetail.SessionId == session.Id)
        {
            SessionDetail.ResetCommand.Execute(default);
        }
    }

    [RelayCommand]
    private void CreateAssistant()
    {
        var assistant = new Assistant();
        AssistantDetail.InitializeCommand.Execute(assistant);
    }

    [RelayCommand]
    private void OpenAssistant(AssistantItemViewModel assistant)
    {
        CheckSelectedAssistant(assistant.Data.Id);
        AssistantDetail.InitializeCommand.Execute(assistant.Data);
    }

    [RelayCommand]
    private void RefreshAssistants()
    {
        TryClear(Assistants);
        var assistants = ChatDataService.GetAssistants();
        foreach (var item in assistants)
        {
            Assistants.Add(new AssistantItemViewModel(item));
        }

        IsAssistantsEmpty = Assistants.Count == 0;
    }

    private void ShowError(Exception ex)
    {
        if (ex is KernelException kex)
        {
            var content = kex.Type switch
            {
                KernelExceptionType.InvalidConfiguration => ResourceToolkit.GetLocalizedString(StringNames.ChatInvalidConfiguration),
                _ => ResourceToolkit.GetLocalizedString(StringNames.UnknownError),
            };

            InitializeErrorText = content;
        }
    }

    private void CheckChatListType()
    {
        ListTitle = ListType switch
        {
            ChatListType.Session => ResourceToolkit.GetLocalizedString(StringNames.Chat),
            ChatListType.Assistant => ResourceToolkit.GetLocalizedString(StringNames.Assistant),
            _ => throw new NotImplementedException(),
        };

        IsAssistantList = ListType == ChatListType.Assistant;
        IsSessionList = ListType == ChatListType.Session;
    }

    private void CheckSelectedSession(string sessionId)
    {
        foreach (var item in RecentSessions)
        {
            item.IsSelected = item.Id == sessionId;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LastOpenSessionId, sessionId);
    }

    private void CheckSelectedAssistant(string assistantId)
    {
        foreach (var item in Assistants)
        {
            item.IsSelected = item.Data.Id == assistantId;
        }
    }

    partial void OnListColumnWidthChanged(double value)
    {
        if (value >= 200)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.ChatListColumnWidth, value);
        }
    }

    partial void OnListTypeChanged(ChatListType value)
    {
        CheckChatListType();
        SettingsToolkit.WriteLocalSetting(SettingNames.ChatListType, value);
    }
}
