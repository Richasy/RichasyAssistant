// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Threading;
using RichasyAssistant.Models.Constants;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace RichasyAssistant.AppServices;

/// <summary>
/// 基础服务，用于后台任务的注册.
/// </summary>
public sealed class BasicService : IBackgroundTask
{
    private BackgroundTaskDeferral _deferral;
    private AppServiceConnection _connection;
    private CancellationTokenSource _cancellation;

    /// <inheritdoc/>
    public void Run(IBackgroundTaskInstance taskInstance)
    {
        _deferral = taskInstance.GetDeferral();
        taskInstance.Canceled += OnTaskInstanceCanceled;
        var detail = taskInstance.TriggerDetails as AppServiceTriggerDetails;
        _connection = detail.AppServiceConnection;
        _connection.RequestReceived += OnConnectionRequestReceivedAsync;
    }

    private async void OnConnectionRequestReceivedAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
    {
        var msgDeferral = args.GetDeferral();

        var msg = args.Request.Message;
        var returnData = new ValueSet();

        var command = msg["Command"] as string;
        var request = msg["Request"] as string;

        if (!string.IsNullOrEmpty(command))
        {
            _cancellation = new CancellationTokenSource();
            try
            {
                if (command.Equals("getlib", StringComparison.InvariantCultureIgnoreCase))
                {
                    var libPath = BasicUtils.ReadLocalSetting(SettingNames.LibraryFolderPath, string.Empty);
                    returnData.Add("Response", libPath);
                }
            }
            catch (Exception ex)
            {
                returnData.Add("Error", ex.Message);
            }

            _cancellation = null;
        }
        else
        {
            returnData.Add("Error", "Invalid command");
        }

        try
        {
            await args.Request.SendResponseAsync(returnData);
        }
        catch (Exception)
        {
        }
        finally
        {
            msgDeferral.Complete();
        }
    }

    private void OnTaskInstanceCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
    {
        _cancellation?.Cancel();
        _deferral?.Complete();
    }
}
