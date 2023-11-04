// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls;

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
    }
}

/// <summary>
/// 迷你页面基类.
/// </summary>
public abstract class MiniPageBase : PageBase
{
}
