// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.App.Kernel;
using RichasyAssistant.Models.Context;

namespace RichasyAssistant.Libs.Service;

/// <summary>
/// 聊天数据服务.
/// </summary>
public sealed partial class ChatDataService
{
    private const string DbName = "chat.db";
    private const string ExtraKernelFileName = "_extraKernels.json";
    private static ChatDbContext _dbContext;

    private static List<ChatSession> _sessions;
    private static List<Assistant> _assistants;
    private static List<ServiceMetadata> _extraKernels;
}
