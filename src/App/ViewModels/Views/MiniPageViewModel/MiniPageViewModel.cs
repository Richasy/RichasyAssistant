// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Kernel;
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
        if (AppViewModel.Instance.ChatClient == null)
        {
            AppViewModel.Instance.ChatClient = new ChatClient();
            var defaultKernel = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultKernel, KernelType.AzureOpenAI);
            if (defaultKernel == KernelType.AzureOpenAI)
            {
                AppViewModel.Instance.ChatClient.UseAzure();
            }
            else if (defaultKernel == KernelType.OpenAI)
            {
                AppViewModel.Instance.ChatClient.UseOpenAI();
            }

            AppViewModel.Instance.ChatClient.InitializeCorePlugins();
            await AppViewModel.Instance.ChatClient.InitializeLocalDatabaseAsync();
        }

        var chatClient = AppViewModel.Instance.ChatClient;
        var sessions = chatClient.GetSessions().Take(20);
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
        var chatClient = AppViewModel.Instance.ChatClient;
        var session = await chatClient.CreateNewSessionAsync();
        RecentSessions.Insert(0, new ChatSessionItemViewModel(session));
        IsHistoryEmpty = RecentSessions.Count == 0;
        Session.InitializeCommand.Execute(session);
        IsInSession = true;
    }

    [RelayCommand]
    private void OpenSession(ChatSessionItemViewModel session)
    {
        Session.InitializeCommand.Execute(session.GetData());
        IsInSession = true;
    }

    [RelayCommand]
    private void OpenTranslation()
        => IsInTranslation = true;

    [RelayCommand]
    private async Task DeleteSessionAsync(ChatSessionItemViewModel session)
    {
        var chatClient = AppViewModel.Instance.ChatClient;
        await chatClient.RemoveSessionAsync(session.GetData().Id);
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
