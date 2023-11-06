// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.Controls.Items;

/// <summary>
/// 会话项.
/// </summary>
public sealed partial class ChatSessionItem : ChatSessionItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionItem"/> class.
    /// </summary>
    public ChatSessionItem() => InitializeComponent();

    /// <summary>
    /// 点击.
    /// </summary>
    public event EventHandler Click;

    private void OnPanelClick(object sender, RoutedEventArgs e)
        => Click?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// 会话项基类.
/// </summary>
public abstract class ChatSessionItemBase : ReactiveUserControl<ChatSessionItemViewModel>
{
}
