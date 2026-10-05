using System;
using System.Collections.Generic;
using System.Globalization;
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

    //რეესტრი: გარემოები. სია სახელით დალაგებულია და ყოველ ჩანაწერს თავისი Version აქვს
    public Task<Result<List<StsEnvironmentDataModel>>> GetEnvironments(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsEnvironmentDataModel>>(
            $"{SupportToolsServerApiRoutes.Environments.Base}{SupportToolsServerApiRoutes.Environments.List}", false,
            cancellationToken);
    }

    public Task<Result<StsEnvironmentDataModel>> GetEnvironment(string key,
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsEnvironmentDataModel>(
            $"{SupportToolsServerApiRoutes.Environments.Base}/{Uri.EscapeDataString(key)}", false, cancellationToken);
    }

    //upsert: environment.Version მოსალოდნელი ვერსიაა (0 — შექმნა). წარმატებისას ბრუნდება ჩანაწერის ახალი ვერსია
    public Task<Result<int>> UpdateEnvironment(string key, StsEnvironmentDataModel environment,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(environment);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.Environments.Base}{SupportToolsServerApiRoutes.Environments.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, cancellationToken);
    }

    //ვერსიით წაშლა მხოლოდ მაშინ სრულდება, თუ სერვერზე შენახული ვერსია იგივეა. version-ის გარეშე წაშლა უპირობოა
    public ValueTask<Result> DeleteEnvironment(string key, int? version, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.Environments.Base}{SupportToolsServerApiRoutes.Environments.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    //რეესტრი: Runtime-ები (RID). მეთოდები გარემოების მეთოდებივით მუშაობს
    public Task<Result<List<StsRuntimeDataModel>>> GetRuntimes(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsRuntimeDataModel>>(
            $"{SupportToolsServerApiRoutes.Runtimes.Base}{SupportToolsServerApiRoutes.Runtimes.List}", false,
            cancellationToken);
    }

    public Task<Result<StsRuntimeDataModel>> GetRuntime(string key, CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsRuntimeDataModel>(
            $"{SupportToolsServerApiRoutes.Runtimes.Base}/{Uri.EscapeDataString(key)}", false, cancellationToken);
    }

    public Task<Result<int>> UpdateRuntime(string key, StsRuntimeDataModel runtime,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(runtime);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.Runtimes.Base}{SupportToolsServerApiRoutes.Runtimes.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, cancellationToken);
    }

    //Runtime-ს, რომელსაც სერვერი იყენებს, სერვერი არ შლის: 409 RecordIsInUse
    public ValueTask<Result> DeleteRuntime(string key, int? version, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.Runtimes.Base}{SupportToolsServerApiRoutes.Runtimes.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    //რეესტრი: npm-ის პაკეტები. scoped პაკეტის სახელში (@scope/name) / წერია, key-ს escape ამიტომაც სჭირდება
    public Task<Result<List<StsNpmPackageDataModel>>> GetNpmPackages(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsNpmPackageDataModel>>(
            $"{SupportToolsServerApiRoutes.NpmPackages.Base}{SupportToolsServerApiRoutes.NpmPackages.List}", false,
            cancellationToken);
    }

    public Task<Result<StsNpmPackageDataModel>> GetNpmPackage(string key, CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsNpmPackageDataModel>(
            $"{SupportToolsServerApiRoutes.NpmPackages.Base}/{Uri.EscapeDataString(key)}", false, cancellationToken);
    }

    public Task<Result<int>> UpdateNpmPackage(string key, StsNpmPackageDataModel npmPackage,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(npmPackage);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.NpmPackages.Base}{SupportToolsServerApiRoutes.NpmPackages.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, cancellationToken);
    }

    public ValueTask<Result> DeleteNpmPackage(string key, int? version, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.NpmPackages.Base}{SupportToolsServerApiRoutes.NpmPackages.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    //რეესტრი: React აპლიკაციების შაბლონები
    public Task<Result<List<StsReactAppTemplateDataModel>>> GetReactAppTemplates(
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsReactAppTemplateDataModel>>(
            $"{SupportToolsServerApiRoutes.ReactAppTemplates.Base}{SupportToolsServerApiRoutes.ReactAppTemplates.List}",
            false, cancellationToken);
    }

    public Task<Result<StsReactAppTemplateDataModel>> GetReactAppTemplate(string key,
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsReactAppTemplateDataModel>(
            $"{SupportToolsServerApiRoutes.ReactAppTemplates.Base}/{Uri.EscapeDataString(key)}", false,
            cancellationToken);
    }

    public Task<Result<int>> UpdateReactAppTemplate(string key, StsReactAppTemplateDataModel reactAppTemplate,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(reactAppTemplate);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.ReactAppTemplates.Base}{SupportToolsServerApiRoutes.ReactAppTemplates.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, cancellationToken);
    }

    public ValueTask<Result> DeleteReactAppTemplate(string key, int? version,
        CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.ReactAppTemplates.Base}{SupportToolsServerApiRoutes.ReactAppTemplates.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    //რეესტრი: dotnet-ის ხელსაწყოები, მხოლოდ საერთო ველებით (InstalledVersion, LatestVersion და CommandName
    //კომპიუტერისაა)
    public Task<Result<List<StsDotnetToolDataModel>>> GetDotnetTools(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsDotnetToolDataModel>>(
            $"{SupportToolsServerApiRoutes.DotnetTools.Base}{SupportToolsServerApiRoutes.DotnetTools.List}", false,
            cancellationToken);
    }

    public Task<Result<StsDotnetToolDataModel>> GetDotnetTool(string key, CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsDotnetToolDataModel>(
            $"{SupportToolsServerApiRoutes.DotnetTools.Base}/{Uri.EscapeDataString(key)}", false, cancellationToken);
    }

    public Task<Result<int>> UpdateDotnetTool(string key, StsDotnetToolDataModel dotnetTool,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(dotnetTool);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.DotnetTools.Base}{SupportToolsServerApiRoutes.DotnetTools.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, cancellationToken);
    }

    public ValueTask<Result> DeleteDotnetTool(string key, int? version, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.DotnetTools.Base}{SupportToolsServerApiRoutes.DotnetTools.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    //რეესტრი: ჭკვიანი სქემები, დეტალებით. განახლება სქემას დეტალებიანად ანაცვლებს
    public Task<Result<List<StsSmartSchemaDataModel>>> GetSmartSchemas(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsSmartSchemaDataModel>>(
            $"{SupportToolsServerApiRoutes.SmartSchemas.Base}{SupportToolsServerApiRoutes.SmartSchemas.List}", false,
            cancellationToken);
    }

    public Task<Result<StsSmartSchemaDataModel>> GetSmartSchema(string key,
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsSmartSchemaDataModel>(
            $"{SupportToolsServerApiRoutes.SmartSchemas.Base}/{Uri.EscapeDataString(key)}", false, cancellationToken);
    }

    public Task<Result<int>> UpdateSmartSchema(string key, StsSmartSchemaDataModel smartSchema,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(smartSchema);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.SmartSchemas.Base}{SupportToolsServerApiRoutes.SmartSchemas.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, cancellationToken);
    }

    public ValueTask<Result> DeleteSmartSchema(string key, int? version, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.SmartSchemas.Base}{SupportToolsServerApiRoutes.SmartSchemas.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    //რეესტრი: ფაილსაცავები. ჩანაწერში პაროლია, ამიტომ განახლების ტანი შეცდომისას კონსოლზე არ იბეჭდება
    public Task<Result<List<StsFileStorageDataModel>>> GetFileStorages(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsFileStorageDataModel>>(
            $"{SupportToolsServerApiRoutes.FileStorages.Base}{SupportToolsServerApiRoutes.FileStorages.List}", false,
            cancellationToken);
    }

    public Task<Result<StsFileStorageDataModel>> GetFileStorage(string key,
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsFileStorageDataModel>(
            $"{SupportToolsServerApiRoutes.FileStorages.Base}/{Uri.EscapeDataString(key)}", false, cancellationToken);
    }

    public Task<Result<int>> UpdateFileStorage(string key, StsFileStorageDataModel fileStorage,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(fileStorage);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.FileStorages.Base}{SupportToolsServerApiRoutes.FileStorages.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, bodyContainsSecrets: true, cancellationToken);
    }

    public ValueTask<Result> DeleteFileStorage(string key, int? version, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.FileStorages.Base}{SupportToolsServerApiRoutes.FileStorages.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    //რეესტრი: API კლიენტები. ჩანაწერში API key-ა, ამიტომ განახლების ტანი შეცდომისას კონსოლზე არ იბეჭდება
    public Task<Result<List<StsApiClientDataModel>>> GetApiClients(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsApiClientDataModel>>(
            $"{SupportToolsServerApiRoutes.ApiClients.Base}{SupportToolsServerApiRoutes.ApiClients.List}", false,
            cancellationToken);
    }

    public Task<Result<StsApiClientDataModel>> GetApiClient(string key, CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsApiClientDataModel>(
            $"{SupportToolsServerApiRoutes.ApiClients.Base}/{Uri.EscapeDataString(key)}", false, cancellationToken);
    }

    public Task<Result<int>> UpdateApiClient(string key, StsApiClientDataModel apiClient,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(apiClient);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.ApiClients.Base}{SupportToolsServerApiRoutes.ApiClients.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, bodyContainsSecrets: true, cancellationToken);
    }

    //ApiClient-ს, რომელსაც სხვა ჩანაწერი იყენებს (DatabaseServerConnection, Server, GlobalSettings), სერვერი არ შლის:
    //409 RecordIsInUse
    public ValueTask<Result> DeleteApiClient(string key, int? version, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.ApiClients.Base}{SupportToolsServerApiRoutes.ApiClients.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    //რეესტრი: ბაზის სერვერებთან კავშირები, folders set-ებით. ჩანაწერში მომხმარებელი და პაროლია, ამიტომ განახლების ტანი
    //შეცდომისას კონსოლზე არ იბეჭდება
    public Task<Result<List<StsDatabaseServerConnectionDataModel>>> GetDatabaseServerConnections(
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsDatabaseServerConnectionDataModel>>(
            $"{SupportToolsServerApiRoutes.DatabaseServerConnections.Base}{SupportToolsServerApiRoutes.DatabaseServerConnections.List}",
            false, cancellationToken);
    }

    public Task<Result<StsDatabaseServerConnectionDataModel>> GetDatabaseServerConnection(string key,
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsDatabaseServerConnectionDataModel>(
            $"{SupportToolsServerApiRoutes.DatabaseServerConnections.Base}/{Uri.EscapeDataString(key)}", false,
            cancellationToken);
    }

    public Task<Result<int>> UpdateDatabaseServerConnection(string key,
        StsDatabaseServerConnectionDataModel databaseServerConnection, CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(databaseServerConnection);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.DatabaseServerConnections.Base}{SupportToolsServerApiRoutes.DatabaseServerConnections.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, bodyContainsSecrets: true, cancellationToken);
    }

    public ValueTask<Result> DeleteDatabaseServerConnection(string key, int? version,
        CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.DatabaseServerConnections.Base}{SupportToolsServerApiRoutes.DatabaseServerConnections.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    //რეესტრი: სერვერები. ჩანაწერში საიდუმლო არ არის (FilesUserName სერვისის OS ანგარიშია, პაროლის გარეშე), ამიტომ
    //განახლების ტანი შეცდომისას კონსოლზე იბეჭდება. სერვერის გადარქმევა წაშლა და ახლის შექმნაა
    public Task<Result<List<StsServerDataModel>>> GetServers(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsServerDataModel>>(
            $"{SupportToolsServerApiRoutes.Servers.Base}{SupportToolsServerApiRoutes.Servers.List}", false,
            cancellationToken);
    }

    public Task<Result<StsServerDataModel>> GetServer(string key, CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsServerDataModel>(
            $"{SupportToolsServerApiRoutes.Servers.Base}/{Uri.EscapeDataString(key)}", false, cancellationToken);
    }

    public Task<Result<int>> UpdateServer(string key, StsServerDataModel server,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(server);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.Servers.Base}{SupportToolsServerApiRoutes.Servers.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, cancellationToken);
    }

    public ValueTask<Result> DeleteServer(string key, int? version, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.Servers.Base}{SupportToolsServerApiRoutes.Servers.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    //რეესტრი: გლობალური პარამეტრები. ჩანაწერი ერთადერთია (singleton) და გასაღები არ აქვს. სანამ ის შეიქმნება, სერვერი
    //ცარიელ კონტრაქტს აბრუნებს Version = 0-ით. ჩანაწერში MediatRLicenseKey-ია, ამიტომ განახლების ტანი შეცდომისას
    //კონსოლზე არ იბეჭდება
    public Task<Result<StsGlobalSettingsDataModel>> GetGlobalSettings(CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsGlobalSettingsDataModel>(
            $"{SupportToolsServerApiRoutes.GlobalSettings.Base}{SupportToolsServerApiRoutes.GlobalSettings.Get}", false,
            cancellationToken);
    }

    //upsert: globalSettings.Version მოსალოდნელი ვერსიაა (0 — პირველი შექმნა). წარმატებისას ბრუნდება ახალი ვერსია
    public Task<Result<int>> UpdateGlobalSettings(StsGlobalSettingsDataModel globalSettings,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(globalSettings);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.GlobalSettings.Base}{SupportToolsServerApiRoutes.GlobalSettings.Update}",
            false, bodyJsonData, bodyContainsSecrets: true, cancellationToken);
    }

    //რეესტრი: პროექტის შემქმნელის პარამეტრები, GlobalSettings-ის მსგავსი singleton. ჩანაწერში საიდუმლო არ არის, ამიტომ
    //განახლების ტანი შეცდომისას კონსოლზე იბეჭდება
    public Task<Result<StsProjectCreatorSettingsDataModel>> GetProjectCreatorSettings(
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsProjectCreatorSettingsDataModel>(
            $"{SupportToolsServerApiRoutes.ProjectCreatorSettings.Base}{SupportToolsServerApiRoutes.ProjectCreatorSettings.Get}",
            false, cancellationToken);
    }

    public Task<Result<int>> UpdateProjectCreatorSettings(StsProjectCreatorSettingsDataModel projectCreatorSettings,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(projectCreatorSettings);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.ProjectCreatorSettings.Base}{SupportToolsServerApiRoutes.ProjectCreatorSettings.Update}",
            false, bodyJsonData, cancellationToken);
    }

    //რეესტრი: პროექტის შაბლონები. ჩანაწერში საიდუმლო არ არის
    public Task<Result<List<StsProjectTemplateDataModel>>> GetProjectTemplates(
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<List<StsProjectTemplateDataModel>>(
            $"{SupportToolsServerApiRoutes.ProjectTemplates.Base}{SupportToolsServerApiRoutes.ProjectTemplates.List}",
            false, cancellationToken);
    }

    public Task<Result<StsProjectTemplateDataModel>> GetProjectTemplate(string key,
        CancellationToken cancellationToken = default)
    {
        return GetAsyncReturn<StsProjectTemplateDataModel>(
            $"{SupportToolsServerApiRoutes.ProjectTemplates.Base}/{Uri.EscapeDataString(key)}", false,
            cancellationToken);
    }

    public Task<Result<int>> UpdateProjectTemplate(string key, StsProjectTemplateDataModel projectTemplate,
        CancellationToken cancellationToken = default)
    {
        var bodyJsonData = JsonConvert.SerializeObject(projectTemplate);

        return PostAsyncReturn<int>(
            $"{SupportToolsServerApiRoutes.ProjectTemplates.Base}{SupportToolsServerApiRoutes.ProjectTemplates.UpdatePrefix}/{Uri.EscapeDataString(key)}",
            false, bodyJsonData, cancellationToken);
    }

    public ValueTask<Result> DeleteProjectTemplate(string key, int? version,
        CancellationToken cancellationToken = default)
    {
        return DeleteAsync(
            $"{SupportToolsServerApiRoutes.ProjectTemplates.Base}{SupportToolsServerApiRoutes.ProjectTemplates.DeletePrefix}/{Uri.EscapeDataString(key)}{VersionQuery(version)}",
            cancellationToken);
    }

    private static string VersionQuery(int? version)
    {
        return version is null ? string.Empty : $"?version={version.Value.ToString(CultureInfo.InvariantCulture)}";
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