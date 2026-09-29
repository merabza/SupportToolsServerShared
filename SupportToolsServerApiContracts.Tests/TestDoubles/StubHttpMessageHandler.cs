using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SupportToolsServerApiContracts.Tests.TestDoubles;

//Remembers the last request and returns the response given in advance
internal sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly string? _body;
    private readonly string _mediaType;
    private readonly HttpStatusCode _statusCode;

    public StubHttpMessageHandler(HttpStatusCode statusCode, string? body, string mediaType = "application/json")
    {
        _statusCode = statusCode;
        _body = body;
        _mediaType = mediaType;
    }

    public Uri? LastRequestUri { get; private set; }
    public HttpMethod? LastRequestMethod { get; private set; }
    public string? LastRequestBody { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequestUri = request.RequestUri;
        LastRequestMethod = request.Method;
        LastRequestBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);

        var response = new HttpResponseMessage(_statusCode) { RequestMessage = request };
        if (_body is not null)
        {
            response.Content = new StringContent(_body, Encoding.UTF8, _mediaType);
        }

        return response;
    }
}
