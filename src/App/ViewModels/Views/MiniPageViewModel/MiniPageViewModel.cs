// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.ObjectModel;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Kernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 迷你页面视图模型.
/// </summary>
public sealed partial class MiniPageViewModel : ViewModelBase
{
    private MiniPageViewModel()
    {
        RecentSessions = new ObservableCollection<SessionItemViewModel>();
        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (AppViewModel.Instance.ChatClient == null)
        {
            AppViewModel.Instance.ChatClient = new ChatClient();
        }

        var chatClient = AppViewModel.Instance.ChatClient;
        await chatClient.InitializeLocalDatabaseAsync();

        var sessions = chatClient.GetSessions().Take(20);
        TryClear(RecentSessions);
        foreach (var item in sessions)
        {
            RecentSessions.Add(new SessionItemViewModel(item));
        }

        IsHistoryEmpty = RecentSessions.Count == 0;
    }
}
