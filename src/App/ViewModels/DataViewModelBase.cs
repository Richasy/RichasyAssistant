// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.ViewModels;

/// <summary>
/// 数据视图模型基类.
/// </summary>
/// <typeparam name="T">数据类型.</typeparam>
public abstract partial class DataViewModelBase<T> : ViewModelBase
{
    [ObservableProperty]
    private T _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataViewModelBase{T}"/> class.
    /// </summary>
    public DataViewModelBase(T data) => Data = data;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is DataViewModelBase<T> @base && EqualityComparer<T>.Default.Equals(Data, @base.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Data);
}
