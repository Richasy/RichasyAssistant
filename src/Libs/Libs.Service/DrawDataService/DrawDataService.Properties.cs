// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Context;

namespace RichasyAssistant.Libs.Service;

/// <summary>
/// 绘图数据服务.
/// </summary>
public sealed partial class DrawDataService
{
    private const string DbName = "draw.db";

    private static DrawDbContext _dbContext;
    private static List<AiImage> _images;
}
