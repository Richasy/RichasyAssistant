// Copyright (c) Richasy Assistant. All rights reserved.

using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using RichasyAssistant.App.Controls.Dialogs;
using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Views;

/// <summary>
/// 设置页视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private void InitializeChatKernels()
    {
        TryClear(ChatKernels);
        TryClear(KernelExtraServices);
        ChatKernels.Add(new ServiceMetadata(AzureOpenAIId, "Azure Open AI"));
        ChatKernels.Add(new ServiceMetadata(OpenAIId, "Open AI"));

        var extraServices = ChatDataService.GetExtraKernels();
        if (extraServices.Count > 0)
        {
            foreach (var metadata in extraServices)
            {
                ChatKernels.Add(metadata);
                KernelExtraServices.Add(new Items.SlimServiceItemViewModel(metadata, ServiceType.Kernel, DeleteCustomKernelAsync));
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

    private async Task DeleteCustomKernelAsync(ServiceMetadata data)
    {
        ChatKernel = ChatKernels.First();
        ChatKernels.Remove(data);
        var service = KernelExtraServices.FirstOrDefault(p => p.Data.Equals(data));
        var path = service.Path;
        KernelExtraServices.Remove(service);
        await ExtraServiceViewModel.Instance.TryRemoveKernelServiceCommand.ExecuteAsync(data);
        await ChatDataService.DeleteKernelAsync(data.Id);

        await Task.Delay(1000);
        await Task.Run(() =>
        {
            Directory.Delete(path, true);
        });
    }

    private async Task ExtractExtraServiceAsync(string servicePackagePath, ServiceType type)
    {
        var parentFolder = Path.Combine(LibraryPath, "Extensions", type.ToString());
        if (!Directory.Exists(parentFolder))
        {
            Directory.CreateDirectory(parentFolder);
        }

        var tempFolder = Path.Combine(parentFolder, ".temp_unzip");

        var isSuccess = false;
        ExtraServiceConfig metadata = null;
        await Task.Run(() =>
        {
            ZipFile.ExtractToDirectory(servicePackagePath, tempFolder);
            var configFilePath = Path.Combine(tempFolder, "config.json");
            if (!File.Exists(configFilePath))
            {
                Directory.Delete(tempFolder, true);
                return;
            }

            var json = File.ReadAllText(configFilePath);
            metadata = JsonSerializer.Deserialize<ExtraServiceConfig>(json);

            if (string.IsNullOrEmpty(metadata.Id) || string.IsNullOrEmpty(metadata.Name))
            {
                Directory.Delete(tempFolder, true);
                return;
            }
        });

        // 尝试运行初始化脚本.
        if (!string.IsNullOrEmpty(metadata.InitialScript))
        {
            var initScriptPath = Path.Combine(tempFolder, metadata.InitialScript);
            if (File.Exists(initScriptPath))
            {
                var dialog = new TipDialog(ResourceToolkit.GetLocalizedString(StringNames.InitialScriptWarning))
                {
                    PrimaryButtonText = ResourceToolkit.GetLocalizedString(StringNames.Execute),
                    SecondaryButtonText = ResourceToolkit.GetLocalizedString(StringNames.Skip),
                    CloseButtonText = ResourceToolkit.GetLocalizedString(StringNames.Cancel),
                    XamlRoot = AppViewModel.Instance.ActivatedWindow.Content.XamlRoot,
                };

                var dialogResult = await dialog.ShowAsync();
                if (dialogResult == ContentDialogResult.Primary)
                {
                    var tipDialog = new TipDialog(ResourceToolkit.GetLocalizedString(StringNames.InitialScriptRunningTip))
                    {
                        PrimaryButtonText = string.Empty,
                        CloseButtonText = ResourceToolkit.GetLocalizedString(StringNames.Confirm),
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = AppViewModel.Instance.ActivatedWindow.Content.XamlRoot,
                    };

                    tipDialog.Loaded += async (_, _) =>
                    {
                        var initialProcess = new Process();
                        initialProcess.StartInfo.FileName = "powershell.exe";
                        initialProcess.StartInfo.Arguments = $"-ExecutionPolicy Bypass -File \"{initScriptPath}\"";
                        initialProcess.StartInfo.UseShellExecute = true;
                        initialProcess.StartInfo.WorkingDirectory = tempFolder;
                        initialProcess.Start();
                        await initialProcess.WaitForExitAsync();
                        tipDialog.Hide();
                    };

                    await tipDialog.ShowAsync();
                }
                else if (dialogResult == ContentDialogResult.None)
                {
                    Directory.Delete(tempFolder, true);
                    return;
                }
            }
        }

        await Task.Run(() =>
        {
            var targetFolder = Path.Combine(parentFolder, metadata.Id);
            if (Directory.Exists(targetFolder))
            {
                Directory.Delete(targetFolder, true);
            }

            Directory.CreateDirectory(targetFolder);

            // Move all files under temp folder to target folder but not the temp folder itself.
            foreach (var file in Directory.GetFiles(tempFolder))
            {
                var fileName = Path.GetFileName(file);
                var targetFilePath = Path.Combine(targetFolder, fileName);
                File.Move(file, targetFilePath);
            }

            Directory.Delete(tempFolder, true);
            isSuccess = true;
        });

        if (!isSuccess)
        {
            throw new Exception(ResourceToolkit.GetLocalizedString(StringNames.ImportServiceFailed));
        }

        await ChatDataService.AddOrUpdateExtraKernelAsync(metadata);
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
