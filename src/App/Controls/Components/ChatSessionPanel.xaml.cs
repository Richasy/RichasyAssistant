// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Input;
using RichasyAssistant.App.ViewModels.Components;
using Windows.UI.Core;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// 简化的聊天会话面板.
/// </summary>
public sealed partial class ChatSessionPanel : ChatSessionPanelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionPanel"/> class.
    /// </summary>
    public ChatSessionPanel()
    {
        InitializeComponent();
        _ = RegisterPropertyChangedCallback(VisibilityProperty, OnVisibilityChanged);
        Loaded += OnLoadedAsync;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// 重置焦点.
    /// </summary>
    public void ResetFocus()
        => InputBox?.Focus(FocusState.Programmatic);

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ChatSessionViewModel oldVM)
        {
            oldVM.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        }

        if (e.NewValue == null)
        {
            return;
        }

        var vm = e.NewValue as ChatSessionViewModel;
        vm.RequestScrollToBottom += OnRequestScrollToBottomAsync;
        vm.RequestFocusInput += OnRequestFocusInput;
        if (IsLoaded)
        {
            ResetFocus();
        }
    }

    private void OnRequestFocusInput(object sender, EventArgs e)
        => ResetFocus();

    private async void OnRequestScrollToBottomAsync(object sender, EventArgs e)
        => await ScrollToBottomAsync();

    private void OnVisibilityChanged(DependencyObject sender, DependencyProperty dp)
        => ResetFocus();

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        ResetFocus();
        await ScrollToBottomAsync();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.RequestFocusInput -= OnRequestFocusInput;
        ViewModel.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
    }

    private void OnInputBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState is(CoreVirtualKeyStates.Down
                or CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);
            if (!isShiftDown)
            {
                e.Handled = true;
                ViewModel.SendMessageCommand.Execute(default);
            }
        }
    }

    private async Task ScrollToBottomAsync()
    {
        await Task.Delay(200);
        _ = MessageViewer.ChangeView(0, MessageViewer.ScrollableHeight + MessageViewer.ActualHeight + MessageViewer.VerticalOffset, default);
    }
}

/// <summary>
/// 聊天会话面板基类.
/// </summary>
public abstract class ChatSessionPanelBase : ReactiveUserControl<ChatSessionViewModel>
{
}
