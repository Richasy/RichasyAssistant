// Copyright (c) Richasy Assistant. All rights reserved.

using System.Threading;
using RichasyAssistant.Libs.Kernel.DrawKernel;

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
        _kernel = DrawKernel.Create();
        Size = SettingsToolkit.ReadLocalSetting(SettingNames.DrawImageSize, OpenAIImageSize.Medium);

        var defaultType = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultImage, DrawType.AzureDallE);
        var identify = defaultType switch
        {
            DrawType.OpenAIDallE => "OpenAI DALL·E",
            DrawType.AzureDallE => "Azure DALL·E",
            _ => string.Empty
        };

        PoweredBy = string.Format(ResourceToolkit.GetLocalizedString(StringNames.PoweredBy), identify);

        AttachIsRunningToAsyncCommand(p => IsGenerating = p, DrawCommand);
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
        var imagePath = await _kernel.DrawAsync(Prompt, Size, _cancellationTokenSource.Token);
        if (!string.IsNullOrEmpty(imagePath))
        {
            ImagePath = imagePath;
        }

        _cancellationTokenSource = null;
    }

    [RelayCommand]
    private void Cancel()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    partial void OnSizeChanged(OpenAIImageSize value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DrawImageSize, value);
}
