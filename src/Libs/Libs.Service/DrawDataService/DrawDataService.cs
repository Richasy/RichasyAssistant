// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Models.Context;

namespace RichasyAssistant.Libs.Service;

/// <summary>
/// 绘图数据服务.
/// </summary>
public sealed partial class DrawDataService
{
    /// <summary>
    /// 初始化.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    /// <exception cref="KernelException">错误.</exception>
    public static async Task InitializeAsync()
    {
        if (_dbContext != null)
        {
            return;
        }

        var libPath = GlobalSettings.TryGet<string>(SettingNames.LibraryFolderPath);
        if (string.IsNullOrEmpty(libPath))
        {
            throw new KernelException(KernelExceptionType.LibraryNotInitialized);
        }

        var localDbPath = Path.Combine(libPath, DbName);
        if (!File.Exists(localDbPath))
        {
            var defaultDbPath = GlobalSettings.TryGet<string>(SettingNames.DefaultDrawDbPath);
            if (string.IsNullOrEmpty(defaultDbPath)
                || !File.Exists(defaultDbPath))
            {
                throw new KernelException(KernelExceptionType.DrawDbNotInitialized);
            }

            await Task.Run(() =>
            {
                File.Copy(defaultDbPath, localDbPath, true);
            });
        }

        var context = new DrawDbContext(localDbPath);
        _dbContext = context;

        await InitializeHistoryAsync();
    }

    /// <summary>
    /// 检查是否有更多历史记录.
    /// </summary>
    /// <param name="page">页码.</param>
    /// <returns>结果.</returns>
    public static bool HasMoreHistory(int page)
    {
        var startIndex = page * 100;
        return startIndex < _images.Count;
    }

    /// <summary>
    /// 获取历史记录（分页）.
    /// </summary>
    /// <param name="page">页码（每页100个）.</param>
    /// <returns>历史记录.</returns>
    public static List<AiImage> GetHistory(int page)
    {
        return HasMoreHistory(page)
            ? _images.Skip(page * 100).Take(100).OrderByDescending(p => p.Time).ToList()
            : (List<AiImage>?)default;
    }

    /// <summary>
    /// 添加新的历史记录.
    /// </summary>
    /// <param name="image">图片记录.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task AddImageAsync(AiImage image)
    {
        if (_images.Any(p => p.Equals(image)))
        {
            return;
        }

        _images.Add(image);

        await _dbContext.Images.AddAsync(image);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 移除历史记录.
    /// </summary>
    /// <param name="imageId">历史记录 Id.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task RemoveImageAsync(string imageId)
    {
        var img = _images.FirstOrDefault(p => p.Id == imageId);
        if (img != null)
        {
            _images.Remove(img);

            var source = await _dbContext.Images.FirstOrDefaultAsync(p => p.Id == imageId);
            _dbContext.Images.Remove(source);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 清空历史记录.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task ClearHistoryAsync()
    {
        if (_images.Count == 0)
        {
            return;
        }

        _images.Clear();
        _dbContext.Images.RemoveRange(_dbContext.Images);
        await _dbContext.SaveChangesAsync();
    }

    private static async Task InitializeHistoryAsync()
    {
        try
        {
            _images = await _dbContext.Images.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            throw new KernelException(KernelExceptionType.DrawServiceNotInitialized, ex);
        }
    }
}
