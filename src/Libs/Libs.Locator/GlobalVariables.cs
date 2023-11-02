// Copyright (c) Reader Copilot. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Libs.Locator;

/// <summary>
/// 全局变量.
/// </summary>
public static class GlobalVariables
{
    private static readonly Dictionary<VariableNames, object> _variables = new();

    /// <summary>
    /// 添加变量.
    /// </summary>
    /// <param name="name">变量名称.</param>
    /// <param name="value">值.</param>
    public static void Set(VariableNames name, object value)
    {
        if (_variables.ContainsKey(name))
        {
            TryRemove(name);
            _variables[name] = value;
        }
        else
        {
            _variables.Add(name, value);
        }
    }

    /// <summary>
    /// 获取变量.
    /// </summary>
    /// <typeparam name="T">变量类型.</typeparam>
    /// <param name="name">名称.</param>
    /// <returns>变量.</returns>
    public static T? TryGet<T>(VariableNames name)
        where T : class
        => _variables.TryGetValue(name, out var value) ? (T)value : default;

    /// <summary>
    /// 移除变量.
    /// </summary>
    /// <param name="name">变量名.</param>
    public static void TryRemove(VariableNames name)
    {
        if (_variables.TryGetValue(name, out var value))
        {
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _variables.Remove(name);
        }
    }

    /// <summary>
    /// 清空所有变量.
    /// </summary>
    public static void Clear()
    {
        foreach (var item in _variables)
        {
            if (item.Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        _variables.Clear();
    }
}
