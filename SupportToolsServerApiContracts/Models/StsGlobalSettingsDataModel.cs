namespace SupportToolsServerApiContracts.Models;

//გლობალური პარამეტრები: SupportToolsParameters-ის საერთო ველები, კომპიუტერის ველების გარეშე
//(SupportToolsServerWebApiClientName, LocalInstallerSettings, Archivers, ფოლდერები). FileStorageNameForExchange
//ფაილსაცავის, SmartSchemaNameForExchange და SmartSchemaNameForLocal ჭკვიანი სქემების,
//LocalPackageManagerWebApiClientName კი ApiClient-ის სახელია; null ან ცარიელი ნიშნავს, რომ მითითება არ არის,
//არარსებული სახელები კი 404 ReferencedRecordsNotFound-ია. MediatRLicenseKey საიდუმლოა: ღიად გადაიცემა, მაგრამ არსად იბეჭდება.
//ჩანაწერი ერთადერთია (singleton), ამიტომ გასაღები არ აქვს. სანამ ის შეიქმნება, GET ცარიელ კონტრაქტს აბრუნებს
//Version = 0-ით. upsert-ში Version მოსალოდნელი ვერსიაა: 0 ნიშნავს პირველ შექმნას, N კი განახლებას მხოლოდ მაშინ, თუ
//სერვერზე შენახული ვერსია N-ია. განახლება მთელ ჩანაწერს ანაცვლებს
public sealed class StsGlobalSettingsDataModel
{
    public string? ServiceDescriptionSignature { get; set; }
    public string? UploadTempExtension { get; set; }
    public string? ProgramArchiveDateMask { get; set; }
    public string? ProgramArchiveExtension { get; set; }
    public string? ParametersFileDateMask { get; set; }
    public string? ParametersFileExtension { get; set; }
    public string? MediatRLicenseKey { get; set; }
    public string? FileStorageNameForExchange { get; set; }
    public string? SmartSchemaNameForExchange { get; set; }
    public string? SmartSchemaNameForLocal { get; set; }
    public string? LocalPackageManagerWebApiClientName { get; set; }
    public StsDatabasesBackupFilesExchangeDataModel DatabasesBackupFilesExchange { get; set; } = new();
    public int Version { get; set; }
}