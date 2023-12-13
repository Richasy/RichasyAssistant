// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using RichasyAssistant.Models.Constants;
using Windows.Storage;

namespace RichasyAssistant.AppServices;
internal static class BasicUtils
{
    internal static T ReadLocalSetting<T>(SettingNames settingName, T defaultValue)
    {
        var settingContainer = ApplicationData.Current.LocalSettings;

        if (IsSettingKeyExist(settingName))
        {
            if (defaultValue is Enum)
            {
                var tempValue = settingContainer.Values[settingName.ToString()].ToString();
                Enum.TryParse(typeof(T), tempValue, out var result);
                return (T)result;
            }
            else
            {
                return (T)settingContainer.Values[settingName.ToString()];
            }
        }
        else
        {
            return defaultValue;
        }
    }

    internal static bool IsSettingKeyExist(SettingNames settingName)
        => ApplicationData.Current.LocalSettings.Values.ContainsKey(settingName.ToString());
}
