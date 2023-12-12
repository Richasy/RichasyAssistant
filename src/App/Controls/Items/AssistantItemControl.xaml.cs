// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.Controls.Items;

/// <summary>
/// 助手项控件.
/// </summary>
public sealed partial class AssistantItemControl : AssistantItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssistantItemControl"/> class.
    /// </summary>
    public AssistantItemControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 点击.
    /// </summary>
    public event EventHandler Click;

    private void OnPanelClick(object sender, RoutedEventArgs e)
        => Click?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// 助手项控件.
/// </summary>
public abstract class AssistantItemControlBase : ReactiveUserControl<AssistantItemViewModel>
{
}
