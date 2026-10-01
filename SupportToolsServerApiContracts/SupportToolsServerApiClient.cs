using System;
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

    //merge=false-ის დროს სერვერზე წაიშლება ის ჩანაწერები, რომლებიც ატვირთულ სიაში არ არის
    public ValueTask<Result> SyncUpGitIgnoreFileTypes(List<StsGitIgnoreFileTypeDataModel> gitIgnoreFileTypes,
        bool merge, CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(gitIgnoreFileTypes);

        return PostAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.SyncUpGitIgnoreFileTypesPrefix}/{merge}",
            false, bodyJsonData, cancellationToken);
    }

    //merge=false-ის დროს სერვერზე წაიშლება ის ჩანაწერები, რომლებიც ატვირთულ სიაში არ არის
    public ValueTask<Result> SyncUpEditorConfigFileTypes(List<StsEditorConfigFileTypeDataModel> editorConfigFileTypes,
        bool merge, CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(editorConfigFileTypes);

        return PostAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.SyncUpEditorConfigFileTypesPrefix}/{merge}",
            false, bodyJsonData, cancellationToken);
    }

    public Task<Result<List<StsGitIgnoreFileTypeDataModel>>> GetGitIgnoreFileTypesList(
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsGitIgnoreFileTypeDataModel>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitIgnoreFileTypesList}", false,
            cancellationToken);
    }

    public Task<Result<List<StsEditorConfigFileTypeDataModel>>> GetEditorConfigFileTypesList(
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsEditorConfigFileTypeDataModel>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.EditorConfigFileTypesList}",
            false, cancellationToken);
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
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitRepoPrefix}/{Uri.EscapeDataString(gitKey)}",
            false, cancellationToken);
    }

    public async Task<Result> UpdateGitRepoByKey(string gitKey, StsGitDataModel newRecord,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(newRecord);

        return await PostAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.UpdateGitRepoPrefix}/{Uri.EscapeDataString(gitKey)}",
            false, bodyJsonData, cancellationToken);
    }

    public async Task<Result> UpdateGitIgnoreFileType(string gitIgnoreFileTypeName,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.UpdateGitIgnoreFileTypePrefix}/{Uri.EscapeDataString(gitIgnoreFileTypeName)}",
            false, null, cancellationToken);
    }

    public async Task<Result> RemoveGitRepoByKey(string gitKey, CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.DeleteGitRepoPrefix}/{Uri.EscapeDataString(gitKey)}",
            cancellationToken);
    }

    public async Task<Result> RemoveGitIgnoreFileTypeName(string gitIgnoreFileTypeName,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.DeleteGitIgnoreFileTypePrefix}/{Uri.EscapeDataString(gitIgnoreFileTypeName)}",
            cancellationToken);
    }

    public async Task<Result> RemoveEditorConfigFileTypeName(string editorConfigFileTypeName,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.DeleteEditorConfigFileTypePrefix}/{Uri.EscapeDataString(editorConfigFileTypeName)}",
            cancellationToken);
    }

    public Task<Result<List<string>>> GetGitIgnoreFileNames(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<string>>(
            $"{SupportToolsServerApiRoutes.Git.GitBase}{SupportToolsServerApiRoutes.Git.GitIgnoreFileTypesList}", false,
            cancellationToken);
    }
}