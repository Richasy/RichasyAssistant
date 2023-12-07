// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Dialogs;

/// <summary>
/// 绘图服务配置对话框.
/// </summary>
public sealed partial class InternalDrawConfigDialog : ContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalDrawConfigDialog"/> class.
    /// </summary>
    public InternalDrawConfigDialog(InternalDrawServiceViewModel viewModel, bool isAzureDraw)
    {
        InitializeComponent();
        ViewModel = viewModel;
        IsAzureDraw = isAzureDraw;
        AppToolkit.ResetControlTheme(this);
        Loaded += OnLoaded;
    }

    private bool IsAzureDraw { get; }

    private InternalDrawServiceViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(IsAzureDraw);
}
