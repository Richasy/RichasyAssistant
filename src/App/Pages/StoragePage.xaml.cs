// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.Controls;
using RichasyAssistant.App.ViewModels.Views;

namespace RichasyAssistant.App.Pages;

/// <summary>
/// 文件存储页面.
/// </summary>
public sealed partial class StoragePage : StoragePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StoragePage"/> class.
    /// </summary>
    public StoragePage()
    {
        InitializeComponent();
        ViewModel = new StoragePageViewModel();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.InitializeCommand.Execute(default);

    private void OnSearchBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            e.Handled = true;

            // TODO: Search
        }
    }
}

/// <summary>
/// 文件存储页面的基类.
/// </summary>
public abstract class StoragePageBase : PageBase<StoragePageViewModel>
{
}
