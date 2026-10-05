using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SupportToolsServerApiContracts.Models;
using SupportToolsServerApiContracts.Tests.TestDoubles;
using SupportToolsServerApiContracts.V1.Requests;
using SystemTools.SharedKernel;
using Xunit;

namespace SupportToolsServerApiContracts.Tests;

//The tests of one class run one after another, so replacing Console.Out here is safe
public sealed class SupportToolsServerApiClientTests
{
    //The message hub really connects to the server. On port 0 the connection fails at once and the hub writes that
    //to the console, which shows whether the method started the hub (on a closed port Windows waits ~2 seconds)
    private const string Server = "http://127.0.0.1:0/api/v1";

    private const string GitIgnoreListJson =
        """[{"Id":"11111111-1111-1111-1111-111111111111","Name":"CSharp","Content":"bin/"}]""";

    private const string GitRepoJson =
        """{"GitProjectName":"RepoA","GitProjectAddress":"git@github.com:x/a.git","GitProjectFolderName":"A","GitIgnorePatternName":"CSharp"}""";

    private const string NotFoundProblemJson =
        """{"type":"https://tools.ietf.org/html/rfc7231#section-6.5.4","title":"GitWithKeyNotFound","status":404,"detail":"Git With Key Repo X/1 Not Found"}""";

    private static SupportToolsServerApiClient CreateClient(HttpMessageHandler handler)
    {
        return new SupportToolsServerApiClient(null, new FakeHttpClientFactory(handler), Server, null, false);
    }

    //SupportTools creates its client with the console, which writes the errors of the server
    private static SupportToolsServerApiClient CreateConsoleClient(HttpMessageHandler handler)
    {
        return new SupportToolsServerApiClient(null, new FakeHttpClientFactory(handler), Server, null, true);
    }

    private static StubHttpMessageHandler ConflictHandler(string entityName)
    {
        return new StubHttpMessageHandler(HttpStatusCode.Conflict,
            $$"""{"title":"ConcurrencyConflict","status":409,"detail":"{{entityName}} A Version Conflict: Expected 1, Actual 2"}""",
            "application/problem+json");
    }

    private static StsGitDataModel GitRepo(string name)
    {
        return new StsGitDataModel
        {
            GitProjectName = name,
            GitProjectAddress = "git@github.com:x/a.git",
            GitProjectFolderName = "A",
            GitIgnorePatternName = "CSharp"
        };
    }

