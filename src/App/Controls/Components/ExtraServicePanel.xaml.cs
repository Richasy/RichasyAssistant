// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;

namespace RichasyAssistant.App.Controls.Components;

/// <summary>
/// 扩展服务面板.
/// </summary>
public sealed partial class ExtraServicePanel : ExtraServiceControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtraServicePanel"/> class.
    /// </summary>
    public ExtraServicePanel()
    {
        InitializeComponent();
        ViewModel = ExtraServiceViewModel.Instance;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => ServicePicker.SelectedIndex = (int)ViewModel.ServiceType;

    private void OnServicePickerSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        ViewModel.ServiceType = (ServiceType)ServicePicker.SelectedIndex;
    }
}
