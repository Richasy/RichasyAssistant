// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.App.ViewModels.Components;
using RichasyAssistant.Models.App.Kernel;
using Windows.ApplicationModel.DataTransfer;

namespace RichasyAssistant.App.ViewModels.Items;

/// <summary>
/// 聊天消息条目视图模型.
/// </summary>
public sealed partial class ChatMessageItemViewModel : ViewModelBase
{
    private readonly ChatMessage _data;
    private readonly Action<ChatMessage> _regenerateAction;
    private readonly Action<ChatMessage> _editAction;
    private readonly Action<ChatMessage> _deleteAction;

    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private bool _isUser;

    [ObservableProperty]
    private bool _isAssistant;

    [ObservableProperty]
    private string _time;

    [ObservableProperty]
    private bool _isLastMessage;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isRegenerateButtonShown;

    [ObservableProperty]
    private bool _useMarkdownRenderer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemViewModel"/> class.
    /// </summary>
    public ChatMessageItemViewModel(
        ChatMessage message,
        Action<ChatMessage> regenerateAction = default,
        Action<ChatMessage> editAction = default,
        Action<ChatMessage> deleteAction = default)
    {
        _data = message;
        Content = message.Content;
        IsAssistant = message.Role == ChatMessageRole.Assistant;
        IsUser = message.Role == ChatMessageRole.User;
        Time = message.Time.ToString("MM/dd HH:mm:ss");
        _regenerateAction = regenerateAction;
        _editAction = editAction;
        _deleteAction = deleteAction;
        UseMarkdownRenderer = SettingsToolkit.ReadLocalSetting(SettingNames.UseMarkdownRenderer, true);
        CheckRegenerateButtonState();
    }

    /// <summary>
    /// 获取数据.
    /// </summary>
    /// <returns>消息数据.</returns>
    public ChatMessage GetData()
        => _data;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ChatMessageItemViewModel model && EqualityComparer<ChatMessage>.Default.Equals(_data, model._data);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_data);

    [RelayCommand]
    private void Copy()
    {
        var dp = new DataPackage();
        dp.SetText(Content);
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(StringNames.Copied, InfoType.Success);
    }

    [RelayCommand]
    private void Regenerate()
        => _regenerateAction?.Invoke(_data);

    [RelayCommand]
    private void Edit()
    {
        if (string.IsNullOrEmpty(Content))
        {
            Delete();
            return;
        }

        _data.Content = Content;
        _editAction?.Invoke(_data);
    }

    [RelayCommand]
    private void Delete()
        => _deleteAction?.Invoke(_data);

    private void CheckRegenerateButtonState()
        => IsRegenerateButtonShown = !IsUser && IsLastMessage;

    partial void OnIsLastMessageChanged(bool value)
        => CheckRegenerateButtonState();
}
