using SystemTools.SystemToolsShared.Errors;

namespace SupportToolsServerApiContracts.Errors;

public static class SupportToolsServerApiClientErrors
{
    //public static readonly ErrorOmd DatabasesBackupFilesExchangeParametersIsNotConfigured = new()
    //{
    //    Code = nameof(DatabasesBackupFilesExchangeParametersIsNotConfigured),
    //    Name = "Databases Backup Files Exchange Parameters Is Not Configured"
    //};

    //public static readonly ErrorOmd BaseBackupParametersIsNotCreated = new()
    //{
    //    Code = nameof(BaseBackupParametersIsNotCreated), Name = "Base Backup Parameters Is Not Created"
    //};

    //public static readonly ErrorOmd BackupFileParametersIsNull = new()
    //{
    //    Code = nameof(BackupFileParametersIsNull), Name = "BackupFileParameters Is Null"
    //};

    //public static readonly ErrorOmd ErrorWhenRestoreDatabase = new()
    //{
    //    Code = nameof(ErrorWhenRestoreDatabase), Name = "ErrorOmd When Restore Database"
    //};

    //public static readonly ErrorOmd DatabaseServerDataIsNotConfigured = new()
    //{
    //    Code = nameof(DatabaseServerDataIsNotConfigured),
    //    Name = "Database Server Data Is Not Configured"
    //};

    public static ErrorOmd GitWithKeyNotFound(string gitKey)
    {
        return new ErrorOmd { Code = nameof(GitWithKeyNotFound), Name = $"Git With Key {gitKey} Not Found" };
    }
}