using SystemTools.SystemToolsShared.Errors;

namespace SupportToolsServerApiContracts.Errors;

public static class SupportToolsServerApiClientErrors
{
    //public static readonly Err DatabasesBackupFilesExchangeParametersIsNotConfigured = new()
    //{
    //    ErrorCode = nameof(DatabasesBackupFilesExchangeParametersIsNotConfigured),
    //    ErrorMessage = "Databases Backup Files Exchange Parameters Is Not Configured"
    //};

    //public static readonly Err BaseBackupParametersIsNotCreated = new()
    //{
    //    ErrorCode = nameof(BaseBackupParametersIsNotCreated), ErrorMessage = "Base Backup Parameters Is Not Created"
    //};

    //public static readonly Err BackupFileParametersIsNull = new()
    //{
    //    ErrorCode = nameof(BackupFileParametersIsNull), ErrorMessage = "BackupFileParameters Is Null"
    //};

    //public static readonly Err ErrorWhenRestoreDatabase = new()
    //{
    //    ErrorCode = nameof(ErrorWhenRestoreDatabase), ErrorMessage = "Error When Restore Database"
    //};

    //public static readonly Err DatabaseServerDataIsNotConfigured = new()
    //{
    //    ErrorCode = nameof(DatabaseServerDataIsNotConfigured),
    //    ErrorMessage = "Database Server Data Is Not Configured"
    //};

    public static Err GitWithKeyNotFound(string gitKey)
    {
        return new Err { ErrorCode = nameof(GitWithKeyNotFound), ErrorMessage = $"Git With Key {gitKey} Not Found" };
    }
}