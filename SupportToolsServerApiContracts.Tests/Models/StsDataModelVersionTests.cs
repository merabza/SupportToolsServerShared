using System.Linq;
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

    [Fact]
    public void StsSmartSchemaDataModel_ReadsMissingDetailsAsAnEmptyListAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsSmartSchemaDataModel>("""{"Name":"Reduce"}""")!;

        Assert.Equal("Reduce", model.Name);
        Assert.Equal(0, model.LastPreserveCount);
        Assert.Empty(model.Details);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsSmartSchemaDataModel_RoundTripsEveryValueWithTheDetailsInTheirOrder()
    {
        var model = new StsSmartSchemaDataModel
        {
            Name = "Reduce",
            LastPreserveCount = 2,
            Details =
            [
                new StsSmartSchemaDetailDataModel { PeriodType = "Week", PreserveCount = 4 },
                new StsSmartSchemaDetailDataModel { PeriodType = "Day", PreserveCount = 7 }
            ],
            Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsSmartSchemaDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("Reduce", read.Name);
        Assert.Equal(2, read.LastPreserveCount);
        Assert.Equal(["Week", "Day"], read.Details.Select(x => x.PeriodType));
        Assert.Equal([4, 7], read.Details.Select(x => x.PreserveCount));
        Assert.Equal(7, read.Version);
    }

    [Fact]
    public void StsFileStorageDataModel_ReadsTheMissingOptionalValuesAsNullAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsFileStorageDataModel>("""{"Name":"Exchange"}""")!;

        Assert.Equal("Exchange", model.Name);
        Assert.Null(model.FileStoragePath);
        Assert.Null(model.UserName);
        Assert.Null(model.Password);
        Assert.Equal(0, model.FileNameMaxLength);
        Assert.Equal(0, model.FileSizeSplitPositionInRow);
        Assert.Equal(0, model.FtpSiteLsFileOffset);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsFileStorageDataModel_RoundTripsEveryValue()
    {
        var model = new StsFileStorageDataModel
        {
            Name = "Exchange",
            FileStoragePath = "ftp://ftp.example.com/exchange/",
            UserName = "made-up-user",
            Password = "made-up-password",
            FileNameMaxLength = 255,
            FileSizeSplitPositionInRow = 4,
            FtpSiteLsFileOffset = 1,
            Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsFileStorageDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("Exchange", read.Name);
        Assert.Equal("ftp://ftp.example.com/exchange/", read.FileStoragePath);
        Assert.Equal("made-up-user", read.UserName);
        Assert.Equal("made-up-password", read.Password);
        Assert.Equal(255, read.FileNameMaxLength);
        Assert.Equal(4, read.FileSizeSplitPositionInRow);
        Assert.Equal(1, read.FtpSiteLsFileOffset);
        Assert.Equal(7, read.Version);
    }

    [Fact]
    public void StsApiClientDataModel_ReadsTheMissingOptionalValuesAsNullAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsApiClientDataModel>("""{"Name":"Pc1.WebAgent"}""")!;

        Assert.Equal("Pc1.WebAgent", model.Name);
        Assert.Null(model.Server);
        Assert.Null(model.ApiKey);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsApiClientDataModel_RoundTripsEveryValue()
    {
        var model = new StsApiClientDataModel
        {
            Name = "Pc1.WebAgent", Server = "http://localhost:5031/api/v1/", ApiKey = "made-up-key", Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsApiClientDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("Pc1.WebAgent", read.Name);
        Assert.Equal("http://localhost:5031/api/v1/", read.Server);
        Assert.Equal("made-up-key", read.ApiKey);
        Assert.Equal(7, read.Version);
    }

    [Fact]
    public void StsDatabaseServerConnectionDataModel_ReadsTheMissingOptionalValuesAsDefaultsAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsDatabaseServerConnectionDataModel>(
            """{"Name":"Pc1.Sql","DatabaseServerProvider":"SqlServer"}""")!;

        Assert.Equal("Pc1.Sql", model.Name);
        Assert.Equal("SqlServer", model.DatabaseServerProvider);
        Assert.Null(model.DbWebAgentName);
        Assert.Null(model.RemoteDbConnectionName);
        Assert.Null(model.ServerAddress);
        Assert.False(model.WindowsNtIntegratedSecurity);
        Assert.Null(model.ServerUser);
        Assert.Null(model.ServerPass);
        Assert.False(model.TrustServerCertificate);
        Assert.Equal(0, model.ConnectionTimeOut);
        Assert.False(model.Encrypt);
        Assert.Empty(model.DatabaseFoldersSets);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsDatabaseServerConnectionDataModel_RoundTripsEveryValueWithTheFoldersSets()
    {
        var model = new StsDatabaseServerConnectionDataModel
        {
            Name = "Pc1.Sql",
            DatabaseServerProvider = "WebAgent",
            DbWebAgentName = "Pc1.WebAgent",
            RemoteDbConnectionName = "Main",
            ServerAddress = "pc1",
            WindowsNtIntegratedSecurity = true,
            ServerUser = "made-up-user",
            ServerPass = "made-up-password",
            TrustServerCertificate = true,
            ConnectionTimeOut = 30,
            Encrypt = true,
            DatabaseFoldersSets =
            [
                new StsDatabaseFoldersSetDataModel
                {
                    Name = "Default", Backup = @"D:\Bak", Data = @"D:\Data", DataLog = @"D:\Log"
                },
                new StsDatabaseFoldersSetDataModel { Name = "Second" }
            ],
            Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsDatabaseServerConnectionDataModel>(
            JsonConvert.SerializeObject(model))!;

        Assert.Equal("Pc1.Sql", read.Name);
        Assert.Equal("WebAgent", read.DatabaseServerProvider);
        Assert.Equal("Pc1.WebAgent", read.DbWebAgentName);
        Assert.Equal("Main", read.RemoteDbConnectionName);
        Assert.Equal("pc1", read.ServerAddress);
        Assert.True(read.WindowsNtIntegratedSecurity);
        Assert.Equal("made-up-user", read.ServerUser);
        Assert.Equal("made-up-password", read.ServerPass);
        Assert.True(read.TrustServerCertificate);
        Assert.Equal(30, read.ConnectionTimeOut);
        Assert.True(read.Encrypt);
        Assert.Equal(["Default", "Second"], read.DatabaseFoldersSets.Select(x => x.Name));
        Assert.Equal(@"D:\Bak", read.DatabaseFoldersSets[0].Backup);
        Assert.Equal(@"D:\Data", read.DatabaseFoldersSets[0].Data);
        Assert.Equal(@"D:\Log", read.DatabaseFoldersSets[0].DataLog);
        Assert.Null(read.DatabaseFoldersSets[1].Backup);
        Assert.Equal(7, read.Version);
    }
}
