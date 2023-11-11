// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.Controls.Items;

/// <summary>
/// 聊天消息条目.
/// </summary>
public sealed partial class ChatMessageItemControl : ChatMessageItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemControl"/> class.
    /// </summary>
    public ChatMessageItemControl() => InitializeComponent();

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ChatMessageItemViewModel vm)
        {
            if (vm.IsUser)
            {
                VisualStateManager.GoToState(this, nameof(MyState), false);
            }
            else
            {
                VisualStateManager.GoToState(this, nameof(AssistantState), false);
            }
        }
    }

    private void OnRootCardStateChanged(object sender, CardPanelStateChangedEventArgs e)
    {
        if (e.IsPointerOver)
        {
            // TODO: 显示操作条.
        }
    }
}

/// <summary>
/// 聊天消息基类.
/// </summary>
public abstract class ChatMessageItemControlBase : ReactiveUserControl<ChatMessageItemViewModel>
{
}
