// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Dialogs;

/// <summary>
/// 内部翻译服务配置对话框.
/// </summary>
public sealed partial class InternalTranslateConfigDialog : ContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalTranslateConfigDialog"/> class.
    /// </summary>
    public InternalTranslateConfigDialog(InternalTranslateServiceViewModel viewModel, bool isAzureTranslate)
    {
        InitializeComponent();
        ViewModel = viewModel;
        IsAzureTranslate = isAzureTranslate;
        AppToolkit.ResetControlTheme(this);
        Loaded += OnLoaded;
    }

    private bool IsAzureTranslate { get; }

    private InternalTranslateServiceViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(IsAzureTranslate);
}
