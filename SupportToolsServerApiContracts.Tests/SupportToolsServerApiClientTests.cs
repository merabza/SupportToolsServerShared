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
}
