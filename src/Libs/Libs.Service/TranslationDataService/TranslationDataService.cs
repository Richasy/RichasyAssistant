﻿// Copyright (c) Richasy Assistant. All rights reserved.

using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Args;
using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.App.Translate;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Models.Context;

namespace RichasyAssistant.Libs.Service;

/// <summary>
/// 翻译数据服务.
/// </summary>
public static partial class TranslationDataService
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
            var defaultTranslateDbPath = GlobalSettings.TryGet<string>(SettingNames.DefaultTranslationDbPath);
            if (string.IsNullOrEmpty(defaultTranslateDbPath)
                || !File.Exists(defaultTranslateDbPath))
            {
                throw new KernelException(KernelExceptionType.TranslationDbNotInitialized);
            }

            await Task.Run(() =>
            {
                File.Copy(defaultTranslateDbPath, localDbPath, true);
            });
        }

        var context = new TranslationDbContext(localDbPath);
        _dbContext = context;

        await _dbContext.Database.MigrateAsync();
        await InitializeLanguagesAsync();
        await InitializeHistoryAsync();
    }

    /// <summary>
    /// 获取语言列表.
    /// </summary>
    /// <returns>语言列表.</returns>
    public static List<Metadata> GetLanguageList(TranslateType type)
    {
        var id = type == TranslateType.Azure ? AzureIdentify : BaiduIdentify;
        return _languages.ContainsKey(id) ? _languages[id] : default;
    }

    /// <summary>
    /// 检查是否有更多历史记录.
    /// </summary>
    /// <param name="page">页码.</param>
    /// <returns>结果.</returns>
    public static bool HasMoreHistory(int page)
    {
        var startIndex = page * 100;
        return startIndex < _history.Count;
    }

    /// <summary>
    /// 获取历史记录（分页）.
    /// </summary>
    /// <param name="page">页码（每页100个）.</param>
    /// <returns>历史记录.</returns>
    public static List<TranslationRecord> GetHistory(int page)
    {
        return HasMoreHistory(page)
            ? _history.Skip(page * 100).Take(100).OrderByDescending(p => p.Time).ToList()
            : (List<TranslationRecord>?)default;
    }

    /// <summary>
    /// 添加新的语言列表.
    /// </summary>
    /// <param name="list">语言列表.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task AddOrUpdateLanguageListAsync(LanguageList list)
    {
        var suffix = list.Id == AzureIdentify ? "_a" : "_b";
        foreach (var item in list.Languages)
        {
            item.Id = item.Id + suffix;
        }

        if (!_languages.ContainsKey(list.Id))
        {
            _languages.Add(list.Id, list.Languages);
            await _dbContext.Languages.AddAsync(list);
        }
        else
        {
            var source = await _dbContext.Languages.Include(p => p.Languages).FirstOrDefaultAsync(p => p.Id == list.Id);
            foreach (var item in list.Languages)
            {
                if (!source.Languages.Any(j => j.Equals(item)))
                {
                    source.Languages.Add(item);
                }
            }

            _dbContext.Languages.Update(source);
            _languages[list.Id] = list.Languages;
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 添加新的历史记录.
    /// </summary>
    /// <param name="record">翻译记录.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task AddRecordAsync(TranslationRecord record)
    {
        if (_history.Any(p => p.Equals(record)))
        {
            return;
        }

        _history.Add(record);

        await _dbContext.History.AddAsync(record);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 移除历史记录.
    /// </summary>
    /// <param name="recordId">历史记录 Id.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task RemoveRecordAsync(string recordId)
    {
        var record = _history.FirstOrDefault(p => p.Id == recordId);
        if (record != null)
        {
            _history.Remove(record);

            var source = await _dbContext.History.FirstOrDefaultAsync(p => p.Id == recordId);
            _dbContext.History.Remove(source);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 清空历史记录.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task ClearHistoryAsync()
    {
        if (_history.Count == 0)
        {
            return;
        }

        _history.Clear();
        _dbContext.History.RemoveRange(_dbContext.History);
        await _dbContext.SaveChangesAsync();
    }

    private static async Task InitializeLanguagesAsync()
    {
        try
        {
            var languages = await _dbContext.Languages.AsNoTracking().Include(p => p.Languages).ToListAsync();
            languages.ForEach(p =>
            {
                if (p.Languages != null)
                {
                    var id = p.Id;
                    var suffix = id == AzureIdentify ? "_a" : "_b";
                    foreach (var item in p.Languages)
                    {
                        item.Id = item.Id.Replace(suffix, string.Empty);
                        var culture = new CultureInfo(item.Id);
                        item.Value = culture.DisplayName;
                    }

                    _languages.Add(id, p.Languages);
                }
            });
        }
        catch (Exception ex)
        {
            var kex = new KernelException(KernelExceptionType.TranslationServiceNotInitialized, ex);
            throw kex;
        }
    }

    private static async Task InitializeHistoryAsync()
    {
        try
        {
            _history = await _dbContext.History.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            throw new KernelException(KernelExceptionType.TranslationServiceNotInitialized, ex);
        }
    }
}