    private static async Task<(T Result, string Output)> CaptureConsole<T>(Func<Task<T>> action)
    {
        TextWriter original = Console.Out;
        await using var writer = new StringWriter(CultureInfo.InvariantCulture);
        Console.SetOut(writer);
        try
        {
            T result = await action();
            return (result, writer.ToString());
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    [Fact]
    public async Task UploadGitRepos_PostsTheRequestAndStartsTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);
        var request = new SyncGitRequest
        {
            Gits = [GitRepo("RepoA")],
            GitIgnoreFiles = [new StsGitIgnoreFileTypeDataModel { Name = "CSharp", Content = "bin/" }]
        };

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).UploadGitRepos(request));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/uploadgitrepos", handler.LastRequestUri!.AbsolutePath);
        SyncGitRequest sent = JsonConvert.DeserializeObject<SyncGitRequest>(handler.LastRequestBody!)!;
        Assert.Equal("RepoA", Assert.Single(sent.Gits).GitProjectName);
        Assert.Equal("bin/", Assert.Single(sent.GitIgnoreFiles).Content);
    }

    [Fact]
    public async Task UploadGitRepos_ReturnsTheServerError()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.Conflict,
            """{"title":"GitAddressIsInUse","status":409,"detail":"Git Address a Is Used By B"}""",
            "application/problem+json");

        (Result result, _) = await CaptureConsole(async () =>
            await CreateClient(handler).UploadGitRepos(new SyncGitRequest { Gits = [], GitIgnoreFiles = [] }));

        Assert.Equal("GitAddressIsInUse", result.Error.Code);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Theory]
    [InlineData(false, "/api/v1/git/syncupgitignorefiletypes/False")]
    [InlineData(true, "/api/v1/git/syncupgitignorefiletypes/True")]
    public async Task SyncUpGitIgnoreFileTypes_PostsTheListWithTheMergeFlagInTheRoute(bool merge, string path)
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);
        List<StsGitIgnoreFileTypeDataModel> list = [new() { Name = "CSharp", Content = "bin/" }];

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).SyncUpGitIgnoreFileTypes(list, merge));

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal(path, handler.LastRequestUri!.AbsolutePath);
        List<StsGitIgnoreFileTypeDataModel> sent =
            JsonConvert.DeserializeObject<List<StsGitIgnoreFileTypeDataModel>>(handler.LastRequestBody!)!;
        Assert.Equal("CSharp", Assert.Single(sent).Name);
    }

    [Theory]
    [InlineData(false, "/api/v1/git/syncupeditorconfigfiletypes/False")]
    [InlineData(true, "/api/v1/git/syncupeditorconfigfiletypes/True")]
    public async Task SyncUpEditorConfigFileTypes_PostsTheListWithTheMergeFlagInTheRoute(bool merge, string path)
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);
        List<StsEditorConfigFileTypeDataModel> list = [new() { Name = "default", Content = "root = true" }];

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).SyncUpEditorConfigFileTypes(list, merge));

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal(path, handler.LastRequestUri!.AbsolutePath);
        StsEditorConfigFileTypeDataModel sent = Assert.Single(
            JsonConvert.DeserializeObject<List<StsEditorConfigFileTypeDataModel>>(handler.LastRequestBody!)!);
        Assert.Equal("default", sent.Name);
        Assert.Equal("root = true", sent.Content);
    }

    [Fact]
    public async Task SyncUpEditorConfigFileTypes_ReturnsTheServerError()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.BadRequest,
            """{"title":"ValueTooLong","status":400,"detail":"default.Content Is Longer Than 65536 Characters"}""",
            "application/problem+json");

        (Result result, _) = await CaptureConsole(async () =>
            await CreateClient(handler).SyncUpEditorConfigFileTypes([], false));

        Assert.Equal("ValueTooLong", result.Error.Code);
        Assert.Equal("default.Content Is Longer Than 65536 Characters", result.Error.Description);
    }

    [Fact]
    public async Task GetGitIgnoreFileTypesList_GetsTheListWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, GitIgnoreListJson);

        (Result<List<StsGitIgnoreFileTypeDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetGitIgnoreFileTypesList());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/gitignorefiletypeslist", handler.LastRequestUri!.AbsolutePath);
        StsGitIgnoreFileTypeDataModel gitIgnoreFileType = Assert.Single(result.Value);
        Assert.Equal(new Guid("11111111-1111-1111-1111-111111111111"), gitIgnoreFileType.Id);
        Assert.Equal("CSharp", gitIgnoreFileType.Name);
    }

    [Fact]
    public async Task GetEditorConfigFileTypesList_GetsTheListWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"Name":"CSharp","Content":"root = true"}]""");

        (Result<List<StsEditorConfigFileTypeDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetEditorConfigFileTypesList());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/editorconfigfiletypeslist", handler.LastRequestUri!.AbsolutePath);
        StsEditorConfigFileTypeDataModel editorConfigFileType = Assert.Single(result.Value);
        Assert.Equal("CSharp", editorConfigFileType.Name);
        Assert.Equal("root = true", editorConfigFileType.Content);
    }

    [Fact]
    public async Task GetGitRepos_GetsTheListWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, $"[{GitRepoJson}]");

        (Result<List<StsGitDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetGitRepos());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/gitrepos", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("CSharp", Assert.Single(result.Value).GitIgnorePatternName);
    }

    [Fact]
    public async Task GetGitRepoByKey_GetsTheRepoOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, GitRepoJson);

        (Result<StsGitDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetGitRepoByKey("Repo X/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/gitrepo/Repo%20X%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("RepoA", result.Value.GitProjectName);
    }

    [Fact]
    public async Task GetGitRepoByKey_ReturnsGitWithKeyNotFound()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.NotFound, NotFoundProblemJson,
            "application/problem+json");

        Result<StsGitDataModel> result = await CreateClient(handler).GetGitRepoByKey("Repo X/1");

        Assert.Equal("GitWithKeyNotFound", result.Error.Code);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task UpdateGitRepoByKey_PostsTheRecordToTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) = await CaptureConsole(async () =>
            await CreateClient(handler).UpdateGitRepoByKey("Repo X/1", GitRepo("Repo X/1")));

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/updategitrepo/Repo%20X%2F1", handler.LastRequestUri!.AbsolutePath);
        StsGitDataModel sent = JsonConvert.DeserializeObject<StsGitDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("Repo X/1", sent.GitProjectName);
        Assert.Equal("git@github.com:x/a.git", sent.GitProjectAddress);
    }

    [Fact]
    public async Task UpdateGitIgnoreFileType_PostsTheEscapedNameWithoutBodyAndWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).UpdateGitIgnoreFileType("C Sharp"));

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/updategitignorefiletype/C%20Sharp", handler.LastRequestUri!.AbsolutePath);
        Assert.Null(handler.LastRequestBody);
    }

    [Fact]
    public async Task RemoveGitRepoByKey_DeletesTheEscapedKeyAndStartsTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).RemoveGitRepoByKey("Repo X/1"));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/deletegitrepo/Repo%20X%2F1", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task RemoveGitIgnoreFileTypeName_DeletesTheEscapedNameAndStartsTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).RemoveGitIgnoreFileTypeName("C Sharp"));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/deletegitignorefiletype/C%20Sharp", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task RemoveGitIgnoreFileTypeName_ReturnsTheServerError()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.Conflict,
            """{"title":"GitIgnoreFileTypeIsInUse","status":409,"detail":"GitIgnore File Type Is Used By Gits: CSharp (RepoA)"}""",
            "application/problem+json");

        (Result result, _) =
            await CaptureConsole(async () => await CreateClient(handler).RemoveGitIgnoreFileTypeName("CSharp"));

        Assert.Equal("GitIgnoreFileTypeIsInUse", result.Error.Code);
    }

    [Fact]
    public async Task RemoveEditorConfigFileTypeName_DeletesTheEscapedNameAndStartsTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).RemoveEditorConfigFileTypeName("Ba Getter"));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/deleteeditorconfigfiletype/Ba%20Getter", handler.LastRequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task RemoveEditorConfigFileTypeName_ReturnsTheServerError()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.NotFound,
            """{"title":"EditorConfigFileTypeWithNameNotFound","status":404,"detail":"EditorConfig File Type With Name React Not Found"}""",
            "application/problem+json");

        (Result result, _) =
            await CaptureConsole(async () => await CreateClient(handler).RemoveEditorConfigFileTypeName("React"));

        Assert.Equal("EditorConfigFileTypeWithNameNotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetGitIgnoreFileNames_GetsTheListRouteWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, """["CSharp","React"]""");

        (Result<List<string>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetGitIgnoreFileNames());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/git/gitignorefiletypeslist", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(["CSharp", "React"], result.Value);
    }

    [Fact]
    public async Task GetEnvironments_GetsTheListWithTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"Dev","description":null,"version":1},{"name":"Prod","description":"Production","version":3}]""");

        (Result<List<StsEnvironmentDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetEnvironments());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/environments", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        Assert.Null(result.Value[0].Description);
        Assert.Equal("Prod", result.Value[1].Name);
        Assert.Equal("Production", result.Value[1].Description);
        Assert.Equal(3, result.Value[1].Version);
    }

    [Fact]
    public async Task GetEnvironment_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"Pre Prod/1","description":"Staging","version":2}""");

        (Result<StsEnvironmentDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetEnvironment("Pre Prod/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/environments/Pre%20Prod%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("Staging", result.Value.Description);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task GetEnvironment_ReturnsRecordWithNameNotFound()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.NotFound,
            """{"title":"RecordWithNameNotFound","status":404,"detail":"Environment With Name Prod Not Found"}""",
            "application/problem+json");

        Result<StsEnvironmentDataModel> result = await CreateClient(handler).GetEnvironment("Prod");

        Assert.Equal("RecordWithNameNotFound", result.Error.Code);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task UpdateEnvironment_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var environment = new StsEnvironmentDataModel { Name = "Pre Prod/1", Description = "Staging", Version = 3 };

        (Result<int> result, string output) =
            await CaptureConsole(() => CreateClient(handler).UpdateEnvironment("Pre Prod/1", environment));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/environments/update/Pre%20Prod%2F1", handler.LastRequestUri!.AbsolutePath);
        StsEnvironmentDataModel sent = JsonConvert.DeserializeObject<StsEnvironmentDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("Pre Prod/1", sent.Name);
        Assert.Equal("Staging", sent.Description);
        Assert.Equal(3, sent.Version);
    }

    [Fact]
    public async Task UpdateEnvironment_ReturnsConcurrencyConflict()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.Conflict,
            """{"title":"ConcurrencyConflict","status":409,"detail":"Environment Prod Version Conflict: Expected 2, Actual 3"}""",
            "application/problem+json");

        (Result<int> result, _) = await CaptureConsole(() =>
            CreateClient(handler).UpdateEnvironment("Prod", new StsEnvironmentDataModel { Name = "Prod", Version = 2 }));

        Assert.Equal("ConcurrencyConflict", result.Error.Code);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
        Assert.Equal("Environment Prod Version Conflict: Expected 2, Actual 3", result.Error.Description);
    }

    [Fact]
    public async Task DeleteEnvironment_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteEnvironment("Pre Prod/1", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/environments/delete/Pre%20Prod%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteEnvironment_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) = await CaptureConsole(async () => await CreateClient(handler).DeleteEnvironment("Dev", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/environments/delete/Dev", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteEnvironment_ReturnsRecordIsInUse()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.Conflict,
            """{"title":"RecordIsInUse","status":409,"detail":"Environment Prod Is Used By: ServerInfo AppA"}""",
            "application/problem+json");

        (Result result, _) = await CaptureConsole(async () => await CreateClient(handler).DeleteEnvironment("Prod", 1));

        Assert.Equal("RecordIsInUse", result.Error.Code);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task GetRuntimes_GetsTheListWithTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"linux-x64","description":null,"version":1},{"name":"win-x64","description":"Windows x64","version":3}]""");

        (Result<List<StsRuntimeDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetRuntimes());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/runtimes", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        Assert.Null(result.Value[0].Description);
        Assert.Equal("win-x64", result.Value[1].Name);
        Assert.Equal("Windows x64", result.Value[1].Description);
        Assert.Equal(3, result.Value[1].Version);
    }

    [Fact]
    public async Task GetRuntime_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"win x64/1","description":"Windows x64","version":2}""");

        (Result<StsRuntimeDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetRuntime("win x64/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/runtimes/win%20x64%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("Windows x64", result.Value.Description);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task UpdateRuntime_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var runtime = new StsRuntimeDataModel { Name = "win x64/1", Description = "Windows x64", Version = 3 };

        (Result<int> result, string output) =
            await CaptureConsole(() => CreateClient(handler).UpdateRuntime("win x64/1", runtime));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/runtimes/update/win%20x64%2F1", handler.LastRequestUri!.AbsolutePath);
        StsRuntimeDataModel sent = JsonConvert.DeserializeObject<StsRuntimeDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("win x64/1", sent.Name);
        Assert.Equal("Windows x64", sent.Description);
        Assert.Equal(3, sent.Version);
    }

    [Fact]
    public async Task DeleteRuntime_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteRuntime("win x64/1", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/runtimes/delete/win%20x64%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteRuntime_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) = await CaptureConsole(async () => await CreateClient(handler).DeleteRuntime("win-x64", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/runtimes/delete/win-x64", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteRuntime_ReturnsRecordIsInUse()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.Conflict,
            """{"title":"RecordIsInUse","status":409,"detail":"Runtime linux-x64 Is Used By: Server dl360"}""",
            "application/problem+json");

        (Result result, _) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteRuntime("linux-x64", 1));

        Assert.Equal("RecordIsInUse", result.Error.Code);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
        Assert.Equal("Runtime linux-x64 Is Used By: Server dl360", result.Error.Description);
    }

    [Fact]
    public async Task GetNpmPackages_GetsTheListWithTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"@reduxjs/toolkit","description":null,"version":1},{"name":"yup","description":"Schema validation","version":3}]""");

        (Result<List<StsNpmPackageDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetNpmPackages());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/npmpackages", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("@reduxjs/toolkit", result.Value[0].Name);
        Assert.Null(result.Value[0].Description);
        Assert.Equal("Schema validation", result.Value[1].Description);
        Assert.Equal(3, result.Value[1].Version);
    }

    //A scoped package name holds a slash
    [Fact]
    public async Task GetNpmPackage_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"@reduxjs/toolkit","description":"Redux toolset","version":2}""");

        (Result<StsNpmPackageDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetNpmPackage("@reduxjs/toolkit"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/npmpackages/%40reduxjs%2Ftoolkit", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("Redux toolset", result.Value.Description);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task UpdateNpmPackage_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var npmPackage = new StsNpmPackageDataModel { Name = "@reduxjs/toolkit", Description = "Redux", Version = 3 };

        (Result<int> result, string output) =
            await CaptureConsole(() => CreateClient(handler).UpdateNpmPackage("@reduxjs/toolkit", npmPackage));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/npmpackages/update/%40reduxjs%2Ftoolkit", handler.LastRequestUri!.AbsolutePath);
        StsNpmPackageDataModel sent = JsonConvert.DeserializeObject<StsNpmPackageDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("@reduxjs/toolkit", sent.Name);
        Assert.Equal("Redux", sent.Description);
        Assert.Equal(3, sent.Version);
    }

    [Fact]
    public async Task DeleteNpmPackage_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteNpmPackage("@reduxjs/toolkit", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/npmpackages/delete/%40reduxjs%2Ftoolkit", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteNpmPackage_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) = await CaptureConsole(async () => await CreateClient(handler).DeleteNpmPackage("yup", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/npmpackages/delete/yup", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task GetReactAppTemplates_GetsTheListWithTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"ReduxApp","template":"redux-typescript","version":1},{"name":"TypeScriptApp","template":"typescript","version":3}]""");

        (Result<List<StsReactAppTemplateDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetReactAppTemplates());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/reactapptemplates", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("redux-typescript", result.Value[0].Template);
        Assert.Equal("TypeScriptApp", result.Value[1].Name);
        Assert.Equal(3, result.Value[1].Version);
    }

    [Fact]
    public async Task GetReactAppTemplate_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"Redux App/1","template":"redux-typescript","version":2}""");

        (Result<StsReactAppTemplateDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetReactAppTemplate("Redux App/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/reactapptemplates/Redux%20App%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("redux-typescript", result.Value.Template);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task UpdateReactAppTemplate_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var reactAppTemplate =
            new StsReactAppTemplateDataModel { Name = "Redux App/1", Template = "redux-typescript", Version = 3 };

        (Result<int> result, string output) = await CaptureConsole(() =>
            CreateClient(handler).UpdateReactAppTemplate("Redux App/1", reactAppTemplate));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/reactapptemplates/update/Redux%20App%2F1", handler.LastRequestUri!.AbsolutePath);
        StsReactAppTemplateDataModel sent =
            JsonConvert.DeserializeObject<StsReactAppTemplateDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("Redux App/1", sent.Name);
        Assert.Equal("redux-typescript", sent.Template);
        Assert.Equal(3, sent.Version);
    }

    [Fact]
    public async Task DeleteReactAppTemplate_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteReactAppTemplate("Redux App/1", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/reactapptemplates/delete/Redux%20App%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteReactAppTemplate_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteReactAppTemplate("ReduxApp", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/reactapptemplates/delete/ReduxApp", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task GetDotnetTools_GetsTheListWithTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"DotnetEf","packageId":"dotnet-ef","maxVersion":null,"description":null,"version":1},{"name":"Stryker","packageId":"dotnet-stryker","maxVersion":"5.0.0","description":"mutation testing","version":3}]""");

        (Result<List<StsDotnetToolDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetDotnetTools());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/dotnettools", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("dotnet-ef", result.Value[0].PackageId);
        Assert.Null(result.Value[0].MaxVersion);
        Assert.Null(result.Value[0].Description);
        Assert.Equal("Stryker", result.Value[1].Name);
        Assert.Equal("5.0.0", result.Value[1].MaxVersion);
        Assert.Equal("mutation testing", result.Value[1].Description);
        Assert.Equal(3, result.Value[1].Version);
    }

    [Fact]
    public async Task GetDotnetTool_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"Dotnet Ef/1","packageId":"dotnet-ef","maxVersion":"9.0.8","description":"Entity Framework","version":2}""");

        (Result<StsDotnetToolDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetDotnetTool("Dotnet Ef/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/dotnettools/Dotnet%20Ef%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("dotnet-ef", result.Value.PackageId);
        Assert.Equal("9.0.8", result.Value.MaxVersion);
        Assert.Equal("Entity Framework", result.Value.Description);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task UpdateDotnetTool_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var dotnetTool = new StsDotnetToolDataModel
        {
            Name = "Dotnet Ef/1",
            PackageId = "dotnet-ef",
            MaxVersion = "9.0.8",
            Description = "Entity Framework",
            Version = 3
        };

        (Result<int> result, string output) =
            await CaptureConsole(() => CreateClient(handler).UpdateDotnetTool("Dotnet Ef/1", dotnetTool));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/dotnettools/update/Dotnet%20Ef%2F1", handler.LastRequestUri!.AbsolutePath);
        StsDotnetToolDataModel sent = JsonConvert.DeserializeObject<StsDotnetToolDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("Dotnet Ef/1", sent.Name);
        Assert.Equal("dotnet-ef", sent.PackageId);
        Assert.Equal("9.0.8", sent.MaxVersion);
        Assert.Equal("Entity Framework", sent.Description);
        Assert.Equal(3, sent.Version);
    }

    [Fact]
    public async Task DeleteDotnetTool_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteDotnetTool("Dotnet Ef/1", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/dotnettools/delete/Dotnet%20Ef%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteDotnetTool_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) = await CaptureConsole(async () => await CreateClient(handler).DeleteDotnetTool("Stryker", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/dotnettools/delete/Stryker", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task GetSmartSchemas_GetsTheListWithTheDetailsAndTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"Hourly","lastPreserveCount":1,"details":[{"periodType":"Hour","preserveCount":48}],"version":1},{"name":"Reduce","lastPreserveCount":2,"details":[],"version":3}]""");

        (Result<List<StsSmartSchemaDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetSmartSchemas());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/smartschemas", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        StsSmartSchemaDetailDataModel detail = Assert.Single(result.Value[0].Details);
        Assert.Equal("Hour", detail.PeriodType);
        Assert.Equal(48, detail.PreserveCount);
        Assert.Equal("Reduce", result.Value[1].Name);
        Assert.Equal(2, result.Value[1].LastPreserveCount);
        Assert.Empty(result.Value[1].Details);
        Assert.Equal(3, result.Value[1].Version);
    }

    [Fact]
    public async Task GetSmartSchema_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"Daily Standard/1","lastPreserveCount":1,"details":[{"periodType":"Day","preserveCount":3}],"version":2}""");

        (Result<StsSmartSchemaDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetSmartSchema("Daily Standard/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/smartschemas/Daily%20Standard%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("Day", Assert.Single(result.Value.Details).PeriodType);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task UpdateSmartSchema_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var smartSchema = new StsSmartSchemaDataModel
        {
            Name = "Daily Standard/1",
            LastPreserveCount = 1,
            Details = [new StsSmartSchemaDetailDataModel { PeriodType = "Day", PreserveCount = 3 }],
            Version = 3
        };

        (Result<int> result, string output) =
            await CaptureConsole(() => CreateClient(handler).UpdateSmartSchema("Daily Standard/1", smartSchema));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/smartschemas/update/Daily%20Standard%2F1", handler.LastRequestUri!.AbsolutePath);
        StsSmartSchemaDataModel sent = JsonConvert.DeserializeObject<StsSmartSchemaDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("Daily Standard/1", sent.Name);
        Assert.Equal(1, sent.LastPreserveCount);
        Assert.Equal(3, Assert.Single(sent.Details).PreserveCount);
        Assert.Equal(3, sent.Version);
    }

    //A smart schema holds no secret, so the console shows its body like any other request
    [Fact]
    public async Task UpdateSmartSchema_WritesTheBodyOfAFailedRequestToTheConsole()
    {
        using StubHttpMessageHandler handler = ConflictHandler("SmartSchema");

        (Result<int> result, string output) = await CaptureConsole(() => CreateConsoleClient(handler)
            .UpdateSmartSchema("Reduce", new StsSmartSchemaDataModel { Name = "Reduce", Version = 1 }));

        Assert.Equal("ConcurrencyConflict", result.Error.Code);
        Assert.Contains("request body was", output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeleteSmartSchema_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteSmartSchema("Daily Standard/1", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/smartschemas/delete/Daily%20Standard%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteSmartSchema_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) = await CaptureConsole(async () => await CreateClient(handler).DeleteSmartSchema("Reduce", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/smartschemas/delete/Reduce", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task GetFileStorages_GetsTheListWithTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"Exchange","fileStoragePath":"ftp://ftp.example.com/x/","userName":"made-up-user","password":"made-up-password","fileNameMaxLength":255,"fileSizeSplitPositionInRow":4,"ftpSiteLsFileOffset":1,"version":3},{"name":"LocalBak","fileStoragePath":"D:\\Bak","userName":null,"password":null,"fileNameMaxLength":0,"fileSizeSplitPositionInRow":0,"ftpSiteLsFileOffset":0,"version":1}]""");

        (Result<List<StsFileStorageDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetFileStorages());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/filestorages", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("ftp://ftp.example.com/x/", result.Value[0].FileStoragePath);
        Assert.Equal("made-up-user", result.Value[0].UserName);
        Assert.Equal("made-up-password", result.Value[0].Password);
        Assert.Equal(255, result.Value[0].FileNameMaxLength);
        Assert.Equal(4, result.Value[0].FileSizeSplitPositionInRow);
        Assert.Equal(1, result.Value[0].FtpSiteLsFileOffset);
        Assert.Equal(3, result.Value[0].Version);
        Assert.Equal(@"D:\Bak", result.Value[1].FileStoragePath);
        Assert.Null(result.Value[1].Password);
    }

    [Fact]
    public async Task GetFileStorage_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"Local Bak/1","fileStoragePath":"D:\\Bak","version":2}""");

        (Result<StsFileStorageDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetFileStorage("Local Bak/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/filestorages/Local%20Bak%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(@"D:\Bak", result.Value.FileStoragePath);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task UpdateFileStorage_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var fileStorage = new StsFileStorageDataModel
        {
            Name = "Local Bak/1",
            FileStoragePath = "ftp://ftp.example.com/x/",
            UserName = "made-up-user",
            Password = "made-up-password",
            FileNameMaxLength = 255,
            FileSizeSplitPositionInRow = 4,
            FtpSiteLsFileOffset = 1,
            Version = 3
        };

        (Result<int> result, string output) =
            await CaptureConsole(() => CreateClient(handler).UpdateFileStorage("Local Bak/1", fileStorage));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/filestorages/update/Local%20Bak%2F1", handler.LastRequestUri!.AbsolutePath);
        StsFileStorageDataModel sent = JsonConvert.DeserializeObject<StsFileStorageDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("Local Bak/1", sent.Name);
        Assert.Equal("ftp://ftp.example.com/x/", sent.FileStoragePath);
        Assert.Equal("made-up-user", sent.UserName);
        Assert.Equal("made-up-password", sent.Password);
        Assert.Equal(255, sent.FileNameMaxLength);
        Assert.Equal(4, sent.FileSizeSplitPositionInRow);
        Assert.Equal(1, sent.FtpSiteLsFileOffset);
        Assert.Equal(3, sent.Version);
    }

    //The body holds the password, so the console of a failed request leaves it out
    [Fact]
    public async Task UpdateFileStorage_DoesNotWriteTheSecretsOfAFailedRequestToTheConsole()
    {
        using StubHttpMessageHandler handler = ConflictHandler("FileStorage");

        (Result<int> result, string output) = await CaptureConsole(() => CreateConsoleClient(handler)
            .UpdateFileStorage("A",
                new StsFileStorageDataModel
                {
                    Name = "A", UserName = "made-up-user", Password = "made-up-password", Version = 1
                }));

        Assert.Equal("ConcurrencyConflict", result.Error.Code);
        Assert.Contains("409 Conflict", output, StringComparison.Ordinal);
        Assert.DoesNotContain("request body was", output, StringComparison.Ordinal);
        Assert.DoesNotContain("made-up", output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeleteFileStorage_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteFileStorage("Local Bak/1", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/filestorages/delete/Local%20Bak%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteFileStorage_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteFileStorage("Exchange", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/filestorages/delete/Exchange", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task GetApiClients_GetsTheListWithTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"Pc1.WebAgent","server":"http://localhost:5031/api/v1/","apiKey":"made-up-key","version":3},{"name":"Pc2.WebAgent","server":null,"apiKey":null,"version":1}]""");

        (Result<List<StsApiClientDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetApiClients());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/apiclients", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("http://localhost:5031/api/v1/", result.Value[0].Server);
        Assert.Equal("made-up-key", result.Value[0].ApiKey);
        Assert.Equal(3, result.Value[0].Version);
        Assert.Null(result.Value[1].Server);
        Assert.Null(result.Value[1].ApiKey);
    }

    //The names of the API clients hold dots
    [Fact]
    public async Task GetApiClient_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"Pc1.Web Agent/1","server":"http://localhost:5031/api/v1/","apiKey":"made-up-key","version":2}""");

        (Result<StsApiClientDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetApiClient("Pc1.Web Agent/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/apiclients/Pc1.Web%20Agent%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("made-up-key", result.Value.ApiKey);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task UpdateApiClient_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var apiClient = new StsApiClientDataModel
        {
            Name = "Pc1.Web Agent/1", Server = "http://localhost:5031/api/v1/", ApiKey = "made-up-key", Version = 3
        };

        (Result<int> result, string output) =
            await CaptureConsole(() => CreateClient(handler).UpdateApiClient("Pc1.Web Agent/1", apiClient));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/apiclients/update/Pc1.Web%20Agent%2F1", handler.LastRequestUri!.AbsolutePath);
        StsApiClientDataModel sent = JsonConvert.DeserializeObject<StsApiClientDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("Pc1.Web Agent/1", sent.Name);
        Assert.Equal("http://localhost:5031/api/v1/", sent.Server);
        Assert.Equal("made-up-key", sent.ApiKey);
        Assert.Equal(3, sent.Version);
    }

    //The body holds the API key, so the console of a failed request leaves it out
    [Fact]
    public async Task UpdateApiClient_DoesNotWriteTheSecretsOfAFailedRequestToTheConsole()
    {
        using StubHttpMessageHandler handler = ConflictHandler("ApiClient");

        (Result<int> result, string output) = await CaptureConsole(() => CreateConsoleClient(handler)
            .UpdateApiClient("A", new StsApiClientDataModel { Name = "A", ApiKey = "made-up-key", Version = 1 }));

        Assert.Equal("ConcurrencyConflict", result.Error.Code);
        Assert.Contains("409 Conflict", output, StringComparison.Ordinal);
        Assert.DoesNotContain("request body was", output, StringComparison.Ordinal);
        Assert.DoesNotContain("made-up-key", output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeleteApiClient_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteApiClient("Pc1.Web Agent/1", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/apiclients/delete/Pc1.Web%20Agent%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteApiClient_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteApiClient("Pc1.WebAgent", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/apiclients/delete/Pc1.WebAgent", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteApiClient_ReturnsRecordIsInUse()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.Conflict,
            """{"title":"RecordIsInUse","status":409,"detail":"ApiClient Pc1.WebAgent Is Used By: DatabaseServerConnection Pc1.Sql"}""",
            "application/problem+json");

        (Result result, _) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteApiClient("Pc1.WebAgent", 1));

        Assert.Equal("RecordIsInUse", result.Error.Code);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
        Assert.Equal("ApiClient Pc1.WebAgent Is Used By: DatabaseServerConnection Pc1.Sql", result.Error.Description);
    }

    [Fact]
    public async Task GetDatabaseServerConnections_GetsTheListWithTheFoldersSetsAndTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"Pc1.Sql","databaseServerProvider":"WebAgent","dbWebAgentName":"Pc1.WebAgent","remoteDbConnectionName":"Main","serverAddress":"pc1","windowsNtIntegratedSecurity":true,"serverUser":"made-up-user","serverPass":"made-up-password","trustServerCertificate":true,"connectionTimeOut":30,"encrypt":true,"databaseFoldersSets":[{"name":"Default","backup":"D:\\Bak","data":"D:\\Data","dataLog":"D:\\Log"}],"version":3},{"name":"Pc2.Sql","databaseServerProvider":"SqlServer","databaseFoldersSets":[],"version":1}]""");

        (Result<List<StsDatabaseServerConnectionDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetDatabaseServerConnections());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/databaseserverconnections", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        StsDatabaseServerConnectionDataModel first = result.Value[0];
        Assert.Equal("WebAgent", first.DatabaseServerProvider);
        Assert.Equal("Pc1.WebAgent", first.DbWebAgentName);
        Assert.Equal("Main", first.RemoteDbConnectionName);
        Assert.Equal("pc1", first.ServerAddress);
        Assert.True(first.WindowsNtIntegratedSecurity);
        Assert.Equal("made-up-user", first.ServerUser);
        Assert.Equal("made-up-password", first.ServerPass);
        Assert.True(first.TrustServerCertificate);
        Assert.Equal(30, first.ConnectionTimeOut);
        Assert.True(first.Encrypt);
        StsDatabaseFoldersSetDataModel foldersSet = Assert.Single(first.DatabaseFoldersSets);
        Assert.Equal("Default", foldersSet.Name);
        Assert.Equal(@"D:\Bak", foldersSet.Backup);
        Assert.Equal(@"D:\Data", foldersSet.Data);
        Assert.Equal(@"D:\Log", foldersSet.DataLog);
        Assert.Equal(3, first.Version);
        Assert.Null(result.Value[1].DbWebAgentName);
        Assert.Empty(result.Value[1].DatabaseFoldersSets);
    }

    [Fact]
    public async Task GetDatabaseServerConnection_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"Pc1.Sql Main/1","databaseServerProvider":"SqlServer","databaseFoldersSets":[],"version":2}""");

        (Result<StsDatabaseServerConnectionDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetDatabaseServerConnection("Pc1.Sql Main/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/databaseserverconnections/Pc1.Sql%20Main%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("SqlServer", result.Value.DatabaseServerProvider);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task UpdateDatabaseServerConnection_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var connection = new StsDatabaseServerConnectionDataModel
        {
            Name = "Pc1.Sql Main/1",
            DatabaseServerProvider = "WebAgent",
            DbWebAgentName = "Pc1.WebAgent",
            ServerUser = "made-up-user",
            ServerPass = "made-up-password",
            ConnectionTimeOut = 30,
            DatabaseFoldersSets = [new StsDatabaseFoldersSetDataModel { Name = "Default", Backup = @"D:\Bak" }],
            Version = 3
        };

        (Result<int> result, string output) = await CaptureConsole(() =>
            CreateClient(handler).UpdateDatabaseServerConnection("Pc1.Sql Main/1", connection));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/databaseserverconnections/update/Pc1.Sql%20Main%2F1",
            handler.LastRequestUri!.AbsolutePath);
        StsDatabaseServerConnectionDataModel sent =
            JsonConvert.DeserializeObject<StsDatabaseServerConnectionDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("Pc1.Sql Main/1", sent.Name);
        Assert.Equal("WebAgent", sent.DatabaseServerProvider);
        Assert.Equal("Pc1.WebAgent", sent.DbWebAgentName);
        Assert.Equal("made-up-user", sent.ServerUser);
        Assert.Equal("made-up-password", sent.ServerPass);
        Assert.Equal(30, sent.ConnectionTimeOut);
        Assert.Equal(@"D:\Bak", Assert.Single(sent.DatabaseFoldersSets).Backup);
        Assert.Equal(3, sent.Version);
    }

    //The body holds the user and the password, so the console of a failed request leaves it out
    [Fact]
    public async Task UpdateDatabaseServerConnection_DoesNotWriteTheSecretsOfAFailedRequestToTheConsole()
    {
        using StubHttpMessageHandler handler = ConflictHandler("DatabaseServerConnection");

        (Result<int> result, string output) = await CaptureConsole(() => CreateConsoleClient(handler)
            .UpdateDatabaseServerConnection("A",
                new StsDatabaseServerConnectionDataModel
                {
                    Name = "A",
                    DatabaseServerProvider = "SqlServer",
                    ServerUser = "made-up-user",
                    ServerPass = "made-up-password",
                    Version = 1
                }));

        Assert.Equal("ConcurrencyConflict", result.Error.Code);
        Assert.Contains("409 Conflict", output, StringComparison.Ordinal);
        Assert.DoesNotContain("request body was", output, StringComparison.Ordinal);
        Assert.DoesNotContain("made-up", output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeleteDatabaseServerConnection_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) = await CaptureConsole(async () =>
            await CreateClient(handler).DeleteDatabaseServerConnection("Pc1.Sql Main/1", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/databaseserverconnections/delete/Pc1.Sql%20Main%2F1",
            handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteDatabaseServerConnection_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) = await CaptureConsole(async () =>
            await CreateClient(handler).DeleteDatabaseServerConnection("Pc1.Sql", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/databaseserverconnections/delete/Pc1.Sql", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task GetServers_GetsTheListWithTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"dl360","webAgentName":"Dl360.WebAgent","webAgentInstallerName":"Dl360.Installer","filesUserName":"deployer","filesUsersGroupName":"deployers","runtime":"linux-x64","serverSideDownloadFolder":"/home/deployer/Download","serverSideDeployFolder":"/opt/apps","version":3},{"name":"PAZISI","version":1}]""");

        (Result<List<StsServerDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetServers());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/servers", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        StsServerDataModel first = result.Value[0];
        Assert.Equal("dl360", first.Name);
        Assert.Equal("Dl360.WebAgent", first.WebAgentName);
        Assert.Equal("Dl360.Installer", first.WebAgentInstallerName);
        Assert.Equal("deployer", first.FilesUserName);
        Assert.Equal("deployers", first.FilesUsersGroupName);
        Assert.Equal("linux-x64", first.Runtime);
        Assert.Equal("/home/deployer/Download", first.ServerSideDownloadFolder);
        Assert.Equal("/opt/apps", first.ServerSideDeployFolder);
        Assert.Equal(3, first.Version);
        Assert.Null(result.Value[1].WebAgentName);
        Assert.Null(result.Value[1].Runtime);
    }

    [Fact]
    public async Task GetServer_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"dl 360/1","runtime":"linux-x64","version":2}""");

        (Result<StsServerDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetServer("dl 360/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/servers/dl%20360%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("linux-x64", result.Value.Runtime);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task UpdateServer_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var server = new StsServerDataModel
        {
            Name = "dl 360/1",
            WebAgentName = "Dl360.WebAgent",
            WebAgentInstallerName = "Dl360.Installer",
            FilesUserName = "deployer",
            FilesUsersGroupName = "deployers",
            Runtime = "linux-x64",
            ServerSideDownloadFolder = "/home/deployer/Download",
            ServerSideDeployFolder = "/opt/apps",
            Version = 3
        };

        (Result<int> result, string output) =
            await CaptureConsole(() => CreateClient(handler).UpdateServer("dl 360/1", server));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/servers/update/dl%20360%2F1", handler.LastRequestUri!.AbsolutePath);
        StsServerDataModel sent = JsonConvert.DeserializeObject<StsServerDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("dl 360/1", sent.Name);
        Assert.Equal("Dl360.WebAgent", sent.WebAgentName);
        Assert.Equal("Dl360.Installer", sent.WebAgentInstallerName);
        Assert.Equal("deployer", sent.FilesUserName);
        Assert.Equal("deployers", sent.FilesUsersGroupName);
        Assert.Equal("linux-x64", sent.Runtime);
        Assert.Equal("/home/deployer/Download", sent.ServerSideDownloadFolder);
        Assert.Equal("/opt/apps", sent.ServerSideDeployFolder);
        Assert.Equal(3, sent.Version);
    }

    //A server holds no secret, so the console shows its body like any other request
    [Fact]
    public async Task UpdateServer_WritesTheBodyOfAFailedRequestToTheConsole()
    {
        using StubHttpMessageHandler handler = ConflictHandler("Server");

        (Result<int> result, string output) = await CaptureConsole(() => CreateConsoleClient(handler)
            .UpdateServer("dl360", new StsServerDataModel { Name = "dl360", FilesUserName = "deployer", Version = 1 }));

        Assert.Equal("ConcurrencyConflict", result.Error.Code);
        Assert.Contains("request body was", output, StringComparison.Ordinal);
        Assert.Contains("deployer", output, StringComparison.Ordinal);
    }

    //Every missing name comes in one error, grouped by the type of the referenced record
    [Fact]
    public async Task UpdateServer_ReturnsReferencedRecordsNotFound()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.NotFound,
            """{"title":"ReferencedRecordsNotFound","status":404,"detail":"Referenced ApiClient Records Not Found: Pc9.WebAgent; Referenced Runtime Records Not Found: osx-arm64"}""",
            "application/problem+json");

        (Result<int> result, _) = await CaptureConsole(() =>
            CreateClient(handler).UpdateServer("dl360", new StsServerDataModel { Name = "dl360" }));

        Assert.Equal("ReferencedRecordsNotFound", result.Error.Code);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Equal(
            "Referenced ApiClient Records Not Found: Pc9.WebAgent; Referenced Runtime Records Not Found: osx-arm64",
            result.Error.Description);
    }

    [Fact]
    public async Task DeleteServer_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteServer("dl 360/1", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/servers/delete/dl%20360%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteServer_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) = await CaptureConsole(async () => await CreateClient(handler).DeleteServer("dl360", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/servers/delete/dl360", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task GetGlobalSettings_GetsTheSingletonWithTheExchangeParametersWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"serviceDescriptionSignature":"ltgmz","uploadTempExtension":".up!","programArchiveDateMask":"yyyyMMddHHmmss","programArchiveExtension":".zip","parametersFileDateMask":"yyyyMMdd","parametersFileExtension":".json","mediatRLicenseKey":"made-up-license-key","fileStorageNameForExchange":"Exchange","smartSchemaNameForExchange":"Reduce","smartSchemaNameForLocal":"Keep","localPackageManagerWebApiClientName":"packages.example.com","databasesBackupFilesExchange":{"downloadTempExtension":".down!","uploadTempExtension":".up!","exchangeFileStorageName":"Backups","exchangeSmartSchemaName":"Reduce","localSmartSchemaName":"Keep"},"version":3}""");

        (Result<StsGlobalSettingsDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetGlobalSettings());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/settings/global", handler.LastRequestUri!.AbsolutePath);
        StsGlobalSettingsDataModel globalSettings = result.Value;
        Assert.Equal("ltgmz", globalSettings.ServiceDescriptionSignature);
        Assert.Equal(".up!", globalSettings.UploadTempExtension);
        Assert.Equal("yyyyMMddHHmmss", globalSettings.ProgramArchiveDateMask);
        Assert.Equal(".zip", globalSettings.ProgramArchiveExtension);
        Assert.Equal("yyyyMMdd", globalSettings.ParametersFileDateMask);
        Assert.Equal(".json", globalSettings.ParametersFileExtension);
        Assert.Equal("made-up-license-key", globalSettings.MediatRLicenseKey);
        Assert.Equal("Exchange", globalSettings.FileStorageNameForExchange);
        Assert.Equal("Reduce", globalSettings.SmartSchemaNameForExchange);
        Assert.Equal("Keep", globalSettings.SmartSchemaNameForLocal);
        Assert.Equal("packages.example.com", globalSettings.LocalPackageManagerWebApiClientName);
        Assert.Equal(".down!", globalSettings.DatabasesBackupFilesExchange.DownloadTempExtension);
        Assert.Equal(".up!", globalSettings.DatabasesBackupFilesExchange.UploadTempExtension);
        Assert.Equal("Backups", globalSettings.DatabasesBackupFilesExchange.ExchangeFileStorageName);
        Assert.Equal("Reduce", globalSettings.DatabasesBackupFilesExchange.ExchangeSmartSchemaName);
        Assert.Equal("Keep", globalSettings.DatabasesBackupFilesExchange.LocalSmartSchemaName);
        Assert.Equal(3, globalSettings.Version);
    }

    //Before the singleton is created the server returns an empty contract with version 0, not an error
    [Fact]
    public async Task GetGlobalSettings_ReadsTheEmptyContractOfTheSingletonThatIsNotCreatedYet()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"databasesBackupFilesExchange":{},"version":0}""");

        (Result<StsGlobalSettingsDataModel> result, string output) =
            await CaptureConsole(() => CreateConsoleClient(handler).GetGlobalSettings());

        Assert.Equal(string.Empty, output);
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.Version);
        Assert.Null(result.Value.ServiceDescriptionSignature);
        Assert.Null(result.Value.DatabasesBackupFilesExchange.ExchangeFileStorageName);
    }

    [Fact]
    public async Task UpdateGlobalSettings_PostsTheRecordAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var globalSettings = new StsGlobalSettingsDataModel
        {
            ServiceDescriptionSignature = "ltgmz",
            MediatRLicenseKey = "made-up-license-key",
            FileStorageNameForExchange = "Exchange",
            DatabasesBackupFilesExchange = new StsDatabasesBackupFilesExchangeDataModel
            {
                DownloadTempExtension = ".down!", LocalSmartSchemaName = "Keep"
            },
            Version = 3
        };

        (Result<int> result, string output) =
            await CaptureConsole(() => CreateClient(handler).UpdateGlobalSettings(globalSettings));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/settings/global/update", handler.LastRequestUri!.AbsolutePath);
        StsGlobalSettingsDataModel sent =
            JsonConvert.DeserializeObject<StsGlobalSettingsDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("ltgmz", sent.ServiceDescriptionSignature);
        Assert.Equal("made-up-license-key", sent.MediatRLicenseKey);
        Assert.Equal("Exchange", sent.FileStorageNameForExchange);
        Assert.Equal(".down!", sent.DatabasesBackupFilesExchange.DownloadTempExtension);
        Assert.Equal("Keep", sent.DatabasesBackupFilesExchange.LocalSmartSchemaName);
        Assert.Equal(3, sent.Version);
    }

    //The body holds the MediatR license key, so the console of a failed request leaves it out
    [Fact]
    public async Task UpdateGlobalSettings_DoesNotWriteTheSecretsOfAFailedRequestToTheConsole()
    {
        using StubHttpMessageHandler handler = ConflictHandler("Settings");

        (Result<int> result, string output) = await CaptureConsole(() => CreateConsoleClient(handler)
            .UpdateGlobalSettings(new StsGlobalSettingsDataModel
            {
                MediatRLicenseKey = "made-up-license-key", Version = 1
            }));

        Assert.Equal("ConcurrencyConflict", result.Error.Code);
        Assert.Contains("409 Conflict", output, StringComparison.Ordinal);
        Assert.DoesNotContain("request body was", output, StringComparison.Ordinal);
        Assert.DoesNotContain("made-up", output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetProjectCreatorSettings_GetsTheSingletonWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"indentSize":4,"fakeHostProjectName":"FakeHost","projectsFolderPathReal":"D:\\1WorkDotnet","secretsFolderPathReal":"D:\\1WorkSecurity","productionServerName":"dl360","productionEnvironmentName":"Prod","developerDbConnectionName":"Pazisi","databaseExchangeFileStorageName":"Backups","useSmartSchema":"Reduce","version":3}""");

        (Result<StsProjectCreatorSettingsDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetProjectCreatorSettings());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/settings/projectcreator", handler.LastRequestUri!.AbsolutePath);
        StsProjectCreatorSettingsDataModel projectCreatorSettings = result.Value;
        Assert.Equal(4, projectCreatorSettings.IndentSize);
        Assert.Equal("FakeHost", projectCreatorSettings.FakeHostProjectName);
        Assert.Equal(@"D:\1WorkDotnet", projectCreatorSettings.ProjectsFolderPathReal);
        Assert.Equal(@"D:\1WorkSecurity", projectCreatorSettings.SecretsFolderPathReal);
        Assert.Equal("dl360", projectCreatorSettings.ProductionServerName);
        Assert.Equal("Prod", projectCreatorSettings.ProductionEnvironmentName);
        Assert.Equal("Pazisi", projectCreatorSettings.DeveloperDbConnectionName);
        Assert.Equal("Backups", projectCreatorSettings.DatabaseExchangeFileStorageName);
        Assert.Equal("Reduce", projectCreatorSettings.UseSmartSchema);
        Assert.Equal(3, projectCreatorSettings.Version);
    }

    [Fact]
    public async Task UpdateProjectCreatorSettings_PostsTheRecordAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "1");
        var projectCreatorSettings = new StsProjectCreatorSettingsDataModel
        {
            IndentSize = 4,
            FakeHostProjectName = "FakeHost",
            ProjectsFolderPathReal = @"D:\1WorkDotnet",
            ProductionServerName = "dl360",
            UseSmartSchema = "Reduce",
            Version = 0
        };

        (Result<int> result, string output) = await CaptureConsole(() =>
            CreateClient(handler).UpdateProjectCreatorSettings(projectCreatorSettings));

        Assert.Equal(1, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/settings/projectcreator/update", handler.LastRequestUri!.AbsolutePath);
        StsProjectCreatorSettingsDataModel sent =
            JsonConvert.DeserializeObject<StsProjectCreatorSettingsDataModel>(handler.LastRequestBody!)!;
        Assert.Equal(4, sent.IndentSize);
        Assert.Equal("FakeHost", sent.FakeHostProjectName);
        Assert.Equal(@"D:\1WorkDotnet", sent.ProjectsFolderPathReal);
        Assert.Equal("dl360", sent.ProductionServerName);
        Assert.Equal("Reduce", sent.UseSmartSchema);
        Assert.Equal(0, sent.Version);
    }

    //The project creator settings hold no secret, so the console shows the body like any other request
    [Fact]
    public async Task UpdateProjectCreatorSettings_WritesTheBodyOfAFailedRequestToTheConsole()
    {
        using StubHttpMessageHandler handler = ConflictHandler("Settings");

        (Result<int> result, string output) = await CaptureConsole(() => CreateConsoleClient(handler)
            .UpdateProjectCreatorSettings(new StsProjectCreatorSettingsDataModel
            {
                FakeHostProjectName = "FakeHost", Version = 1
            }));

        Assert.Equal("ConcurrencyConflict", result.Error.Code);
        Assert.Contains("request body was", output, StringComparison.Ordinal);
        Assert.Contains("FakeHost", output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetProjectTemplates_GetsTheListWithTheVersionsWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """[{"name":"Console","supportProjectType":"Console","testProjectName":"ConsoleTest","testProjectShortName":"CT","useDatabase":true,"useMenu":true,"version":1},{"name":"Reactredux","supportProjectType":"Api","useReact":true,"useFluentValidation":true,"reactTemplateName":"redux-typescript","version":3}]""");

        (Result<List<StsProjectTemplateDataModel>> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetProjectTemplates());

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/projecttemplates", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(2, result.Value.Count);
        StsProjectTemplateDataModel first = result.Value[0];
        Assert.Equal("Console", first.Name);
        Assert.Equal("Console", first.SupportProjectType);
        Assert.Equal("ConsoleTest", first.TestProjectName);
        Assert.Equal("CT", first.TestProjectShortName);
        Assert.True(first.UseDatabase);
        Assert.True(first.UseMenu);
        Assert.False(first.UseReact);
        Assert.Null(first.ReactTemplateName);
        Assert.Equal(1, first.Version);
        StsProjectTemplateDataModel second = result.Value[1];
        Assert.True(second.UseReact);
        Assert.True(second.UseFluentValidation);
        Assert.Equal("redux-typescript", second.ReactTemplateName);
        Assert.Equal(3, second.Version);
    }

    //Template names hold spaces, such as "Console With Database"
    [Fact]
    public async Task GetProjectTemplate_GetsTheRecordOfTheEscapedKeyWithoutTheMessageHub()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK,
            """{"name":"Console With Database/1","supportProjectType":"Console","useDatabase":true,"version":2}""");

        (Result<StsProjectTemplateDataModel> result, string output) =
            await CaptureConsole(() => CreateClient(handler).GetProjectTemplate("Console With Database/1"));

        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Get, handler.LastRequestMethod);
        Assert.Equal("/api/v1/projecttemplates/Console%20With%20Database%2F1", handler.LastRequestUri!.AbsolutePath);
        Assert.True(result.Value.UseDatabase);
        Assert.Equal(2, result.Value.Version);
    }

    [Fact]
    public async Task UpdateProjectTemplate_PostsTheRecordToTheEscapedKeyAndReturnsTheNewVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "4");
        var projectTemplate = new StsProjectTemplateDataModel
        {
            Name = "Console With Database/1",
            SupportProjectType = "Console",
            TestProjectName = "ConsoleDbTest",
            UseDatabase = true,
            UseDbPartFolderForDatabaseProjects = true,
            UseSignalR = true,
            ReactTemplateName = "typescript",
            Version = 3
        };

        (Result<int> result, string output) = await CaptureConsole(() =>
            CreateClient(handler).UpdateProjectTemplate("Console With Database/1", projectTemplate));

        Assert.Equal(4, result.Value);
        Assert.Equal(string.Empty, output);
        Assert.Equal(HttpMethod.Post, handler.LastRequestMethod);
        Assert.Equal("/api/v1/projecttemplates/update/Console%20With%20Database%2F1",
            handler.LastRequestUri!.AbsolutePath);
        StsProjectTemplateDataModel sent =
            JsonConvert.DeserializeObject<StsProjectTemplateDataModel>(handler.LastRequestBody!)!;
        Assert.Equal("Console With Database/1", sent.Name);
        Assert.Equal("Console", sent.SupportProjectType);
        Assert.Equal("ConsoleDbTest", sent.TestProjectName);
        Assert.True(sent.UseDatabase);
        Assert.True(sent.UseDbPartFolderForDatabaseProjects);
        Assert.True(sent.UseSignalR);
        Assert.False(sent.UseMenu);
        Assert.Equal("typescript", sent.ReactTemplateName);
        Assert.Equal(3, sent.Version);
    }

    [Fact]
    public async Task DeleteProjectTemplate_DeletesTheEscapedKeyWithTheExpectedVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, string output) = await CaptureConsole(async () =>
            await CreateClient(handler).DeleteProjectTemplate("Console With Database/1", 12));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(string.Empty, output);
        Assert.Equal(HttpMethod.Delete, handler.LastRequestMethod);
        Assert.Equal("/api/v1/projecttemplates/delete/Console%20With%20Database%2F1",
            handler.LastRequestUri!.AbsolutePath);
        Assert.Equal("?version=12", handler.LastRequestUri.Query);
    }

    [Fact]
    public async Task DeleteProjectTemplate_WithoutVersion_SendsNoVersion()
    {
        using var handler = new StubHttpMessageHandler(HttpStatusCode.OK, null);

        (Result result, _) =
            await CaptureConsole(async () => await CreateClient(handler).DeleteProjectTemplate("Console", null));

        Assert.True(result.IsSuccess);
        Assert.Equal("/api/v1/projecttemplates/delete/Console", handler.LastRequestUri!.AbsolutePath);
        Assert.Equal(string.Empty, handler.LastRequestUri.Query);
    }
}
