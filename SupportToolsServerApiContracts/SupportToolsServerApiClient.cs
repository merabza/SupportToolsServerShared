using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApiContracts;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StringMessagesApiContracts;
using SupportToolsServerApiContracts.Models;
using SupportToolsServerApiContracts.V1.Routes;
using SystemToolsShared.Errors;

namespace SupportToolsServerApiContracts;

public sealed class SupportToolsServerApiClient : ApiClient
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public SupportToolsServerApiClient(ILogger? logger, IHttpClientFactory httpClientFactory, string server,
        string? apiKey, bool useConsole) : base(logger, httpClientFactory, server, apiKey,
        new StringMessageHubClient(server, apiKey), useConsole)
    {
    }

    //შემოწმდეს არსებული ბაზის მდგომარეობა და საჭიროების შემთხვევაში გამოასწოროს ბაზა
    public ValueTask<Option<IEnumerable<Err>>> UploadGitRepos(SyncGitRequest gits,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(gits);

        return PostAsync(SupportToolsServerApiRoutes.Git.UploadGitRepos, true, bodyJsonData, cancellationToken);
    }

    ////დამზადდეს ბაზის სარეზერვო ასლი სერვერის მხარეს.
    ////ასევე ამ მეთოდის ამოცანაა უზრუნველყოს ბექაპის ჩამოსაქაჩად ხელმისაწვდომ ადგილას მოხვედრა
    //public Task<OneOf<BackupFileParameters, IEnumerable<Err>>> CreateBackup(
    //    DatabaseBackupParametersDomain databaseBackupParameters, string backupBaseName, string dbServerFoldersSetName,
    //    CancellationToken cancellationToken = default)
    //{
    //    return PostAsyncReturn<BackupFileParameters>(
    //        $"{DatabaseApiRoutes.Database.DatabaseBase}{DatabaseApiRoutes.Database.CreateBackupPrefix}/{backupBaseName}/{dbServerFoldersSetName}",
    //        true, JsonConvert.SerializeObject(databaseBackupParameters), cancellationToken);
    //}

    ////სერვერის მხარეს მონაცემთა ბაზაში ბრძანების გაშვება
    //public ValueTask<Option<IEnumerable<Err>>> ExecuteCommand(string executeQueryCommand, string? databaseName = null,
    //    CancellationToken cancellationToken = default)
    //{
    //    return PostAsync(
    //        $"{DatabaseApiRoutes.Database.DatabaseBase}{DatabaseApiRoutes.Database.ExecuteCommandPrefix}{(string.IsNullOrWhiteSpace(databaseName) ? string.Empty : $"/{databaseName}")}",
    //        true, executeQueryCommand, cancellationToken);
    //}

    ////მონაცემთა ბაზების სიის მიღება სერვერიდან
    //public Task<OneOf<List<DatabaseInfoModel>, IEnumerable<Err>>> GetDatabaseNames(
    //    CancellationToken cancellationToken = default)
    //{
    //    return GetAsyncReturn<List<DatabaseInfoModel>>(
    //        DatabaseApiRoutes.Database.DatabaseBase + DatabaseApiRoutes.Database.GetDatabaseNames, false,
    //        cancellationToken);
    //}

    ////გამოიყენება ბაზის დამაკოპირებელ ინსტრუმენტში, იმის დასადგენად,
    ////მიზნის ბაზა უკვე არსებობს თუ არა, რომ არ მოხდეს ამ ბაზის ისე წაშლა ახლით,
    ////რომ არსებულის გადანახვა არ მოხდეს.
    //public Task<OneOf<bool, IEnumerable<Err>>> IsDatabaseExists(string databaseName,
    //    CancellationToken cancellationToken = default)
    //{
    //    return GetAsyncReturn<bool>(
    //        $"{DatabaseApiRoutes.Database.DatabaseBase}{DatabaseApiRoutes.Database.IsDatabaseExistsPrefix}/{databaseName}",
    //        false, cancellationToken);
    //}

    ////გამოიყენება ბაზის დამაკოპირებელ ინსტრუმენტში, დაკოპირებული ბაზის აღსადგენად,
    //public Task<Option<IEnumerable<Err>>> RestoreDatabaseFromBackup(string prefix, string suffix, string name,
    //    string dateMask, string databaseName, string dbServerFoldersSetName,
    //    EDatabaseRecoveryModel databaseRecoveryModel, CancellationToken cancellationToken = default)
    //{
    //    var bodyJsonData = JsonConvert.SerializeObject(new RestoreBackupRequest
    //    {
    //        Prefix = prefix,
    //        Suffix = suffix,
    //        Name = name,
    //        DateMask = dateMask,
    //        DatabaseRecoveryModel = databaseRecoveryModel
    //    });
    //    return PutAsync(
    //        $"{DatabaseApiRoutes.Database.DatabaseBase}{DatabaseApiRoutes.Database.RestoreBackupPrefix}/{databaseName}/{dbServerFoldersSetName}",
    //        bodyJsonData, cancellationToken);
    //}

    ////მონაცემთა ბაზაში არსებული პროცედურების რეკომპილირება
    //public ValueTask<Option<IEnumerable<Err>>> RecompileProcedures(string databaseName,
    //    CancellationToken cancellationToken = default)
    //{
    //    return PostAsync(
    //        $"{DatabaseApiRoutes.Database.DatabaseBase}{DatabaseApiRoutes.Database.RecompileProceduresPrefix}/{databaseName}",
    //        cancellationToken);
    //}

    //public Task<Option<IEnumerable<Err>>> TestConnection(string? databaseName,
    //    CancellationToken cancellationToken = default)
    //{
    //    return GetAsync(
    //        $"{DatabaseApiRoutes.Database.DatabaseBase}{DatabaseApiRoutes.Database.TestConnectionPrefix}{(databaseName == null ? string.Empty : $"/{databaseName}")}",
    //        cancellationToken);
    //}

    ////მონაცემთა ბაზაში არსებული სტატისტიკების დაანგარიშება
    //public ValueTask<Option<IEnumerable<Err>>> UpdateStatistics(string databaseName,
    //    CancellationToken cancellationToken = default)
    //{
    //    return PostAsync(
    //        $"{DatabaseApiRoutes.Database.DatabaseBase}{DatabaseApiRoutes.Database.UpdateStatisticsPrefix}/{databaseName}",
    //        cancellationToken);
    //}

    //public Task<OneOf<List<string>, IEnumerable<Err>>> GetDatabaseFoldersSetNames(CancellationToken cancellationToken)
    //{
    //    return GetAsyncReturn<List<string>>(
    //        DatabaseApiRoutes.Database.DatabaseBase + DatabaseApiRoutes.Database.GetDatabaseFoldersSetNames, true,
    //        cancellationToken);
    //}

    //public Task<OneOf<List<string>, IEnumerable<Err>>> GetDatabaseConnectionNames(CancellationToken cancellationToken)
    //{
    //    return GetAsyncReturn<List<string>>(
    //        DatabaseApiRoutes.Database.DatabaseBase + DatabaseApiRoutes.Database.GetDatabaseConnectionNames, false,
    //        cancellationToken);
    //}

    //public ValueTask<Option<IEnumerable<Err>>> ChangeDatabaseRecoveryModel(string databaseName,
    //    EDatabaseRecoveryModel databaseRecoveryModel, CancellationToken cancellationToken)
    //{
    //    return PostAsync(
    //        $"{DatabaseApiRoutes.Database.DatabaseBase}{DatabaseApiRoutes.Database.ChangeDatabaseRecoveryModel}/{databaseName}/{databaseRecoveryModel.ToString()}",
    //        cancellationToken);
    //}
}