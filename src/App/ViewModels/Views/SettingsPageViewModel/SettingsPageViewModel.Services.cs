// Copyright (c) Richasy Assistant. All rights reserved.

using System.Text.Json;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 设置页视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private async Task InitializeChatKernelsAsync()
    {
        TryClear(ChatKernels);
        ChatKernels.Add(new ServiceMetadata(AzureOpenAIId, "Azure Open AI"));
        ChatKernels.Add(new ServiceMetadata(OpenAIId, "Open AI"));

        var extraServicesPath = Path.Combine(LibraryPath, ExtraKernelFileName);
        if (File.Exists(extraServicesPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(extraServicesPath);
                var data = JsonSerializer.Deserialize<List<ServiceMetadata>>(json);
                foreach (var metadata in data)
                {
                    ChatKernels.Add(metadata);
                }
            }
            catch (Exception ex)
            {
                LogException(new Exception("Extra kernels load error", ex));
            }
        }

        var defaultKernel = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultKernel, KernelType.AzureOpenAI);
        if (defaultKernel == KernelType.AzureOpenAI)
        {
            ChatKernel = ChatKernels.First();
        }
        else if (defaultKernel == KernelType.OpenAI)
        {
            ChatKernel = ChatKernels[1];
        }
        else
        {
            var chatKernelId = SettingsToolkit.ReadLocalSetting(SettingNames.CustomKernelId, string.Empty);
            if (!string.IsNullOrEmpty(chatKernelId))
            {
                ChatKernel = ChatKernels.FirstOrDefault(p => p.Id == chatKernelId);
            }
        }

        if (ChatKernel == null)
        {
            AppViewModel.Instance.ShowTip(StringNames.ExtraKernelLoadFailed, InfoType.Warning);
        }
    }

    private async Task InitializeDrawServicesAsync()
    {
        TryClear(DrawServices);
        DrawServices.Add(new ServiceMetadata(AzureDrawId, "Azure DALL·E"));
        DrawServices.Add(new ServiceMetadata(OpenAIDrawId, "Open AI DALL·E"));

        var extraServicesPath = Path.Combine(LibraryPath, ExtraDrawFileName);
        if (File.Exists(extraServicesPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(extraServicesPath);
                var data = JsonSerializer.Deserialize<List<ServiceMetadata>>(json);
                foreach (var metadata in data)
                {
                    DrawServices.Add(metadata);
                }
            }
            catch (Exception ex)
            {
                LogException(new Exception("Extra drawings load error", ex));
            }
        }

        var defaultDraw = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultDrawService, DrawType.AzureDallE);
        if (defaultDraw == DrawType.AzureDallE)
        {
            DrawService = DrawServices.First();
        }
        else if (defaultDraw == DrawType.OpenAIDallE)
        {
            DrawService = DrawServices[1];
        }
        else
        {
            var drawServiceId = SettingsToolkit.ReadLocalSetting(SettingNames.CustomDrawId, string.Empty);
            if (!string.IsNullOrEmpty(drawServiceId))
            {
                DrawService = DrawServices.FirstOrDefault(p => p.Id == drawServiceId);
            }
        }

        if (DrawService == null)
        {
            AppViewModel.Instance.ShowTip(StringNames.ExtraDrawLoadFailed, InfoType.Warning);
        }
    }

    private async Task InitializeTranslateServicesAsync()
    {
        TryClear(TranslateServices);
        TranslateServices.Add(new ServiceMetadata(AzureTranslateId, ResourceToolkit.GetLocalizedString(StringNames.AzureTranslate)));
        TranslateServices.Add(new ServiceMetadata(BaiduTranslateId, ResourceToolkit.GetLocalizedString(StringNames.BaiduTranslate)));

        var extraServicesPath = Path.Combine(LibraryPath, ExtraTranslateFileName);
        if (File.Exists(extraServicesPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(extraServicesPath);
                var data = JsonSerializer.Deserialize<List<ServiceMetadata>>(json);
                foreach (var metadata in data)
                {
                    TranslateServices.Add(metadata);
                }
            }
            catch (Exception ex)
            {
                LogException(new Exception("Extra translate services load error", ex));
            }
        }

        var defaultTranslate = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultTranslate, TranslateType.Azure);
        if (defaultTranslate == TranslateType.Azure)
        {
            TranslateService = TranslateServices.First();
        }
        else if (defaultTranslate == TranslateType.Baidu)
        {
            TranslateService = TranslateServices[1];
        }
        else
        {
            var translateServiceId = SettingsToolkit.ReadLocalSetting(SettingNames.CustomTranslateId, string.Empty);
            if (!string.IsNullOrEmpty(translateServiceId))
            {
                TranslateService = TranslateServices.FirstOrDefault(p => p.Id == translateServiceId);
            }
        }

        if (TranslateService == null)
        {
            AppViewModel.Instance.ShowTip(StringNames.ExtraTranslateLoadFailed, InfoType.Warning);
        }
    }

    private async Task InitializeSpeechServicesAsync()
    {
        TryClear(SpeechServices);
        SpeechServices.Add(new ServiceMetadata(AzureSpeechId, ResourceToolkit.GetLocalizedString(StringNames.AzureSpeech)));
        SpeechServices.Add(new ServiceMetadata(AzureWhisperId, ResourceToolkit.GetLocalizedString(StringNames.AzureWhisper)));
        SpeechServices.Add(new ServiceMetadata(OpenAIWhisperId, ResourceToolkit.GetLocalizedString(StringNames.OpenAIWhisper)));

        var extraServicesPath = Path.Combine(LibraryPath, ExtraSpeechFileName);
        if (File.Exists(extraServicesPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(extraServicesPath);
                var data = JsonSerializer.Deserialize<List<ServiceMetadata>>(json);
                foreach (var metadata in data)
                {
                    SpeechServices.Add(metadata);
                }
            }
            catch (Exception ex)
            {
                LogException(new Exception("Extra Speech services load error", ex));
            }
        }

        var defaultSpeech = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultSpeech, SpeechType.Azure);
        if (defaultSpeech == SpeechType.Azure)
        {
            SpeechService = SpeechServices.First();
        }
        else if (defaultSpeech == SpeechType.AzureWhisper)
        {
            SpeechService = SpeechServices[1];
        }
        else if (defaultSpeech == SpeechType.OpenAIWhisper)
        {
            SpeechService = SpeechServices[2];
        }
        else
        {
            var speechServiceId = SettingsToolkit.ReadLocalSetting(SettingNames.CustomSpeechId, string.Empty);
            if (!string.IsNullOrEmpty(speechServiceId))
            {
                SpeechService = SpeechServices.FirstOrDefault(p => p.Id == speechServiceId);
            }
        }

        if (SpeechService == null)
        {
            AppViewModel.Instance.ShowTip(StringNames.ExtraSpeechLoadFailed, InfoType.Warning);
        }
    }

    partial void OnChatKernelChanged(ServiceMetadata value)
    {
        if (value == null)
        {
            return;
        }

        if (value.Id == AzureOpenAIId)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultKernel, KernelType.AzureOpenAI);
        }
        else if (value.Id == OpenAIId)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultKernel, KernelType.OpenAI);
        }
        else
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultKernel, KernelType.Custom);
            SettingsToolkit.WriteLocalSetting(SettingNames.CustomKernelId, value.Id);
        }

        AppViewModel.ResetGlobalSettings();
    }

    partial void OnDrawServiceChanged(ServiceMetadata value)
    {
        if (value == null)
        {
            return;
        }

        if (value.Id == AzureDrawId)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultDrawService, DrawType.AzureDallE);
        }
        else if (value.Id == OpenAIDrawId)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultDrawService, DrawType.OpenAIDallE);
        }
        else
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultDrawService, DrawType.Custom);
            SettingsToolkit.WriteLocalSetting(SettingNames.CustomDrawId, value.Id);
        }

        AppViewModel.ResetGlobalSettings();
    }

    partial void OnTranslateServiceChanged(ServiceMetadata value)
    {
        if (value == null)
        {
            return;
        }

        if (value.Id == AzureTranslateId)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultTranslate, TranslateType.Azure);
        }
        else if (value.Id == BaiduTranslateId)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultTranslate, TranslateType.Baidu);
        }
        else
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultTranslate, TranslateType.Custom);
            SettingsToolkit.WriteLocalSetting(SettingNames.CustomTranslateId, value.Id);
        }

        AppViewModel.ResetGlobalSettings();
    }

    partial void OnSpeechServiceChanged(ServiceMetadata value)
    {
        if (value == null)
        {
            return;
        }

        if (value.Id == AzureSpeechId)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultSpeech, SpeechType.Azure);
        }
        else if (value.Id == AzureWhisperId)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultSpeech, SpeechType.AzureWhisper);
        }
        else
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.DefaultSpeech, SpeechType.Custom);
            SettingsToolkit.WriteLocalSetting(SettingNames.CustomSpeechId, value.Id);
        }

        AppViewModel.ResetGlobalSettings();
    }
}
