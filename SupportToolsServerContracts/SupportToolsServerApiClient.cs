using ApiContracts;
using Microsoft.Extensions.Logging;
using StringMessagesApiContracts;

namespace SupportToolsServerContracts;

public class SupportToolsServerApiClient : ApiClient
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public SupportToolsServerApiClient(ILogger logger, IHttpClientFactory httpClientFactory, string server, string? apiKey,
        bool useConsole) : base(logger, httpClientFactory, server, apiKey, new StringMessageHubClient(server, apiKey),
        useConsole)
    {
    }

}