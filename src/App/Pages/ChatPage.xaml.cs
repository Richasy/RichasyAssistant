// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls;
using RichasyAssistant.App.Controls.Items;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 对话页面.
/// </summary>
public sealed partial class ChatPage : ChatPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatPage"/> class.
    /// </summary>
    public ChatPage()
    {
        InitializeComponent();
        ViewModel = new ChatPageViewModel();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);
}

/// <summary>
/// 对话页面基类.
/// </summary>
public abstract class ChatPageBase : PageBase<ChatPageViewModel>
{
}
