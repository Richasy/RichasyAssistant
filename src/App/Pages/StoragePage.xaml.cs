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
    protected override async void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(default);
        LayoutPicker.SelectedIndex = ViewModel.IsGridLayout ? 0 : 1;
        SortTypeComboBox.SelectedIndex = (int)ViewModel.SortType;
        await Task.Delay(500);
        if (ViewModel.IsEverythingAvailable)
        {
            SearchBox.Focus(FocusState.Programmatic);
        }
    }

    private void OnSearchSubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        => ViewModel.SearchCommand.Execute(default);

    private void OnLayoutPickerSelectionChanged(object sender, SelectionChangedEventArgs e)
        => ViewModel.IsGridLayout = LayoutPicker.SelectedIndex == 0;

    private void OnSortTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!ViewModel.IsEverythingAvailable)
        {
            return;
        }

        ViewModel.SortType = (StorageSortType)SortTypeComboBox.SelectedIndex;
    }
}

/// <summary>
/// 文件存储页面的基类.
/// </summary>
public abstract class StoragePageBase : PageBase<StoragePageViewModel>
{
}
