using SystemTools.SharedKernel;

namespace SupportToolsServerApiContracts.Errors;

public static class SupportToolsServerApiClientErrors
{
    //public static readonly Error DatabasesBackupFilesExchangeParametersIsNotConfigured = new()
    //{
    //    Code = nameof(DatabasesBackupFilesExchangeParametersIsNotConfigured),
    //    Name = "Databases Backup Files Exchange Parameters Is Not Configured"
    //};

    //public static readonly Error BaseBackupParametersIsNotCreated = new()
    //{
    //    Code = nameof(BaseBackupParametersIsNotCreated), Name = "Base Backup Parameters Is Not Created"
    //};

    //public static readonly Error BackupFileParametersIsNull = new()
    //{
    //    Code = nameof(BackupFileParametersIsNull), Name = "BackupFileParameters Is Null"
    //};

    //public static readonly Error ErrorWhenRestoreDatabase = new()
    //{
    //    Code = nameof(ErrorWhenRestoreDatabase), Name = "Error When Restore Database"
    //};

    //public static readonly Error DatabaseServerDataIsNotConfigured = new()
    //{
    //    Code = nameof(DatabaseServerDataIsNotConfigured),
    //    Name = "Database Server Data Is Not Configured"
    //};

    public static Error GitWithKeyNotFound(string gitKey)
    {
        return Error.NotFound(nameof(GitWithKeyNotFound), $"Git With Key {gitKey} Not Found");
    }
}