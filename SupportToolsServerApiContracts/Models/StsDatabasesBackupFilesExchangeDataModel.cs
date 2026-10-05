namespace SupportToolsServerApiContracts.Models;

//ბაზების backup ფაილების გაცვლის პარამეტრები, StsGlobalSettingsDataModel-ის ნაწილი: კლიენტის
//DatabasesBackupFilesExchangeParameters, LocalPath-ის გარეშე (ის კომპიუტერის ფოლდერია). ExchangeFileStorageName
//ფაილსაცავის სახელია, ExchangeSmartSchemaName და LocalSmartSchemaName ჭკვიანი სქემებისა; null ან ცარიელი ნიშნავს, რომ
//მითითება არ არის
public sealed class StsDatabasesBackupFilesExchangeDataModel
{
    public string? DownloadTempExtension { get; set; }
    public string? UploadTempExtension { get; set; }
    public string? ExchangeFileStorageName { get; set; }
    public string? ExchangeSmartSchemaName { get; set; }
    public string? LocalSmartSchemaName { get; set; }
}
