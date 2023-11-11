// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RichasyAssistant.Models.App.Kernel;

namespace RichasyAssistant.Models.Context;

/// <summary>
/// 聊天数据存储库.
/// </summary>
public sealed class ChatDbContext : DbContext
{
    private readonly string _dbPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatDbContext"/> class.
    /// </summary>
    public ChatDbContext() => _dbPath = "Assets/chat.db";

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatDbContext"/> class.
    /// </summary>
    public ChatDbContext(string dbPath) => _dbPath = dbPath;

    /// <summary>
    /// 会话列表.
    /// </summary>
    public DbSet<ChatSession> Sessions { get; set; }

    /// <summary>
    /// 助理列表.
    /// </summary>
    public DbSet<Assistant> Assistants { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<List<string>>()
            .HasNoKey();

        modelBuilder.Entity<ChatSession>()
            .HasMany(p => p.Messages)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatSession>()
            .Property(e => e.Assistants)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        modelBuilder.Entity<ChatSession>()
            .HasOne(p => p.Options)
            .WithOne()
            .HasForeignKey<SessionOptions>(o => o.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
}
