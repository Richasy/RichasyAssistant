// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Input;
using RichasyAssistant.App.ViewModels.Components;
using Windows.UI.Core;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// 简化的聊天会话面板.
/// </summary>
public sealed partial class SlimChatSessionPanel : ChatSessionPanelBase
{
    /// <summary>
    /// <see cref="LeftElement"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty LeftElementProperty =
        DependencyProperty.Register(nameof(LeftElement), typeof(object), typeof(SlimChatSessionPanel), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="RightElement"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty RightElementProperty =
        DependencyProperty.Register(nameof(RightElement), typeof(object), typeof(SlimChatSessionPanel), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="SlimChatSessionPanel"/> class.
    /// </summary>
    public SlimChatSessionPanel()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 头部左侧插件.
    /// </summary>
    public object LeftElement
    {
        get => (object)GetValue(LeftElementProperty);
        set => SetValue(LeftElementProperty, value);
    }

    /// <summary>
    /// 头部右侧插件.
    /// </summary>
    public object RightElement
    {
        get => (object)GetValue(RightElementProperty);
        set => SetValue(RightElementProperty, value);
    }

    /// <summary>
    /// 重置焦点.
    /// </summary>
    public void ResetFocus()
        => InputBox.Focus(FocusState.Programmatic);

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
        if (IsLoaded)
        {
            InputBox.Focus(FocusState.Programmatic);
        }
    }

    private async void OnRequestScrollToBottomAsync(object sender, EventArgs e)
    {
        await Task.Delay(200);
        MessageViewer.ChangeView(0, MessageViewer.ScrollableHeight + MessageViewer.ActualHeight, default);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ResetFocus();

    private void OnInputBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down
                || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);
            if (!isShiftDown)
            {
                e.Handled = true;
                ViewModel.SendMessageCommand.Execute(default);
            }
        }
    }
}

/// <summary>
/// 聊天会话面板基类.
/// </summary>
public abstract class ChatSessionPanelBase : ReactiveUserControl<ChatSessionViewModel>
{
}
