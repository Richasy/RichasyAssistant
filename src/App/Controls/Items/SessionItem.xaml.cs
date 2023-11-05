// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.Controls.Items;

/// <summary>
/// 会话项.
/// </summary>
public sealed partial class SessionItem : SessionItemBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionItem"/> class.
    /// </summary>
    public SessionItem()
    {
        InitializeComponent();
    }

    private void OnPanelStateChanged(object sender, CardPanelStateChangedEventArgs e)
    {
        DateBlock.Visibility = e.IsPointerOver ? Visibility.Collapsed : Visibility.Visible;
        MoreButton.Visibility = e.IsPointerOver ? Visibility.Visible : Visibility.Collapsed;
    }
}

/// <summary>
/// 会话项基类.
/// </summary>
public abstract class SessionItemBase : ReactiveUserControl<SessionItemViewModel>
{
}
