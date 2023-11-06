// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RichasyAssistant.Models.App.Translate;

namespace RichasyAssistant.Libs.Database;

/// <summary>
/// 翻译数据存储库.
/// </summary>
public sealed class TranslationDbContext : DbContext
{
    private readonly string _dbPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationDbContext"/> class.
    /// </summary>
    public TranslationDbContext() => _dbPath = "Assets/trans.db";

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationDbContext"/> class.
    /// </summary>
    public TranslationDbContext(string dbPath) => _dbPath = dbPath;

    /// <summary>
    /// 语言列表.
    /// </summary>
    public DbSet<LanguageList> Languages { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LanguageList>()
            .HasMany(p => p.Langauges)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
}
