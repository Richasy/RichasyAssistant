// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Libs.Kernel.DrawKernel;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 内部绘图服务视图模型.
/// </summary>
public sealed partial class InternalDrawServiceViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalDrawServiceViewModel"/> class.
    /// </summary>
    public InternalDrawServiceViewModel()
    {
        AzureDrawModelCollection = new ObservableCollection<Metadata>();
        AttachIsRunningToAsyncCommand(p => IsLoading = p, InitializeCommand, TryLoadAIModelSourceCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync(DrawType drawType)
    {
        if (drawType == DrawType.AzureDallE)
        {
            AzureImageKey = SettingsToolkit.ReadLocalSetting(SettingNames.AzureImageKey, string.Empty);
            AzureImageEndpoint = SettingsToolkit.ReadLocalSetting(SettingNames.AzureImageEndpoint, string.Empty);
        }
        else if(drawType == DrawType.OpenAIDallE)
        {
            OpenAIImageKey = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAIImageKey, string.Empty);
            OpenAICustomEndpoint = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAICustomEndpoint, string.Empty);
        }

        await TryLoadAIModelSourceAsync(drawType);
    }

    [RelayCommand]
    private async Task TryLoadAIModelSourceAsync(DrawType drawType)
    {
        if ((drawType == DrawType.AzureDallE && AzureDrawModelCollection.Count > 0)
            || (drawType == DrawType.AzureDallE && (string.IsNullOrEmpty(AzureImageKey) || string.IsNullOrEmpty(AzureImageEndpoint)))
            || (drawType == DrawType.OpenAIDallE && string.IsNullOrEmpty(OpenAIImageKey)))
        {
            return;
        }

        try
        {
            if (drawType == DrawType.AzureDallE)
            {
                GlobalSettings.Set(SettingNames.AzureImageKey, AzureImageKey);
                GlobalSettings.Set(SettingNames.AzureImageEndpoint, AzureImageEndpoint);
                var model = await DrawKernel.GetSupportModelsAsync(drawType);
                TryClear(AzureDrawModelCollection);
                AzureDrawModelCollection.Add(model);

                var localChatModel = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultAzureDrawModel, string.Empty);
                AzureDrawModel = string.IsNullOrEmpty(localChatModel)
                    ? AzureDrawModelCollection.FirstOrDefault()
                    : AzureDrawModelCollection.FirstOrDefault(p => p.Value.Equals(localChatModel));
            }
        }
        catch (Exception ex)
        {
            LogException(ex);
        }
    }
}
