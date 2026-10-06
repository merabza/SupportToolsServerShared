namespace SupportToolsServerApiContracts.Models;

//პროექტის route კლასის აღწერა, კლიენტის RouteClasses dictionary-ის ჩანაწერი (RouteClassModel). Name dictionary-ის key-ა
//და პროექტში უნიკალურია. ApiVersion კლიენტის RouteClassModel.Version-ია: სახელი იმიტომ განსხვავდება, რომ პროექტის
//Version-ში არ აგერიოს
public sealed class StsProjectRouteClassDataModel
{
    public required string Name { get; set; }
    public string? Root { get; set; }
    public string? ApiVersion { get; set; }
    public string? Base { get; set; }
}