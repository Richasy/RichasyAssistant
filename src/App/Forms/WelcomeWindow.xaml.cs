// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Pages;

namespace RichasyAssistant.App.Forms;

/// <summary>
/// 欢迎窗口.
/// </summary>
public sealed partial class WelcomeWindow : WindowBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WelcomeWindow"/> class.
    /// </summary>
    public WelcomeWindow()
    {
        InitializeComponent();
        IsMaximizable = false;
        IsMinimizable = false;
        IsResizable = false;
        IsTitleBarVisible = false;
        Width = 700;
        Height = 460;

        this.CenterOnScreen();
        MainFrame.Navigate(typeof(WelcomePage));
    }
}
