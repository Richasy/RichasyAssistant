// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Locator;

/// <summary>
/// 全局设置.
/// </summary>
public static class GlobalSettings
{
    private static readonly Dictionary<SettingNames, object> _settings = new();

    /// <summary>
    /// 添加设置.
    /// </summary>
    /// <param name="name">设置名称.</param>
    /// <param name="value">值.</param>
    public static void Set(SettingNames name, object value)
    {
        if (_settings.ContainsKey(name))
        {
            _settings[name] = value;
        }
        else
        {
            _settings.Add(name, value);
        }
    }

    /// <summary>
    /// 获取设置.
    /// </summary>
    /// <typeparam name="T">设置类型.</typeparam>
    /// <param name="name">名称.</param>
    /// <returns>设置.</returns>
    public static T? TryGet<T>(SettingNames name)
        => _settings.TryGetValue(name, out var value) ? (T)value : default;

    /// <summary>
    /// 清空所有设置.
    /// </summary>
    public static void Clear() => _settings.Clear();
}
