using SystemTools.SharedKernel;

namespace SupportToolsServerApiContracts.Errors;

public static class SupportToolsServerApiClientErrors
{
    //public static readonly Error DatabasesBackupFilesExchangeParametersIsNotConfigured =
    //    Error.Problem(nameof(DatabasesBackupFilesExchangeParametersIsNotConfigured),
    //        "Databases Backup Files Exchange Parameters Is Not Configured");

    //public static readonly Error BaseBackupParametersIsNotCreated =
    //    Error.Problem(nameof(BaseBackupParametersIsNotCreated), "Base Backup Parameters Is Not Created");

    //public static readonly Error BackupFileParametersIsNull =
    //    Error.Problem(nameof(BackupFileParametersIsNull), "BackupFileParameters Is Null");

    //public static readonly Error ErrorWhenRestoreDatabase =
    //    Error.Problem(nameof(ErrorWhenRestoreDatabase), "Error When Restore Database");

    //public static readonly Error DatabaseServerDataIsNotConfigured =
    //    Error.Problem(nameof(DatabaseServerDataIsNotConfigured), "Database Server Data Is Not Configured");

    public static Error GitWithKeyNotFound(string gitKey)
    {
        return Error.NotFound(nameof(GitWithKeyNotFound), $"Git With Key {gitKey} Not Found");
    }
}
