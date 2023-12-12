// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using RichasyAssistant.App.ViewModels.Components;
using Windows.Storage;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// 助理面板.
/// </summary>
public sealed partial class AssistantDetailPanel : AssistantDetailPanelBase
{
    private MemoryStream _avatarStream;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssistantDetailPanel"/> class.
    /// </summary>
    public AssistantDetailPanel()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is AssistantDetailViewModel vm)
        {
            vm.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => Avatar.Id = ViewModel.Source?.Id ?? string.Empty;

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Source))
        {
            Avatar.Id = ViewModel.Source?.Id ?? string.Empty;
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        ReleaseAvatarStream();
    }

    private async void OnAvatarTappedAsync(object sender, TappedRoutedEventArgs e)
    {
        var fileObj = await FileToolkit.PickFileAsync(".png,.jpg,.bmp", AppViewModel.Instance.ActivatedWindow);
        if (fileObj is StorageFile file)
        {
            // 截图.
            ViewModel.IsImageCropper = true;
            await Cropper.LoadImageFromFile(file);
        }
    }

    private async void OnImageCropperConfirmButtonClickAsync(object sender, RoutedEventArgs e)
    {
        ReleaseAvatarStream();
        var ms = new MemoryStream();
        await Cropper.SaveAsync(ms.AsRandomAccessStream(), CommunityToolkit.WinUI.Controls.BitmapFileFormat.Png, true);
        ms.Seek(0, SeekOrigin.Begin);
        _avatarStream = ms;
        var bitmap = new BitmapImage();
        await bitmap.SetSourceAsync(_avatarStream.AsRandomAccessStream());
        Avatar.SetSource(bitmap);
        ViewModel.IsImageCropper = false;
    }

    private void OnImageCropperCancelButtonClickAsync(object sender, RoutedEventArgs e)
    {
        ReleaseAvatarStream();
        ViewModel.IsImageCropper = false;
    }

    private async void OnSaveButtonClickAsync(object sender, RoutedEventArgs e)
    {
        SaveButton.IsEnabled = false;
        await ViewModel.SaveCommand.ExecuteAsync(_avatarStream);
        ReleaseAvatarStream();
        SaveButton.IsEnabled = true;
    }

    private void ReleaseAvatarStream()
    {
        _avatarStream?.Dispose();
        _avatarStream = null;
    }
}

/// <summary>
/// 助理面板基类.
/// </summary>
public abstract class AssistantDetailPanelBase : ReactiveUserControl<AssistantDetailViewModel>
{
}
