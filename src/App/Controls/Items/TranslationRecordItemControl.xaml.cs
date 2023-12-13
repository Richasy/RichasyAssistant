// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.Controls.Items;

/// <summary>
/// 翻译记录项控件.
/// </summary>
public sealed partial class TranslationRecordItemControl : TranslationRecordItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationRecordItemControl"/> class.
    /// </summary>
    public TranslationRecordItemControl()
        => InitializeComponent();

    /// <summary>
    /// 删除按钮点击事件.
    /// </summary>
    public event EventHandler<TranslationRecordItemViewModel> DeleteItemClick;

    private void OnDeleteItemClick(object sender, RoutedEventArgs e)
        => DeleteItemClick?.Invoke(this, ViewModel);
}

/// <summary>
/// 翻译记录项控件基类.
/// </summary>
public abstract class TranslationRecordItemControlBase : ReactiveUserControl<TranslationRecordItemViewModel>
{
}
