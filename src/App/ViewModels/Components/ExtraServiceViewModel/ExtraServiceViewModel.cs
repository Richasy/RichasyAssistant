// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Items;
using RichasyAssistant.Libs.Service;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.App.ViewModels.Components;

/// <summary>
/// 额外服务视图模型.
/// </summary>
public sealed partial class ExtraServiceViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtraServiceViewModel"/> class.
    /// </summary>
    private ExtraServiceViewModel()
    {
        CustomKernels = new ObservableCollection<ExtraServiceItemViewModel>();
        CustomSpeechServices = new ObservableCollection<ExtraServiceItemViewModel>();
        CustomTranslateServices = new ObservableCollection<ExtraServiceItemViewModel>();
        CustomDrawServices = new ObservableCollection<ExtraServiceItemViewModel>();
    }

    /// <summary>
    /// 清除所有服务.
    /// </summary>
    public void Clean()
    {
        if (CustomKernels.Count > 0)
        {
            foreach (var item in CustomKernels)
            {
                item.StopCommand.Execute(default);
            }
        }
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        await ChatDataService.InitializeExtraKernelsAsync();
        var kernels = ChatDataService.GetExtraKernels();
        foreach (var kernel in kernels)
        {
            CustomKernels.Add(new ExtraServiceItemViewModel(kernel, ServiceType.Kernel));
        }

        CheckServiceType();
        CheckServicesAvailable();
    }

    /// <summary>
    /// 启动内核服务.
    /// </summary>
    /// <param name="service">服务数据.</param>
    [RelayCommand]
    private async Task LaunchKernelServiceAsync(ServiceMetadata service)
    {
        var runningService = CustomKernels.FirstOrDefault(p => p.Data.Equals(service));
        if (runningService == null)
        {
            var newVM = new ExtraServiceItemViewModel(service, ServiceType.Kernel);
            CustomKernels.Add(newVM);
            runningService = newVM;
            CheckServicesAvailable();
        }

        await runningService.InitializeCommand.ExecuteAsync(default);
    }

    [RelayCommand]
    private async Task TryRemoveKernelServiceAsync(ServiceMetadata service)
    {
        var source = CustomKernels.FirstOrDefault(p => p.Data.Equals(service));
        if (source == null)
        {
            return;
        }

        await source.StopCommand.ExecuteAsync(default);
        CustomKernels.Remove(source);
        CheckServicesAvailable();
    }

    private void CheckServiceType()
    {
        IsKernel = ServiceType == ServiceType.Kernel;
        IsSpeech = ServiceType == ServiceType.Speech;
        IsTranslate = ServiceType == ServiceType.Translate;
        IsDraw = ServiceType == ServiceType.Draw;

        IsEmpty = ServiceType switch
        {
            ServiceType.Speech => CustomSpeechServices.Count == 0,
            ServiceType.Translate => CustomTranslateServices.Count == 0,
            ServiceType.Draw => CustomDrawServices.Count == 0,
            _ => CustomKernels.Count == 0,
        };

        Title = ServiceType switch
        {
            ServiceType.Speech => ResourceToolkit.GetLocalizedString(StringNames.SpeechService),
            ServiceType.Translate => ResourceToolkit.GetLocalizedString(StringNames.TranslateService),
            ServiceType.Draw => ResourceToolkit.GetLocalizedString(StringNames.DrawService),
            _ => ResourceToolkit.GetLocalizedString(StringNames.KernelService),
        };
    }

    private void CheckServicesAvailable()
    {
        HasService = CustomKernels.Count > 0
            || CustomDrawServices.Count > 0
            || CustomTranslateServices.Count > 0
            || CustomSpeechServices.Count > 0;
    }

    partial void OnServiceTypeChanged(ServiceType value)
        => CheckServiceType();
}
