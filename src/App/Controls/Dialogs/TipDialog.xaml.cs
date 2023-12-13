// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.Controls.Dialogs;

/// <summary>
/// 提示对话框.
/// </summary>
public sealed partial class TipDialog : ContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TipDialog"/> class.
    /// </summary>
    public TipDialog(string content)
    {
        InitializeComponent();
        AppToolkit.ResetControlTheme(this);
        TipBlock.Text = content;
    }
}
