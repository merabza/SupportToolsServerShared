using Newtonsoft.Json;
using SupportToolsServerApiContracts.Models;
using Xunit;

namespace SupportToolsServerApiContracts.Tests.Models;

//An older client sends no Version, which reads as 0; the existing POST routes of the server ignore it
public sealed class StsDataModelVersionTests
{
    [Fact]
    public void StsGitDataModel_ReadsAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsGitDataModel>(
            """{"GitProjectName":"RepoA","GitProjectAddress":"git@github.com:x/a.git","GitProjectFolderName":"A","GitIgnorePatternName":"CSharp"}""")!;

        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsGitIgnoreFileTypeDataModel_ReadsAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsGitIgnoreFileTypeDataModel>(
            """{"Id":"11111111-1111-1111-1111-111111111111","Name":"CSharp","Content":"bin/"}""")!;

        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsEditorConfigFileTypeDataModel_ReadsAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsEditorConfigFileTypeDataModel>(
            """{"Name":"default","Content":"root = true"}""")!;

        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsEnvironmentDataModel_ReadsAMissingDescriptionAsNullAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsEnvironmentDataModel>("""{"Name":"Dev"}""")!;

        Assert.Equal("Dev", model.Name);
        Assert.Null(model.Description);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsEnvironmentDataModel_RoundTripsTheVersion()
    {
        var model = new StsEnvironmentDataModel { Name = "Prod", Description = "Production", Version = 7 };

        var read = JsonConvert.DeserializeObject<StsEnvironmentDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("Prod", read.Name);
        Assert.Equal("Production", read.Description);
        Assert.Equal(7, read.Version);
    }

    [Fact]
    public void StsRuntimeDataModel_ReadsAMissingDescriptionAsNullAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsRuntimeDataModel>("""{"Name":"win-x64"}""")!;

        Assert.Equal("win-x64", model.Name);
        Assert.Null(model.Description);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsRuntimeDataModel_RoundTripsTheVersion()
    {
        var model = new StsRuntimeDataModel { Name = "win-x64", Description = "Windows x64", Version = 7 };

        var read = JsonConvert.DeserializeObject<StsRuntimeDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("win-x64", read.Name);
        Assert.Equal("Windows x64", read.Description);
        Assert.Equal(7, read.Version);
    }

    [Fact]
    public void StsNpmPackageDataModel_ReadsAMissingDescriptionAsNullAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsNpmPackageDataModel>("""{"Name":"@reduxjs/toolkit"}""")!;

        Assert.Equal("@reduxjs/toolkit", model.Name);
        Assert.Null(model.Description);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsNpmPackageDataModel_RoundTripsTheVersion()
    {
        var model = new StsNpmPackageDataModel { Name = "@reduxjs/toolkit", Description = "Redux", Version = 7 };

        var read = JsonConvert.DeserializeObject<StsNpmPackageDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("@reduxjs/toolkit", read.Name);
        Assert.Equal("Redux", read.Description);
        Assert.Equal(7, read.Version);
    }

    [Fact]
    public void StsReactAppTemplateDataModel_ReadsAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsReactAppTemplateDataModel>(
            """{"Name":"ReduxApp","Template":"redux-typescript"}""")!;

        Assert.Equal("ReduxApp", model.Name);
        Assert.Equal("redux-typescript", model.Template);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsReactAppTemplateDataModel_RoundTripsTheVersion()
    {
        var model = new StsReactAppTemplateDataModel { Name = "ReduxApp", Template = "redux-typescript", Version = 7 };

        var read = JsonConvert.DeserializeObject<StsReactAppTemplateDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("ReduxApp", read.Name);
        Assert.Equal("redux-typescript", read.Template);
        Assert.Equal(7, read.Version);
    }

    [Fact]
    public void StsDotnetToolDataModel_ReadsTheMissingOptionalValuesAsNullAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsDotnetToolDataModel>(
            """{"Name":"DotnetEf","PackageId":"dotnet-ef"}""")!;

        Assert.Equal("DotnetEf", model.Name);
        Assert.Equal("dotnet-ef", model.PackageId);
        Assert.Null(model.MaxVersion);
        Assert.Null(model.Description);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsDotnetToolDataModel_RoundTripsEveryValue()
    {
        var model = new StsDotnetToolDataModel
        {
            Name = "DotnetEf",
            PackageId = "dotnet-ef",
            MaxVersion = "9.0.8",
            Description = "Entity Framework",
            Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsDotnetToolDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("DotnetEf", read.Name);
        Assert.Equal("dotnet-ef", read.PackageId);
        Assert.Equal("9.0.8", read.MaxVersion);
        Assert.Equal("Entity Framework", read.Description);
        Assert.Equal(7, read.Version);
    }
}
