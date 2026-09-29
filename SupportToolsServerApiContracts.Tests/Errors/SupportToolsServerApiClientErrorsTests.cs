using SupportToolsServerApiContracts.Errors;
using SystemTools.SharedKernel;
using Xunit;

namespace SupportToolsServerApiContracts.Tests.Errors;

public sealed class SupportToolsServerApiClientErrorsTests
{
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
}
