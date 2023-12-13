// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Dialogs;

/// <summary>
/// 内核配置对话框.
/// </summary>
public sealed partial class InternalKernelConfigDialog : ContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalKernelConfigDialog"/> class.
    /// </summary>
    public InternalKernelConfigDialog(InternalKernelViewModel viewModel, bool isAzureOpenAI)
    {
        InitializeComponent();
        ViewModel = viewModel;
        IsAzureOpenAI = isAzureOpenAI;
        AppToolkit.ResetControlTheme(this);
        Loaded += OnLoaded;
    }

    private bool IsAzureOpenAI { get; }

    private InternalKernelViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(IsAzureOpenAI);
}
