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
using SupportToolsServerApiContracts.V1.Requests;
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

    public Task<OneOf<List<string>, IEnumerable<Err>>> GetGitIgnoreFileTypesList(
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<string>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitIgnoreFileTypesList}", false,
            cancellationToken);
    }

    public Task<OneOf<List<GitDataDto>, IEnumerable<Err>>> GetGitRepos(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<GitDataDto>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitRepos}", false,
            cancellationToken);
    }

    public Task<OneOf<GitDataDto, IEnumerable<Err>>> GetGitRepoByKey(string gitKey,
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<GitDataDto>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitRepo}/{gitKey}", false,
            cancellationToken);
    }

    public async Task<Option<IEnumerable<Err>>> UpdateGitRepoByKey(string gitKey, GitDataDto newRecord,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(newRecord);

        return await PostAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.UpdateGitRepo}/{gitKey}", false,
            bodyJsonData, cancellationToken);
    }

    public async Task<Option<IEnumerable<Err>>> AddGitIgnoreFileTypeNameIfNotExists(string gitIgnoreFileTypeName,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.AddGitIgnoreFileTypeNameIfNotExists}/{gitIgnoreFileTypeName}",
            false, null, cancellationToken);
    }

    public async Task<Option<IEnumerable<Err>>> RemoveGitRepoByKey(string gitKey,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.DeleteGitRepo}/{gitKey}",
            cancellationToken);
    }

    public async Task<Option<IEnumerable<Err>>> RemoveGitIgnoreFileTypeName(string gitIgnoreFileTypeName,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.DeleteGitIgnoreFileType}/{gitIgnoreFileTypeName}",
            cancellationToken);
    }

    //public Task<OneOf<List<GitIgnoreFilePathDomain>, IEnumerable<Err>>> GetGitIgnoreFilePaths(CancellationToken cancellationToken = default)
    //{
    //    return GetAsyncReturn<List<GitIgnoreFilePathDomain>>(
    //        $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GetGitIgnoreFilePaths}", false,
    //        cancellationToken);
    //}

    public Task<OneOf<List<string>, IEnumerable<Err>>> GetGitIgnoreFileNames(
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<string>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitIgnoreFileTypesList}", false,
            cancellationToken);
    }
}