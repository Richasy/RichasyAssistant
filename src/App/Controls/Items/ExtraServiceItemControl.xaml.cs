// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.Controls.Items;

/// <summary>
/// 额外服务项.
/// </summary>
public sealed partial class ExtraServiceItemControl : ExtraServiceItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtraServiceItemControl"/> class.
    /// </summary>
    public ExtraServiceItemControl() => InitializeComponent();
}

/// <summary>
/// <see cref="ExtraServiceItemControl"/> 的基类.
/// </summary>
public abstract class ExtraServiceItemControlBase : ReactiveUserControl<ExtraServiceItemViewModel>
{
}
