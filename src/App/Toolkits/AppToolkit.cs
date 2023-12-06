// Copyright (c) Richasy Assistant. All rights reserved.

using System.Globalization;
using System.Text;
using Windows.ApplicationModel;
using Windows.UI;

namespace RichasyAssistant.App.Toolkits;

/// <summary>
/// 应用工具组.
/// </summary>
public static class AppToolkit
{
    /// <summary>
    /// Get the current environment language code.
    /// </summary>
    /// <param name="isWindowsName">
    /// Whether it is the Windows display name,
    /// for example, Simplified Chinese is CHS,
    /// if not, it is displayed as the default name,
    /// for example, Simplified Chinese is zh-Hans.
    /// </param>
    /// <returns>Language code.</returns>
    public static string GetLanguageCode(bool isWindowsName = false)
    {
        var culture = CultureInfo.CurrentUICulture;
        return isWindowsName ? culture.ThreeLetterWindowsLanguageName : culture.Name;
    }

    /// <summary>
    /// 获取应用包版本.
    /// </summary>
    /// <returns>包版本.</returns>
    public static string GetPackageVersion()
    {
        var appVersion = Package.Current.Id.Version;
        return $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}.{appVersion.Revision}";
    }

    /// <summary>
    /// 将颜色转换为十六进制字符串.
    /// </summary>
    /// <param name="color">颜色.</param>
    /// <returns>十六进制字符串.</returns>
    public static string ColorToHex(Windows.UI.Color color)
        => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    /// <summary>
    /// Convert the color code to the <see cref="Color"/> object.
    /// </summary>
    /// <param name="hexCode">Hex color. e.g. #FFFFFF.</param>
    /// <returns><see cref="Color"/>.</returns>
    public static Color HexToColor(string hexCode)
    {
        if (!hexCode.StartsWith('#'))
        {
            throw new ArgumentException("Hex color must start with #.");
        }

        hexCode = hexCode[1..];
        var color = default(Color);

        if (hexCode.Length != 6)
        {
            throw new ArgumentException("Hex color must be 6 characters long.");
        }

        color.R = byte.Parse(hexCode[..2], NumberStyles.HexNumber);
        color.G = byte.Parse(hexCode.Substring(2, 2), NumberStyles.HexNumber);
        color.B = byte.Parse(hexCode.Substring(4, 2), NumberStyles.HexNumber);
        color.A = 255;

        return color;
    }

    /// <summary>
    /// Base64编码.
    /// </summary>
    /// <returns>Base64 text.</returns>
    public static string Base64Encode(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        var base64String = Convert.ToBase64String(bytes);
        return base64String;
    }

    /// <summary>
    /// Base64解码.
    /// </summary>
    /// <returns>Text.</returns>
    public static string Base64Decode(string text)
    {
        var bytes = Convert.FromBase64String(text);
        var decodeText = Encoding.UTF8.GetString(bytes);
        return decodeText;
    }

    /// <summary>
    /// 重置控件主题.
    /// </summary>
    /// <param name="element">控件.</param>
    public static void ResetControlTheme(FrameworkElement element)
    {
        var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        element.RequestedTheme = localTheme;
    }
}
