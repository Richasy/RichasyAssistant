// Copyright (c) Reader Copilot. All rights reserved.

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

/**************************************************
 ** 以下代码为控制台程序的UI渲染流程.
 ***************************************************/

// 显示标题.
AnsiConsole.Write(
    new FigletText("Richasy Assistant")
           .LeftJustified()
           .Color(Color.Green));

// 输入系统提示词
AnsiConsole.Write(
    new Rule("[yellow]基础配置[/]")
           .LeftJustified());

var sysPrompt = AnsiConsole.Ask<string>("[grey]请输入系统提示词: [/]", string.Empty).Trim();

// 进行准备工作.
AnsiConsole.Write(
          new Rule("[magenta3_2]准备工作[/]")
                   .LeftJustified());

// 创建聊天客户端.
var client = new ChatClient()
       .UseAzure();

AnsiConsole.MarkupLine("[grey]已创建聊天客户端[/]");

// 创建会话.
var session = client.CreateNewSession(sysPrompt);
AnsiConsole.MarkupLine($"[grey]已创建会话: {session.SessionId}[/]");

// 切换会话.
client.SwitchSession(session.SessionId);
AnsiConsole.MarkupLine($"[grey]已切换会话: {session.SessionId}[/]");

// 开始聊天.
AnsiConsole.Write(
             new Rule("[orange3]聊天[/]")
                      .LeftJustified());

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
