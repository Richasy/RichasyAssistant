﻿// Copyright (c) Reader Copilot. All rights reserved.

using System.Text.Json;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Terminal.Models;
using Spectre.Console;

// 配置默认设置.
GlobalSettings.Set(SettingNames.DefaultTopP, 0d);
GlobalSettings.Set(SettingNames.DefaultTemperature, 0.5d);
GlobalSettings.Set(SettingNames.DefaultFrequencyPenalty, 0d);
GlobalSettings.Set(SettingNames.DefaultPresencePenalty, 0d);
GlobalSettings.Set(SettingNames.DefaultMaxResponseTokens, 1000);

// 配置 Azure OpenAI 设置.
var aoaiConfigContent = File.ReadAllText("Assets/azure-config.json");
var aoaiConfig = JsonSerializer.Deserialize<AzureOpenAIConfig>(aoaiConfigContent);
GlobalSettings.Set(SettingNames.AzureOpenAIAccessKey, aoaiConfig.AccessKey);
GlobalSettings.Set(SettingNames.AzureOpenAIEndpoint, aoaiConfig.Endpoint);
GlobalSettings.Set(SettingNames.AzureOpenAIChatModelName, aoaiConfig.ChatModel);
GlobalSettings.Set(SettingNames.AzureOpenAICompletionModelName, aoaiConfig.CompletionModel);
GlobalSettings.Set(SettingNames.AzureOpenAIEmbeddingModelName, aoaiConfig.EmbeddingModel);

// 配置存储设置.
var localPath = AppDomain.CurrentDomain.BaseDirectory;
var localChatDbPath = Path.Combine(localPath, "Assets/Database/chat.db");
var libPath = Path.Combine(localPath, "Library");
if (!Directory.Exists(libPath))
{
    Directory.CreateDirectory(libPath);
}

GlobalSettings.Set(SettingNames.LibraryFolderPath, libPath);
GlobalSettings.Set(SettingNames.DefaultChatDbPath, localChatDbPath);

/**************************************************
 ** 以下代码为控制台程序的UI渲染流程.
 ***************************************************/

// 显示标题.
AnsiConsole.Write(
    new FigletText("Richasy Assistant")
           .LeftJustified()
           .Color(Color.Green));

// 进行准备工作.
AnsiConsole.Write(
          new Rule("[magenta3_2]准备工作[/]")
                   .LeftJustified());

// 创建聊天客户端.
var client = new ChatClient()
       .UseAzure();

AnsiConsole.MarkupLine("[grey]已创建聊天客户端[/]");

// 初始化数据库.
await client.InitializeLocalDatabaseAsync();
AnsiConsole.MarkupLine("[grey]已初始化聊天数据库[/]");

// 获取已有会话，并初始化选项.
start: var sessions = client.GetSessions();
var options = new List<string> { "新会话" };
if (sessions.Count > 0)
{
    options.Add("继续会话");
    options.Add("删除会话");
}

var selectOption = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("功能")
            .AddChoices(options));

var selectSessionId = string.Empty;
if (selectOption == "删除会话")
{
    var selectSession = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("选择需要删除的会话")
            .AddChoices(sessions.Select(p => p.Id)));

    await client.RemoveSessionAsync(selectSession);
    goto start;
}
else if (selectOption == "继续会话")
{
    selectSessionId = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("选择需要继续的会话")
            .AddChoices(sessions.Select(p => p.Id)));
}

if (string.IsNullOrEmpty(selectSessionId))
{
    // 创建会话.
    var newSession = await client.CreateNewSessionAsync();
    AnsiConsole.MarkupLine($"[grey]已创建会话: {newSession.Id}[/]");
    selectSessionId = newSession.Id;
}

// 切换会话.
client.SwitchSession(selectSessionId);
AnsiConsole.MarkupLine($"[grey]已切换会话: {selectSessionId}[/]");

// 开始聊天.
AnsiConsole.Write(
             new Rule("[orange3]聊天[/]")
                      .LeftJustified());

// 加载初始记录.
var messages = client.GetSession(selectSessionId).Messages;
foreach (var message in messages)
{
    if (message.Role == ChatMessageRole.System)
    {
        AnsiConsole.MarkupLine($"[yellow]系统: [/][grey]{message.Content.EscapeMarkup()}[/]");
    }
    else if (message.Role == ChatMessageRole.User)
    {
        AnsiConsole.MarkupLine($"[purple_1]我: [/]{message.Content.EscapeMarkup()}");
    }
    else
    {
        AnsiConsole.MarkupLine($"[yellow2]助理: [/][aquamarine1_1]{message.Content.EscapeMarkup()}[/]");
    }
}

// 轮询.
var needExit = false;
while (!needExit)
{
    var message = AnsiConsole.Ask<string>("[purple_1]我: [/]");

    if (message == "exit")
    {
        break;
    }

    await AnsiConsole.Status()
             .StartAsync("[grey]正在等待响应...[/]", async ctx =>
             {
                 var response = await client.SendMessageAsync(message);
                 AnsiConsole.MarkupLine($"[yellow2]助理: [/][aquamarine1_1]{response.Content.EscapeMarkup()}[/]");
             });
}

// 退出.
AnsiConsole.MarkupLine("[grey]按任意键退出...[/]");
Console.ReadKey();
