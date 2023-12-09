// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Kernel;
using Windows.Storage;
using Windows.System;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// 自定义内核项视图模型.
/// </summary>
public sealed partial class SlimServiceItemViewModel : DataViewModelBase<ServiceMetadata>
{
    private readonly Func<ServiceMetadata, Task> _deleteFunc;
    private readonly ServiceType _serviceType;

    [ObservableProperty]
    private string _path;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlimServiceItemViewModel"/> class.
    /// </summary>
    public SlimServiceItemViewModel(ServiceMetadata data, ServiceType type, Func<ServiceMetadata, Task> deleteFunc)
        : base(data)
    {
        _deleteFunc = deleteFunc;
        _serviceType = type;
        var libPath = SettingsToolkit.ReadLocalSetting(SettingNames.LibraryFolderPath, string.Empty);
        Path = System.IO.Path.Combine(libPath, "Extensions", type.ToString(), data.Id);
    }

    [RelayCommand]
    private async Task OpenFolderAsync()
    {
        if (Directory.Exists(Path))
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(Path);
            await Launcher.LaunchFolderAsync(folder);
        }
        else
        {
            AppViewModel.Instance.ShowTip(StringNames.FolderNotExist, InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        var dialog = new ContentDialog()
        {
            Title = ResourceToolkit.GetLocalizedString(StringNames.Tip),
            Content = ResourceToolkit.GetLocalizedString(StringNames.DeleteCustomKernelWarning),
            PrimaryButtonText = ResourceToolkit.GetLocalizedString(StringNames.Confirm),
            CloseButtonText = ResourceToolkit.GetLocalizedString(StringNames.Cancel),
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = AppViewModel.Instance.ActivatedWindow.Content.XamlRoot,
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await _deleteFunc(Data);
        }
    }
}
