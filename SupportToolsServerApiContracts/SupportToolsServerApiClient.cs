using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApiContracts;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OneOf;
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

        return PostAsync($"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.UploadGitRepos}",
            true, bodyJsonData, cancellationToken);
    }

    public Task<OneOf<List<GitDataDomain>, IEnumerable<Err>>> GetGitRepos(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<GitDataDomain>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitRepos}", false,
            cancellationToken);
    }

    public Task<OneOf<GitDataDomain, IEnumerable<Err>>> GetGitRepoByKey(string recordKey,
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<GitDataDomain>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GetGitRepo}/{recordKey}", false,
            cancellationToken);
    }

    public async Task<Option<IEnumerable<Err>>> UpdateGitRepoByKey(string recordKey, GitDataDomain newRecord,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(newRecord);

        return await PostAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.UpdateGitRepo}/{recordKey}",
            false, bodyJsonData, cancellationToken);
    }

    public async Task<Option<IEnumerable<Err>>> RemoveGitRepoByKey(string recordKey,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.DeleteGitRepo}/{recordKey}",
            cancellationToken);
    }

    //public Task<OneOf<List<GitIgnoreFilePathDomain>, IEnumerable<Err>>> GetGitIgnoreFilePaths(CancellationToken cancellationToken = default)
    //{
    //    return GetAsyncReturn<List<GitIgnoreFilePathDomain>>(
    //        $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GetGitIgnoreFilePaths}", false,
    //        cancellationToken);
    //}

    public Task<OneOf<List<string>, IEnumerable<Err>>> GetGitIgnoreFileNames(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<string>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitIgnoreFileNames}", false,
            cancellationToken);
    }





}