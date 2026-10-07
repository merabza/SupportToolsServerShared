using System.Collections.Generic;

namespace SupportToolsServerApiContracts.Models;

//პროექტის პარამეტრები ერთ სერვერსა და გარემოში, კლიენტის ServerInfos dictionary-ის ჩანაწერი (ServerInfoModel), პროექტის
//აგრეგატის ნაწილი (README G7): საკუთარი ვერსია არ აქვს და მისი ცვლილება პროექტის Version-ს ზრდის. ServerName და
//EnvironmentName სერვერისა და გარემოს სახელებია და ერთად ჩანაწერის გასაღებია: პროექტში ერთხელ გვხვდება, რეგისტრის
//გარეშე. კლიენტის dictionary-ის key სერვერზე არ მიდის. WebAgentNameForCheck ApiClient-ის სახელია; null ან ცარიელი
//ნიშნავს, რომ მითითება არ არის. არარსებული სახელები 404 ReferencedRecordsNotFound-ია. ServerSidePort 0-დან 65535-მდეა,
//0 ნიშნავს, რომ პორტი არ არის. AppSettings-ის ფაილები კანონიკური გზებია (README G3). AllowToolsList კლიენტის
//EProjectServerTools-ის სახელებია: სიმრავლეა, ამიტომ სერვერი მას სახელით დალაგებულს აბრუნებს (OrdinalIgnoreCase).
//CurrentDatabaseParameters და NewDatabaseParameters null-ია, თუ ისინი არ არის
public sealed class StsServerInfoDataModel
{
    public required string ServerName { get; set; }
    public required string EnvironmentName { get; set; }
    public string? WebAgentNameForCheck { get; set; }
    public int ServerSidePort { get; set; }
    public string? ApiVersionId { get; set; }
    public string? AppSettingsJsonSourceFileName { get; set; }
    public string? AppSettingsEncodedJsonFileName { get; set; }
    public string? ServiceUserName { get; set; }
    public List<string> AllowToolsList { get; set; } = [];
    public StsDatabaseParametersDataModel? CurrentDatabaseParameters { get; set; }
    public StsDatabaseParametersDataModel? NewDatabaseParameters { get; set; }
}