// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Dialogs;

/// <summary>
/// 绘图服务配置对话框.
/// </summary>
public sealed partial class InternalDrawConfigDialog : ContentDialog
{
    private readonly DrawType _drawType;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalDrawConfigDialog"/> class.
    /// </summary>
    public InternalDrawConfigDialog(InternalDrawServiceViewModel viewModel, DrawType drawType)
    {
        InitializeComponent();
        ViewModel = viewModel;
        _drawType = drawType;
        IsAzureDraw = drawType == DrawType.AzureDallE;
        IsOpenAIDraw = drawType == DrawType.OpenAIDallE;
        AppToolkit.ResetControlTheme(this);
        Loaded += OnLoaded;
    }

    private bool IsAzureDraw { get; }

    private bool IsOpenAIDraw { get; }

    private InternalDrawServiceViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ViewModel.InitializeCommand.Execute(_drawType);
}
