// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Media.Imaging;
using RichasyAssistant.App.Extensions;
using RichasyAssistant.App.ViewModels.Items;

namespace RichasyAssistant.App.Controls.Items;

/// <summary>
/// 存储项控件.
/// </summary>
public sealed class StorageItemControl : ReactiveControl<StorageItemViewModel>
{
    private ImageEx _logo;

    /// <summary>
    /// Initializes a new instance of the <see cref="StorageItemControl"/> class.
    /// </summary>
    public StorageItemControl()
        => DefaultStyleKey = typeof(StorageItemControl);

    internal override async void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is StorageItemViewModel && _logo != null)
        {
            await LoadIconAsync();
        }
    }

    /// <inheritdoc/>
    protected override async void OnApplyTemplate()
    {
        _logo = GetTemplateChild("Logo") as ImageEx;
        if (ViewModel != null)
        {
            await LoadIconAsync();
        }
    }

    private async Task LoadIconAsync()
    {
        if (ViewModel.IsFolder)
        {
            // 显示文件夹图标.
            _logo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Emoji/file_folder_3d.png"));
        }
        else
        {
            try
            {
                using var thumbnail = WindowsThumbnailProvider.GetThumbnail(ViewModel.Data.Path, 96, 96, ThumbnailOptions.None);
                using var ms = new MemoryStream();
                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                var image = new BitmapImage();
                _logo.Source = image;
                await image.SetSourceAsync(ms.AsRandomAccessStream());
            }
            catch
            {
                _logo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Emoji/no_entry_3d.png"));
            }
        }
    }
}
