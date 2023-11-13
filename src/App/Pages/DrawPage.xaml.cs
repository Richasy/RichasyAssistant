// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 绘图页面.
/// </summary>
public sealed partial class DrawPage : DrawPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawPage"/> class.
    /// </summary>
    public DrawPage()
    {
        InitializeComponent();
        ViewModel = new DrawPageViewModel();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => SizeComboBox.SelectedIndex = (int)ViewModel.Size;

    private void OnSizeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.Size = (OpenAIImageSize)SizeComboBox.SelectedIndex;
    }
}

/// <summary>
/// 绘图页面基类.
/// </summary>
public abstract class DrawPageBase : PageBase<DrawPageViewModel>
{
}
