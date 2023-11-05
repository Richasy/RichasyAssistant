// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 迷你页面.
/// </summary>
public sealed partial class MiniPage : MiniPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiniPage"/> class.
    /// </summary>
    public MiniPage()
    {
        InitializeComponent();
        ViewModel = MiniPageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 迷你页面基类.
/// </summary>
public abstract class MiniPageBase : PageBase<MiniPageViewModel>
{
}
