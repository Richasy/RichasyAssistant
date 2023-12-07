// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.Converters;

/// <summary>
/// 数量转换器.
/// </summary>
public sealed partial class CountConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var count = (int)value;
        return count == -1
            ? ResourceToolkit.GetLocalizedString(StringNames.NoLimit)
            : string.Format(ResourceToolkit.GetLocalizedString(StringNames.CountLimitTemplate), value);
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
