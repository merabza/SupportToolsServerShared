namespace SupportToolsServerApiContracts.Models;

//ბაზის პარამეტრები, კლიენტის DatabaseParameters: StsProjectDataModel-ის DevDatabaseParameters და
//ProdCopyDatabaseParameters, ასევე StsServerInfoDataModel-ის CurrentDatabaseParameters და NewDatabaseParameters.
//DbConnectionName ბაზის კავშირის, SmartSchemaName ჭკვიანი სქემის, FileStorageName კი ფაილსაცავის სახელია; null ან
//ცარიელი ნიშნავს, რომ მითითება არ არის, არარსებული სახელები კი 404 ReferencedRecordsNotFound-ია. DbServerFoldersSetName
//ბაზის კავშირის ფოლდერების ნაკრების სახელია, რომელსაც სერვერი არ ამოწმებს. DatabaseRecoveryModel და BackupType კლიენტის
//EDatabaseRecoveryModel-ისა და EBackupType-ის სახელებია
public sealed class StsDatabaseParametersDataModel
{
    public string? DbConnectionName { get; set; }
    public string? DatabaseRecoveryModel { get; set; }
    public string? DbServerFoldersSetName { get; set; }
    public string? DatabaseName { get; set; }
    public string? SmartSchemaName { get; set; }
    public string? FileStorageName { get; set; }
    public int CommandTimeOut { get; set; }
    public bool SkipBackupBeforeRestore { get; set; }
    public string? BackupNamePrefix { get; set; }
    public string? DateMask { get; set; }
    public string? BackupFileExtension { get; set; }
    public string? BackupNameMiddlePart { get; set; }
    public bool? Compress { get; set; }
    public bool? Verify { get; set; }
    public string? BackupType { get; set; }
}