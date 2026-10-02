using SupportToolsServerApiContracts.Errors;
using SystemTools.SharedKernel;
using Xunit;

namespace SupportToolsServerApiContracts.Tests.Errors;

public sealed class SupportToolsServerApiClientErrorsTests
{
    //The SupportTools sync engine recognizes the registry errors by these exact codes (RegistrySyncServerErrorCodes)
    [Fact]
    public void RecordWithNameNotFound_IsANotFoundErrorNamingTheEntityAndTheName()
    {
        Error error = SupportToolsServerApiClientErrors.RecordWithNameNotFound("Environment", "Prod");

        Assert.Equal("RecordWithNameNotFound", error.Code);
        Assert.Equal("Environment With Name Prod Not Found", error.Description);
        Assert.Equal(ErrorType.NotFound, error.Type);
    }

    [Fact]
    public void RecordIsInUse_IsAConflictListingTheUsages()
    {
        Error error =
            SupportToolsServerApiClientErrors.RecordIsInUse("Environment", "Prod", ["ServerInfo AppA", "Project B"]);

        Assert.Equal("RecordIsInUse", error.Code);
        Assert.Equal("Environment Prod Is Used By: ServerInfo AppA, Project B", error.Description);
        Assert.Equal(ErrorType.Conflict, error.Type);
    }

    [Fact]
    public void ConcurrencyConflict_IsAConflictNamingTheExpectedAndTheActualVersion()
    {
        Error error = SupportToolsServerApiClientErrors.ConcurrencyConflict("Environment", "Prod", 2, 3);

        Assert.Equal("ConcurrencyConflict", error.Code);
        Assert.Equal("Environment Prod Version Conflict: Expected 2, Actual 3", error.Description);
        Assert.Equal(ErrorType.Conflict, error.Type);
    }

    [Fact]
    public void ReferencedRecordsNotFound_IsANotFoundErrorListingTheNames()
    {
        Error error = SupportToolsServerApiClientErrors.ReferencedRecordsNotFound("Runtime", ["linux-arm", "osx-x64"]);

        Assert.Equal("ReferencedRecordsNotFound", error.Code);
        Assert.Equal("Referenced Runtime Records Not Found: linux-arm, osx-x64", error.Description);
        Assert.Equal(ErrorType.NotFound, error.Type);
    }

    [Fact]
    public void GitWithKeyNotFound_IsANotFoundErrorNamingTheKey()
    {
        Error error = SupportToolsServerApiClientErrors.GitWithKeyNotFound("RepoA");

        Assert.Equal("GitWithKeyNotFound", error.Code);
        Assert.Equal("Git With Key RepoA Not Found", error.Description);
        Assert.Equal(ErrorType.NotFound, error.Type);
    }

    [Fact]
    public void GitIgnoreFileTypeWithNameNotFound_IsANotFoundErrorNamingTheType()
    {
        Error error = SupportToolsServerApiClientErrors.GitIgnoreFileTypeWithNameNotFound("CSharp");

        Assert.Equal("GitIgnoreFileTypeWithNameNotFound", error.Code);
        Assert.Equal("GitIgnore File Type With Name CSharp Not Found", error.Description);
        Assert.Equal(ErrorType.NotFound, error.Type);
    }

    [Fact]
    public void GitIgnoreFileTypeIsInUse_IsAConflictListingTheUsages()
    {
        Error error = SupportToolsServerApiClientErrors.GitIgnoreFileTypeIsInUse("CSharp (RepoA, RepoB)");

        Assert.Equal("GitIgnoreFileTypeIsInUse", error.Code);
        Assert.Equal("GitIgnore File Type Is Used By Gits: CSharp (RepoA, RepoB)", error.Description);
        Assert.Equal(ErrorType.Conflict, error.Type);
    }

    [Fact]
    public void GitAddressIsInUse_IsAConflictNamingTheAddressAndTheGits()
    {
        Error error = SupportToolsServerApiClientErrors.GitAddressIsInUse("git@github.com:x/a.git", "RepoA");

        Assert.Equal("GitAddressIsInUse", error.Code);
        Assert.Equal("Git Address git@github.com:x/a.git Is Used By RepoA", error.Description);
        Assert.Equal(ErrorType.Conflict, error.Type);
    }

    [Fact]
    public void EditorConfigFileTypeWithNameNotFound_IsANotFoundErrorNamingTheType()
    {
        Error error = SupportToolsServerApiClientErrors.EditorConfigFileTypeWithNameNotFound("BaGetter");

        Assert.Equal("EditorConfigFileTypeWithNameNotFound", error.Code);
        Assert.Equal("EditorConfig File Type With Name BaGetter Not Found", error.Description);
        Assert.Equal(ErrorType.NotFound, error.Type);
    }

    [Fact]
    public void ValueRequired_IsAProblemNamingTheValue()
    {
        Error error = SupportToolsServerApiClientErrors.ValueRequired("RepoA.GitProjectAddress");

        Assert.Equal("ValueRequired", error.Code);
        Assert.Equal("RepoA.GitProjectAddress Is Required", error.Description);
        Assert.Equal(ErrorType.Problem, error.Type);
    }

    [Fact]
    public void ValueTooLong_IsAProblemNamingTheValueAndTheMaximum()
    {
        Error error = SupportToolsServerApiClientErrors.ValueTooLong("Name", 50);

        Assert.Equal("ValueTooLong", error.Code);
        Assert.Equal("Name Is Longer Than 50 Characters", error.Description);
        Assert.Equal(ErrorType.Problem, error.Type);
    }

    [Fact]
    public void ValuesNotUnique_IsAProblemNamingTheValue()
    {
        Error error = SupportToolsServerApiClientErrors.ValuesNotUnique("GitProjectName");

        Assert.Equal("ValuesNotUnique", error.Code);
        Assert.Equal("GitProjectName Values Are Not Unique", error.Description);
        Assert.Equal(ErrorType.Problem, error.Type);
    }

    [Fact]
    public void InvalidGitFolderName_IsAProblemNamingTheValue()
    {
        Error error = SupportToolsServerApiClientErrors.InvalidGitFolderName("RepoA.GitProjectFolderName");

        Assert.Equal("InvalidGitFolderName", error.Code);
        Assert.Equal("RepoA.GitProjectFolderName Is Not A Valid Relative Folder Path", error.Description);
        Assert.Equal(ErrorType.Problem, error.Type);
    }

    [Fact]
    public void InvalidGitAddress_IsAProblemNamingTheValueAndTheAllowedForms()
    {
        Error error = SupportToolsServerApiClientErrors.InvalidGitAddress("RepoA.GitProjectAddress");

        Assert.Equal("InvalidGitAddress", error.Code);
        Assert.Equal("RepoA.GitProjectAddress Is Not A Valid Git Address (git@host:path, ssh:// Or https://)",
            error.Description);
        Assert.Equal(ErrorType.Problem, error.Type);
    }
}
