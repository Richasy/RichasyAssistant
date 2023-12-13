// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using RichasyAssistant.App.Controls.Items;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// 聊天列表面板.
/// </summary>
public sealed partial class ChatListPanel : ChatListPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatListPanel"/> class.
    /// </summary>
    public ChatListPanel()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        DisplayPicker.SelectedIndex = (int)ViewModel.ListType;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => ViewModel.PropertyChanged -= OnViewModelPropertyChanged;

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.ListType))
        {
            DisplayPicker.SelectedIndex = (int)ViewModel.ListType;
        }
    }

    private void OnDisplayPickerSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.ListType = (ChatListType)DisplayPicker.SelectedIndex;
    }

    private void OnSessionItemClick(object sender, EventArgs e)
    {
        var vm = (sender as ChatSessionItemControl)?.ViewModel;
        ViewModel.OpenSessionCommand.Execute(vm);
    }

    private void OnDeleteSessionItemClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement).DataContext as ChatSessionItemViewModel;
        ViewModel.DeleteSessionCommand.Execute(vm);
    }

    private void OnAssistantItemClick(object sender, EventArgs e)
    {
        var vm = (sender as AssistantItemControl)?.ViewModel;
        ViewModel.OpenAssistantCommand.Execute(vm);
    }

    private void OnAssistantChatButtonClick(object sender, EventArgs e)
    {
        var vm = (sender as AssistantItemControl)?.ViewModel;
        ViewModel.CreateSessionCommand.Execute(vm.Data);
    }
}

/// <summary>
/// 聊天列表面板基类.
/// </summary>
public abstract class ChatListPanelBase : ReactiveUserControl<ChatPageViewModel>
{
}
