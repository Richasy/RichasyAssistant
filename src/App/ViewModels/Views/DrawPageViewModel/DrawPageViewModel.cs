// Copyright (c) Richasy Assistant. All rights reserved.

using System.Threading;
using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Kernel.DrawKernel;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Args;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 绘图页面视图模型.
/// </summary>
public sealed partial class DrawPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawPageViewModel"/> class.
    /// </summary>
    public DrawPageViewModel()
    {
        Size = SettingsToolkit.ReadLocalSetting(SettingNames.DrawImageSize, OpenAIImageSize.Medium);
        HistoryColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.DrawHistoryColumnWidth, 320d);
        History = new ObservableCollection<AiImageItemViewModel>();

        AttachIsRunningToAsyncCommand(p => IsGenerating = p, DrawCommand);
        AttachExceptionHandlerToAsyncCommand(ShowError, InitializeCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        _kernel = DrawKernel.Create();
        await DrawDataService.InitializeAsync();

        TryClear(History);
        IsAvailable = _kernel.IsConfigValid;

        var defaultType = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultImage, DrawType.AzureDallE);
        var identify = defaultType switch
        {
            DrawType.OpenAIDallE => "OpenAI DALL·E",
            DrawType.AzureDallE => "Azure DALL·E",
            _ => string.Empty
        };

        PoweredBy = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PoweredBy), identify);
        LoadHistory(true);
    }

    [RelayCommand]
    private async Task DrawAsync()
    {
        if (string.IsNullOrEmpty(Prompt))
        {
            return;
        }

        Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var image = await _kernel.DrawAsync(Prompt, Size, _cancellationTokenSource.Token);
        var vm = new AiImageItemViewModel(image);
        CurrentImage = vm;
        if (History.Count > 100)
        {
            History.RemoveAt(History.Count - 1);
        }

        History.Insert(0, vm);
        CheckHistoryCount();
        _cancellationTokenSource = null;
    }

    [RelayCommand]
    private void Cancel()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    [RelayCommand]
    private async Task DeleteImageAsync(AiImageItemViewModel vm)
    {
        if (CurrentImage == vm)
        {
            CurrentImage = null;
        }

        History.Remove(vm);
        await DrawDataService.RemoveImageAsync(vm.Data.Id);
        CheckHistoryCount();
    }

    [RelayCommand]
    private void SetImage(AiImageItemViewModel vm)
    {
        CurrentImage = vm;
        Prompt = vm.Data.Prompt;
    }

    [RelayCommand]
    private void LoadHistory(bool force = false)
    {
        if (!force && !HistoryHasMore)
        {
            return;
        }

        var hasMore = DrawDataService.HasMoreHistory(HistoryPageIndex);
        if (hasMore)
        {
            var list = DrawDataService.GetHistory(HistoryPageIndex);
            foreach (var item in list)
            {
                History.Add(new AiImageItemViewModel(item));
            }

            HistoryPageIndex++;
            HistoryHasMore = DrawDataService.HasMoreHistory(HistoryPageIndex);
        }

        CheckHistoryCount();
    }

    [RelayCommand]
    private async Task ClearHistoryAsync()
    {
        await DrawDataService.ClearHistoryAsync();
        TryClear(History);
        HistoryHasMore = false;
        HistoryPageIndex = 0;
        IsHistoryEmpty = true;
    }

    private void ShowError(Exception ex)
    {
        ErrorText = ex is KernelException kex
            ? kex.Type switch
            {
                KernelExceptionType.InvalidConfiguration => ResourceToolkit.GetLocalizedString(StringNames.ChatInvalidConfiguration),
                _ => ResourceToolkit.GetLocalizedString(StringNames.SomethingWrong) + $"\n{kex.Type}",
            }
            : ex.Message;

        LogException(ex);
    }

    private void CheckHistoryCount()
        => IsHistoryEmpty = History.Count == 0;

    partial void OnSizeChanged(OpenAIImageSize value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DrawImageSize, value);

    partial void OnHistoryColumnWidthChanged(double value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DrawHistoryColumnWidth, value);
}
