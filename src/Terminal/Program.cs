// Copyright (c) Reader Copilot. All rights reserved.

using System.Text.Json;
using RichasyAssistant.Libs.Kernel;
using RichasyAssistant.Libs.Locator;
using RichasyAssistant.Models.App.Kernel;
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

// 配置是否自动生成会话标题.
GlobalSettings.Set(SettingNames.IsAutoGenerateSessionTitle, true);

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

// 初始化插件.
client.InitializeCorePlugins();
AnsiConsole.MarkupLine("[grey]已初始化内部插件[/]");

// 获取已有会话，并初始化选项.
start: var sessions = client.GetSessions();
var options = new List<string> { "新会话", "提示词管理" };
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
        new SelectionPrompt<SessionPayload>()
            .Title("选择需要删除的会话")
            .AddChoices(sessions)
            .UseConverter(p => string.IsNullOrEmpty(p.Title) ? p.Id : p.Title));

    await client.RemoveSessionAsync(selectSession.Id);
    goto start;
}
else if (selectOption == "继续会话")
{
    var selectSession = AnsiConsole.Prompt(
        new SelectionPrompt<SessionPayload>()
            .Title("选择需要继续的会话")
            .AddChoices(sessions)
            .UseConverter(p => string.IsNullOrEmpty(p.Title) ? p.Id : p.Title));
    selectSessionId = selectSession.Id;
}
else if (selectOption == "提示词管理")
{
promptStart:
    var prompts = client.GetPrompts();
    var pOptions = new List<string> { "添加提示词", "返回" };

    if (prompts.Count > 0)
    {
        pOptions.Insert(1, "删除提示词");
    }

    var pSelectOption = AnsiConsole.Prompt(
               new SelectionPrompt<string>()
               .Title("提示词管理")
               .AddChoices(pOptions));

    if (pSelectOption == "添加提示词")
    {
        var name = AnsiConsole.Ask<string>("[grey]提示词名称: [/]");
        var prompt = AnsiConsole.Ask<string>("[grey]提示词: [/]");
        var newPrompt = new SystemPrompt(name, prompt);
        await client.AddOrUpdatePromptAsync(newPrompt);
        goto start;
    }
    else if (pSelectOption == "返回")
    {
        goto start;
    }
    else
    {
        var selectPrompt = AnsiConsole.Prompt(
        new SelectionPrompt<SystemPrompt>()
            .Title("选择需要删除的提示词")
            .AddChoices(prompts)
            .UseConverter(p => p.Name));

        await client.RemoveSessionAsync(selectPrompt.Id);
        goto promptStart;
    }
}

if (string.IsNullOrEmpty(selectSessionId))
{
    // 选择提示词.
    var prompts = client.GetPrompts();
    prompts.Insert(0, new SystemPrompt("无", string.Empty));
    var systemPrompt = string.Empty;
    if (prompts.Count > 1)
    {
        var prompt = AnsiConsole.Prompt(
                       new SelectionPrompt<SystemPrompt>()
                          .Title("选择提示词")
                          .AddChoices(prompts)
                          .UseConverter(p => p.Name));

        if (prompt.Name != "无")
        {
            AnsiConsole.MarkupLine($"[grey]已选择提示词: {prompt.Name}[/]");
            systemPrompt = prompt.Prompt;
        }
    }

    // 创建会话.
    var newSession = await client.CreateNewSessionAsync(systemPrompt);
    AnsiConsole.MarkupLine($"[grey]已创建会话: {newSession.Id}[/]");
    selectSessionId = newSession.Id;
}

// 切换会话.
client.SwitchSession(selectSessionId);
AnsiConsole.MarkupLine($"[grey]已切换会话: {selectSessionId}[/]");

// 开始聊天.
chatStart:
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
var session = client.GetSession(selectSessionId);
var needGenerateTitle = GlobalSettings.TryGet<bool>(SettingNames.IsAutoGenerateSessionTitle)
    && string.IsNullOrEmpty(session.Title);
while (!needExit)
{
    if (needGenerateTitle)
    {
        var title = await client.TryGenerateTitleAsync();
        if (!string.IsNullOrEmpty(title))
        {
            AnsiConsole.MarkupLine($"[grey]会话标题已更新为: {title.EscapeMarkup()}[/]");
            needGenerateTitle = false;
        }
    }

    var message = AnsiConsole.Ask<string>("[purple_1]我: [/]");

    if (message == "exit")
    {
        break;
    }
    else if (message == "clear")
    {
        var ids = client.GetSession(selectSessionId).Messages.Where(p => p.Role != ChatMessageRole.System).Select(p => p.Id);
        await client.RemoveMessagesAsync(ids.ToArray());
        AnsiConsole.MarkupLine("[grey]已清空聊天记录[/]");
        goto chatStart;
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
