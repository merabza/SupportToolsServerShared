using System.Collections.Generic;

namespace SupportToolsServerApiContracts.Models;

//მონაცემთა ბაზის სერვერთან კავშირი, SupportToolsParameters.DatabaseServerConnections-ის ჩანაწერი.
//DatabaseServerProvider კლიენტის EDatabaseProvider-ის სახელია. DbWebAgentName ვებაგენტის ApiClient-ის სახელია; null ან
//ცარიელი ნიშნავს, რომ ვებაგენტი არ არის, არარსებული სახელი კი 404 ReferencedRecordsNotFound-ია. ServerUser და ServerPass
//საიდუმლოა: ღიად გადაიცემა, მაგრამ არსად იბეჭდება. folders set-ები კლიენტის dictionary-ის ჩანაწერებია, სერვერი მათ
//სახელით დალაგებულს აბრუნებს (OrdinalIgnoreCase). ჩანაწერები სახელით ემთხვევა, რეგისტრის გარეშე. upsert-ში Version
//მოსალოდნელი ვერსიაა: 0 ნიშნავს ახალ ჩანაწერს, N კი განახლებას მხოლოდ მაშინ, თუ სერვერზე შენახული ვერსია N-ია.
//განახლება მთელ ჩანაწერს ანაცვლებს, folders set-ების ჩათვლით
public sealed class StsDatabaseServerConnectionDataModel
{
    public required string Name { get; set; }
    public required string DatabaseServerProvider { get; set; }
    public string? DbWebAgentName { get; set; }
    public string? RemoteDbConnectionName { get; set; }
    public string? ServerAddress { get; set; }
    public bool WindowsNtIntegratedSecurity { get; set; }
    public string? ServerUser { get; set; }
    public string? ServerPass { get; set; }
    public bool TrustServerCertificate { get; set; }
    public int ConnectionTimeOut { get; set; }
    public bool Encrypt { get; set; }
    public List<StsDatabaseFoldersSetDataModel> DatabaseFoldersSets { get; set; } = [];
    public int Version { get; set; }
}
