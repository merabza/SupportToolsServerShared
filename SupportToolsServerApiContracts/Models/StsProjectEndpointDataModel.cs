namespace SupportToolsServerApiContracts.Models;

//პროექტის endpoint-ის აღწერა, კლიენტის Endpoints dictionary-ის ჩანაწერი (EndpointModel). Name dictionary-ის key-ა და
//პროექტში უნიკალურია. HttpMethod და EndpointType კლიენტის EHttpMethod-ისა და EEndpointType-ის სახელებია
public sealed class StsProjectEndpointDataModel
{
    public required string Name { get; set; }
    public string? EndpointName { get; set; }
    public string? EndpointRoute { get; set; }
    public bool RequireAuthorization { get; set; }
    public required string HttpMethod { get; set; }
    public required string EndpointType { get; set; }
    public string? ReturnType { get; set; }
    public bool SendMessageToCurrentUser { get; set; }
}