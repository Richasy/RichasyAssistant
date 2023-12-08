// Copyright (c) Richasy Assistant. All rights reserved.

using System.Net;
using Microsoft.SemanticKernel.Http;

namespace RichasyAssistant.Libs.Kernel;

internal static class HttpExtensions
{
    /// <summary>
    /// Reads the content of the HTTP response as a string and translates any HttpRequestException into an HttpOperationException.
    /// </summary>
    /// <param name="httpContent">The HTTP content to read.</param>
    /// <returns>A string representation of the HTTP content.</returns>
    public static async Task<string> ReadAsStringWithExceptionMappingAsync(this HttpContent httpContent)
    {
        try
        {
            return await httpContent.ReadAsStringAsync().ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpOperationException(message: ex.Message, innerException: ex);
        }
    }

    /// <summary>
    /// Reads the content of the HTTP response as a stream and translates any HttpRequestException into an HttpOperationException.
    /// </summary>
    /// <param name="httpContent">The HTTP content to read.</param>
    /// <returns>A stream representing the HTTP content.</returns>
    public static async Task<Stream> ReadAsStreamAndTranslateExceptionAsync(this HttpContent httpContent)
    {
        try
        {
            return await httpContent.ReadAsStreamAsync().ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpOperationException(message: ex.Message, innerException: ex);
        }
    }

    internal static async Task<HttpResponseMessage> SendWithSuccessCheckAsync(this HttpClient client, HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await client.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException e)
        {
            throw new HttpOperationException(HttpStatusCode.BadRequest, null, e.Message, e);
        }

        if (!response.IsSuccessStatusCode)
        {
            string? responseContent = null;
            try
            {
                // On .NET Framework, EnsureSuccessStatusCode disposes of the response content;
                // that was changed years ago in .NET Core, but for .NET Framework it means in order
                // to read the response content in the case of failure, that has to be
                // done before calling EnsureSuccessStatusCode.
                responseContent = await response!.Content.ReadAsStringAsync().ConfigureAwait(false);
                response.EnsureSuccessStatusCode(); // will always throw
            }
            catch (Exception e)
            {
                throw new HttpOperationException(response.StatusCode, responseContent, e.Message, e);
            }
        }

        return response;
    }

    /// <summary>
    /// Sends an HTTP request using the provided <see cref="HttpClient"/> instance and checks for a successful response.
    /// If the response is not successful, it logs an error and throws an <see cref="HttpOperationException"/>.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance to use for sending the request.</param>
    /// <param name="request">The <see cref="HttpRequestMessage"/> to send.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> for canceling the request.</param>
    /// <returns>The <see cref="HttpResponseMessage"/> representing the response.</returns>
    internal static async Task<HttpResponseMessage> SendWithSuccessCheckAsync(this HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await client.SendWithSuccessCheckAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
    }
}
