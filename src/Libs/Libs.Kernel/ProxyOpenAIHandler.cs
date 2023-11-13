// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Libs.Kernel;

internal class ProxyOpenAIHandler : HttpClientHandler
{
    private readonly string _proxyUrl;

    public ProxyOpenAIHandler(string proxyUrl)
        => _proxyUrl = proxyUrl.TrimEnd('/');

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri != null && request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase))
        {
            var path = request.RequestUri.PathAndQuery.TrimStart('/');
            request.RequestUri = new Uri($"{_proxyUrl}/{path}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}
