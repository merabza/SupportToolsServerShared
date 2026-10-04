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
}
