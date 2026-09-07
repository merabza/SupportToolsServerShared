using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SupportToolsServerApiContracts.Models;
using SupportToolsServerApiContracts.V1.Requests;
using SupportToolsServerApiContracts.V1.Routes;
using SystemTools.ApiContracts;
using SystemTools.SharedKernel;
using SystemTools.StringMessagesApiContracts;

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
    public ValueTask<Result> UploadGitRepos(SyncGitRequest gits, CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(gits);

        return PostAsync($"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.UploadGitRepos}",
            true, bodyJsonData, cancellationToken);
    }

    public Task<Result<List<StsGitIgnoreFileTypeDataModel>>> GetGitIgnoreFileTypesList(
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsGitIgnoreFileTypeDataModel>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitIgnoreFileTypesList}", false,
            cancellationToken);
    }

    public Task<Result<List<StsGitDataModel>>> GetGitRepos(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsGitDataModel>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitRepos}", false,
            cancellationToken);
    }

    public Task<Result<StsGitDataModel>> GetGitRepoByKey(string gitKey, CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsGitDataModel>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitRepo}/{gitKey}", false,
            cancellationToken);
    }

    public async Task<Result> UpdateGitRepoByKey(string gitKey, StsGitDataModel newRecord,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(newRecord);

        return await PostAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.UpdateGitRepo}/{gitKey}", false,
            bodyJsonData, cancellationToken);
    }

    public async Task<Result> UpdateGitIgnoreFileType(string gitIgnoreFileTypeName,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.UpdateGitIgnoreFileType}/{gitIgnoreFileTypeName}",
            false, null, cancellationToken);
    }

    public async Task<Result> RemoveGitRepoByKey(string gitKey, CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.DeleteGitRepo}/{gitKey}",
            cancellationToken);
    }

    public async Task<Result> RemoveGitIgnoreFileTypeName(string gitIgnoreFileTypeName,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.DeleteGitIgnoreFileType}/{gitIgnoreFileTypeName}",
            cancellationToken);
    }

    public Task<Result<List<string>>> GetGitIgnoreFileNames(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<string>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitIgnoreFileTypesList}", false,
            cancellationToken);
    }
}