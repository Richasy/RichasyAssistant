// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Args;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 迷你页面视图模型.
/// </summary>
public sealed partial class MiniPageViewModel : ViewModelBase
{
    private MiniPageViewModel()
    {
        RecentSessions = new ObservableCollection<ChatSessionItemViewModel>();
        Session = new ChatSessionViewModel();
        Translation = new TranslationViewModel();
        CheckState();
        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand);
        AttachExceptionHandlerToAsyncCommand(ShowError, InitializeCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (!IsInitialized)
        {
            await ChatDataService.InitializeAsync();
        }

        var sessions = ChatDataService.GetSessions().Take(20);
        TryClear(RecentSessions);
        foreach (var item in sessions)
        {
            RecentSessions.Add(new ChatSessionItemViewModel(item));
        }

        IsHistoryEmpty = RecentSessions.Count == 0;
    }

    [RelayCommand]
    private async Task CreateSessionAsync()
    {
        var kernel = await ChatKernel.CreateAsync();
        RecentSessions.Insert(0, new ChatSessionItemViewModel(kernel.Session));
        IsHistoryEmpty = RecentSessions.Count == 0;
        Session.InitializeCommand.Execute(kernel);
        IsInSession = true;
    }

    [RelayCommand]
    private void OpenSession(ChatSessionItemViewModel session)
    {
        var kernel = ChatKernel.Create(session.GetData().Id);
        Session.InitializeCommand.Execute(kernel);
        IsInSession = true;
    }

    [RelayCommand]
    private void OpenTranslation()
        => IsInTranslation = true;

    [RelayCommand]
    private async Task DeleteSessionAsync(ChatSessionItemViewModel session)
    {
        await ChatDataService.DeleteSessionAsync(session.GetData().Id);
        RecentSessions.Remove(session);
        IsHistoryEmpty = RecentSessions.Count == 0;
    }

    [RelayCommand]
    private void Back()
    {
        IsInSession = false;
        IsInTranslation = false;
        InitializeCommand.Execute(default);
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

            ErrorText = content;
        }
    }

    private void CheckState()
        => IsMainShown = !IsInSession && !IsInTranslation;

    partial void OnIsInSessionChanged(bool value)
        => CheckState();

    partial void OnIsInTranslationChanged(bool value)
    {
        CheckState();
        if (value && !Translation.IsInitialized)
        {
            Translation.InitializeCommand.Execute(default);
        }
    }
}
