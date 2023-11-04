// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 欢迎页面.
/// </summary>
public sealed partial class WelcomePage : WelcomePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WelcomePage"/> class.
    /// </summary>
    public WelcomePage()
    {
        InitializeComponent();
        ViewModel = WelcomePageViewModel.Instance;
    }
}

/// <summary>
/// 欢迎页面基类.
/// </summary>
public abstract class WelcomePageBase : PageBase<WelcomePageViewModel>
{
}
