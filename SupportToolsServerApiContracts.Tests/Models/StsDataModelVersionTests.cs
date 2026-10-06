using System;
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

    //IsLocal of the client's ServerDataModel belongs to the machine: the contract has no such value and ignores it
    [Fact]
    public void StsServerDataModel_ReadsTheMissingOptionalValuesAsNullAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsServerDataModel>("""{"Name":"dl360","IsLocal":true}""")!;

        Assert.Equal("dl360", model.Name);
        Assert.Null(model.WebAgentName);
        Assert.Null(model.WebAgentInstallerName);
        Assert.Null(model.FilesUserName);
        Assert.Null(model.FilesUsersGroupName);
        Assert.Null(model.Runtime);
        Assert.Null(model.ServerSideDownloadFolder);
        Assert.Null(model.ServerSideDeployFolder);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsServerDataModel_RoundTripsEveryValue()
    {
        var model = new StsServerDataModel
        {
            Name = "dl360",
            WebAgentName = "Dl360.WebAgent",
            WebAgentInstallerName = "Dl360.Installer",
            FilesUserName = "deployer",
            FilesUsersGroupName = "deployers",
            Runtime = "linux-x64",
            ServerSideDownloadFolder = "/home/deployer/Download",
            ServerSideDeployFolder = "/opt/apps",
            Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsServerDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("dl360", read.Name);
        Assert.Equal("Dl360.WebAgent", read.WebAgentName);
        Assert.Equal("Dl360.Installer", read.WebAgentInstallerName);
        Assert.Equal("deployer", read.FilesUserName);
        Assert.Equal("deployers", read.FilesUsersGroupName);
        Assert.Equal("linux-x64", read.Runtime);
        Assert.Equal("/home/deployer/Download", read.ServerSideDownloadFolder);
        Assert.Equal("/opt/apps", read.ServerSideDeployFolder);
        Assert.Equal(7, read.Version);
    }

    //The empty contract that the server returns before the singleton is created. LocalPath of the client's
    //DatabasesBackupFilesExchangeParameters belongs to the machine: the contract has no such value and ignores it
    [Fact]
    public void StsGlobalSettingsDataModel_ReadsTheMissingValuesAsEmptyAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsGlobalSettingsDataModel>(
            """{"DatabasesBackupFilesExchange":{"LocalPath":"D:\\Backups"}}""")!;
        var empty = JsonConvert.DeserializeObject<StsGlobalSettingsDataModel>("{}")!;

        Assert.Null(model.ServiceDescriptionSignature);
        Assert.Null(model.MediatRLicenseKey);
        Assert.Null(model.FileStorageNameForExchange);
        Assert.Null(model.LocalPackageManagerWebApiClientName);
        Assert.Null(model.DatabasesBackupFilesExchange.DownloadTempExtension);
        Assert.Null(model.DatabasesBackupFilesExchange.ExchangeFileStorageName);
        Assert.Equal(0, model.Version);
        Assert.NotNull(empty.DatabasesBackupFilesExchange);
        Assert.Null(empty.DatabasesBackupFilesExchange.LocalSmartSchemaName);
        Assert.Equal(0, empty.Version);
    }

    [Fact]
    public void StsGlobalSettingsDataModel_RoundTripsEveryValue()
    {
        var model = new StsGlobalSettingsDataModel
        {
            ServiceDescriptionSignature = "ltgmz",
            UploadTempExtension = ".up!",
            ProgramArchiveDateMask = "yyyyMMddHHmmss",
            ProgramArchiveExtension = ".zip",
            ParametersFileDateMask = "yyyyMMdd",
            ParametersFileExtension = ".json",
            MediatRLicenseKey = "made-up-license-key",
            FileStorageNameForExchange = "Exchange",
            SmartSchemaNameForExchange = "Reduce",
            SmartSchemaNameForLocal = "Keep",
            LocalPackageManagerWebApiClientName = "packages.example.com",
            DatabasesBackupFilesExchange = new StsDatabasesBackupFilesExchangeDataModel
            {
                DownloadTempExtension = ".down!",
                UploadTempExtension = ".up!",
                ExchangeFileStorageName = "Backups",
                ExchangeSmartSchemaName = "Reduce",
                LocalSmartSchemaName = "Keep"
            },
            Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsGlobalSettingsDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("ltgmz", read.ServiceDescriptionSignature);
        Assert.Equal(".up!", read.UploadTempExtension);
        Assert.Equal("yyyyMMddHHmmss", read.ProgramArchiveDateMask);
        Assert.Equal(".zip", read.ProgramArchiveExtension);
        Assert.Equal("yyyyMMdd", read.ParametersFileDateMask);
        Assert.Equal(".json", read.ParametersFileExtension);
        Assert.Equal("made-up-license-key", read.MediatRLicenseKey);
        Assert.Equal("Exchange", read.FileStorageNameForExchange);
        Assert.Equal("Reduce", read.SmartSchemaNameForExchange);
        Assert.Equal("Keep", read.SmartSchemaNameForLocal);
        Assert.Equal("packages.example.com", read.LocalPackageManagerWebApiClientName);
        Assert.Equal(".down!", read.DatabasesBackupFilesExchange.DownloadTempExtension);
        Assert.Equal(".up!", read.DatabasesBackupFilesExchange.UploadTempExtension);
        Assert.Equal("Backups", read.DatabasesBackupFilesExchange.ExchangeFileStorageName);
        Assert.Equal("Reduce", read.DatabasesBackupFilesExchange.ExchangeSmartSchemaName);
        Assert.Equal("Keep", read.DatabasesBackupFilesExchange.LocalSmartSchemaName);
        Assert.Equal(7, read.Version);
    }

    //Templates of the client's AppProjectCreatorAllParameters are separate records: the contract has no such value
    [Fact]
    public void StsProjectCreatorSettingsDataModel_ReadsTheMissingValuesAsDefaultsAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsProjectCreatorSettingsDataModel>(
            """{"Templates":{"Console":{"SupportProjectType":0}}}""")!;

        Assert.Equal(0, model.IndentSize);
        Assert.Null(model.FakeHostProjectName);
        Assert.Null(model.ProjectsFolderPathReal);
        Assert.Null(model.SecretsFolderPathReal);
        Assert.Null(model.ProductionServerName);
        Assert.Null(model.ProductionEnvironmentName);
        Assert.Null(model.DeveloperDbConnectionName);
        Assert.Null(model.DatabaseExchangeFileStorageName);
        Assert.Null(model.UseSmartSchema);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsProjectCreatorSettingsDataModel_RoundTripsEveryValue()
    {
        var model = new StsProjectCreatorSettingsDataModel
        {
            IndentSize = 4,
            FakeHostProjectName = "FakeHost",
            ProjectsFolderPathReal = @"D:\1WorkDotnet",
            SecretsFolderPathReal = @"D:\1WorkSecurity",
            ProductionServerName = "dl360",
            ProductionEnvironmentName = "Prod",
            DeveloperDbConnectionName = "Pazisi",
            DatabaseExchangeFileStorageName = "Backups",
            UseSmartSchema = "Reduce",
            Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsProjectCreatorSettingsDataModel>(
            JsonConvert.SerializeObject(model))!;

        Assert.Equal(4, read.IndentSize);
        Assert.Equal("FakeHost", read.FakeHostProjectName);
        Assert.Equal(@"D:\1WorkDotnet", read.ProjectsFolderPathReal);
        Assert.Equal(@"D:\1WorkSecurity", read.SecretsFolderPathReal);
        Assert.Equal("dl360", read.ProductionServerName);
        Assert.Equal("Prod", read.ProductionEnvironmentName);
        Assert.Equal("Pazisi", read.DeveloperDbConnectionName);
        Assert.Equal("Backups", read.DatabaseExchangeFileStorageName);
        Assert.Equal("Reduce", read.UseSmartSchema);
        Assert.Equal(7, read.Version);
    }

    [Fact]
    public void StsProjectTemplateDataModel_ReadsTheMissingOptionalValuesAsDefaultsAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsProjectTemplateDataModel>(
            """{"Name":"Console","SupportProjectType":"Console"}""")!;

        Assert.Equal("Console", model.Name);
        Assert.Equal("Console", model.SupportProjectType);
        Assert.Null(model.TestProjectName);
        Assert.Null(model.TestProjectShortName);
        Assert.False(model.UseDatabase);
        Assert.False(model.UseFluentValidation);
        Assert.Null(model.ReactTemplateName);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsProjectTemplateDataModel_RoundTripsEveryValue()
    {
        var model = new StsProjectTemplateDataModel
        {
            Name = "Reactredux",
            SupportProjectType = "Api",
            TestProjectName = "ReactTest",
            TestProjectShortName = "Rt",
            UseDatabase = true,
            UseDbPartFolderForDatabaseProjects = true,
            UseMenu = true,
            UseHttps = true,
            UseReact = true,
            UseCarcass = true,
            UseIdentity = true,
            UseReCounter = true,
            UseSignalR = true,
            UseFluentValidation = true,
            ReactTemplateName = "redux-typescript",
            Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsProjectTemplateDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal("Reactredux", read.Name);
        Assert.Equal("Api", read.SupportProjectType);
        Assert.Equal("ReactTest", read.TestProjectName);
        Assert.Equal("Rt", read.TestProjectShortName);
        Assert.True(read.UseDatabase);
        Assert.True(read.UseDbPartFolderForDatabaseProjects);
        Assert.True(read.UseMenu);
        Assert.True(read.UseHttps);
        Assert.True(read.UseReact);
        Assert.True(read.UseCarcass);
        Assert.True(read.UseIdentity);
        Assert.True(read.UseReCounter);
        Assert.True(read.UseSignalR);
        Assert.True(read.UseFluentValidation);
        Assert.Equal("redux-typescript", read.ReactTemplateName);
        Assert.Equal(7, read.Version);
    }

    [Fact]
    public void StsProjectDataModel_ReadsTheMissingValuesAsDefaultsAndAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsProjectDataModel>(
            """{"Name":"AppA","ProjectType":"Standard"}""")!;

        Assert.Equal("AppA", model.Name);
        Assert.Equal("Standard", model.ProjectType);
        Assert.Null(model.ProjectGroupName);
        Assert.Equal(0, model.MajorVersion);
        Assert.False(model.UseAlternativeWebAgent);
        Assert.Null(model.EditorConfigPatternName);
        Assert.Null(model.MainProjectName);
        Assert.Null(model.ProgramArchiveDateMask);
        Assert.Null(model.SolutionFileName);
        Assert.Null(model.KeyGuidPart);
        Assert.Null(model.DevDatabaseParameters);
        Assert.Null(model.ProdCopyDatabaseParameters);
        Assert.Empty(model.GitProjectNames);
        Assert.Empty(model.ScaffoldSeederGitProjectNames);
        Assert.Empty(model.FrontNpmPackageNames);
        Assert.Empty(model.RedundantFileNames);
        Assert.Empty(model.AllowToolsList);
        Assert.Empty(model.Endpoints);
        Assert.Empty(model.RouteClasses);
        Assert.Empty(model.ServerInfos);
        Assert.Equal(0, model.Version);
    }

    //A server info has no version of its own: it is part of the project's version
    [Fact]
    public void StsServerInfoDataModel_ReadsTheMissingValuesAsDefaults()
    {
        var model = JsonConvert.DeserializeObject<StsServerInfoDataModel>(
            """{"ServerName":"PAZISI","EnvironmentName":"Prod"}""")!;

        Assert.Equal("PAZISI", model.ServerName);
        Assert.Equal("Prod", model.EnvironmentName);
        Assert.Null(model.WebAgentNameForCheck);
        Assert.Equal(0, model.ServerSidePort);
        Assert.Null(model.ApiVersionId);
        Assert.Null(model.AppSettingsJsonSourceFileName);
        Assert.Null(model.AppSettingsEncodedJsonFileName);
        Assert.Null(model.ServiceUserName);
        Assert.Empty(model.AllowToolsList);
        Assert.Null(model.CurrentDatabaseParameters);
        Assert.Null(model.NewDatabaseParameters);
        Assert.Null(typeof(StsServerInfoDataModel).GetProperty("Version"));
    }

    [Fact]
    public void StsProjectDataModel_RoundTripsEveryValueWithTheDatabaseParametersAndTheListsInTheirOrder()
    {
        var model = new StsProjectDataModel
        {
            Name = "AppA",
            ProjectType = "IsService",
            ProjectGroupName = "Apps",
            ProjectDescription = "Application A",
            MajorVersion = 2,
            MinorVersion = 5,
            UseAlternativeWebAgent = true,
            EditorConfigPatternName = "default",
            MainProjectName = "AppA",
            ApiContractsProjectName = "AppAApiContracts",
            SpaProjectName = "appa.client",
            DbContextName = "AppADbContext",
            ProjectShortPrefix = "aa",
            ScaffoldSeederProjectName = "AppASeeder",
            DbContextProjectName = "AppADb",
            NewDataSeedingClassLibProjectName = "AppANewDataSeeding",
            ProgramArchiveDateMask = "yyyyMMddHHmmss",
            ProgramArchiveExtension = ".zip",
            ParametersFileDateMask = "yyyyMMdd",
            ParametersFileExtension = ".json",
            ProjectFolderName = @"D:\1WorkDotnet\AppA",
            SolutionFileName = @"D:\1WorkDotnet\AppA\AppA.slnx",
            ProjectSecurityFolderPath = @"D:\1WorkSecurity\AppA",
            MigrationStartupProjectFilePath = @"D:\1WorkDotnet\AppA\Startup.csproj",
            MigrationProjectFilePath = @"D:\1WorkDotnet\AppA\Migration.csproj",
            DataSeederRulesByTableStartupProjectFilePath = @"D:\1WorkDotnet\AppA\Rules.csproj",
            OldDataConvertorForDataSeeder = @"D:\1WorkDotnet\AppA\Convertor.csproj",
            SeedProjectFilePath = @"D:\1WorkDotnet\AppA\Seed.csproj",
            SeedProjectParametersFilePath = @"D:\1WorkSecurity\AppA\Seed.json",
            ExcludesRulesParametersFilePath = @"D:\1WorkSecurity\AppA\Excludes.json",
            AppSetEnKeysJsonFileName = @"D:\1WorkSecurity\AppA\Keys.json",
            MigrationSqlFilesFolder = @"D:\1WorkDotnet\AppA\Sql",
            PrepareProdCopyDatabaseProjectFilePath = @"D:\1WorkDotnet\AppA\Prepare.csproj",
            PrepareProdCopyDatabaseProjectParametersFilePath = @"D:\1WorkSecurity\AppA\Prepare.json",
            PairedDbObjectsResultFileName = @"D:\1WorkSecurity\AppA\Paired.json",
            KeyGuidPart = "made-up-key-guid-part",
            DevDatabaseParameters = new StsDatabaseParametersDataModel
            {
                DbConnectionName = "Pc1.Sql",
                DatabaseRecoveryModel = "Simple",
                DbServerFoldersSetName = "Default",
                DatabaseName = "AppADev",
                SmartSchemaName = "Reduce",
                FileStorageName = "Backups",
                CommandTimeOut = 120,
                SkipBackupBeforeRestore = true,
                BackupNamePrefix = "dev",
                DateMask = "yyyyMMdd",
                BackupFileExtension = ".bak",
                BackupNameMiddlePart = "_FullDb_",
                Compress = true,
                Verify = false,
                BackupType = "Full"
            },
            GitProjectNames = ["AppA", "AppAShared"],
            ScaffoldSeederGitProjectNames = ["AppADbPart"],
            FrontNpmPackageNames = ["react-redux", "@reduxjs/toolkit"],
            RedundantFileNames = ["*.pdb", "web.config"],
            AllowToolsList = ["SeedData", "GenerateApiRoutes"],
            Endpoints =
            [
                new StsProjectEndpointDataModel
                {
                    Name = "Upload",
                    EndpointName = "UploadGitRepos",
                    EndpointRoute = "/uploadgitrepos",
                    RequireAuthorization = true,
                    HttpMethod = "Post",
                    EndpointType = "Command",
                    ReturnType = "int",
                    SendMessageToCurrentUser = true
                }
            ],
            RouteClasses =
            [
                new StsProjectRouteClassDataModel { Name = "Git", Root = "api", ApiVersion = "v1", Base = "/git" }
            ],
            ServerInfos =
            [
                new StsServerInfoDataModel
                {
                    ServerName = "PAZISI",
                    EnvironmentName = "Prod",
                    WebAgentNameForCheck = "PAZISI.WebAgent",
                    ServerSidePort = 5022,
                    ApiVersionId = "v1",
                    AppSettingsJsonSourceFileName = @"D:\1WorkSecurity\AppA\PAZISI\appsettings.json",
                    AppSettingsEncodedJsonFileName = @"D:\1WorkSecurity\AppA\PAZISI\appsettingsEncoded.json",
                    ServiceUserName = "merab",
                    AllowToolsList = ["ProgramUpdater", "AppSettingsEncoder"],
                    NewDatabaseParameters = new StsDatabaseParametersDataModel
                    {
                        DbConnectionName = "PAZISI.Sql", DatabaseName = "AppA", CommandTimeOut = 300
                    }
                },
                new StsServerInfoDataModel { ServerName = "dl360", EnvironmentName = "Test" }
            ],
            Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsProjectDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal(JsonConvert.SerializeObject(model), JsonConvert.SerializeObject(read));
        Assert.Equal("AppA", read.Name);
        Assert.Equal("IsService", read.ProjectType);
        Assert.Equal(@"D:\1WorkDotnet\AppA\AppA.slnx", read.SolutionFileName);
        Assert.Equal("made-up-key-guid-part", read.KeyGuidPart);
        Assert.Equal("Pc1.Sql", read.DevDatabaseParameters!.DbConnectionName);
        Assert.Equal(120, read.DevDatabaseParameters.CommandTimeOut);
        Assert.False(read.DevDatabaseParameters.Verify);
        Assert.Null(read.ProdCopyDatabaseParameters);
        Assert.Equal(["react-redux", "@reduxjs/toolkit"], read.FrontNpmPackageNames);
        Assert.Equal("Post", Assert.Single(read.Endpoints).HttpMethod);
        Assert.Equal("v1", Assert.Single(read.RouteClasses).ApiVersion);
        Assert.Equal(["PAZISI|Prod", "dl360|Test"],
            read.ServerInfos.Select(x => $"{x.ServerName}|{x.EnvironmentName}"));
        StsServerInfoDataModel serverInfo = read.ServerInfos[0];
        Assert.Equal("PAZISI.WebAgent", serverInfo.WebAgentNameForCheck);
        Assert.Equal(5022, serverInfo.ServerSidePort);
        Assert.Equal(["ProgramUpdater", "AppSettingsEncoder"], serverInfo.AllowToolsList);
        Assert.Null(serverInfo.CurrentDatabaseParameters);
        Assert.Equal(300, serverInfo.NewDatabaseParameters!.CommandTimeOut);
        Assert.Equal(7, read.Version);
    }

    //An upload without Version creates the file
    [Fact]
    public void StsStoredFileDataModel_ReadsAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsStoredFileDataModel>(
            """{"Path":"D:\\1WorkSecurity\\AppA\\appsettings.json","Content":"{}"}""")!;

        Assert.Equal(@"D:\1WorkSecurity\AppA\appsettings.json", model.Path);
        Assert.Equal("{}", model.Content);
        Assert.Equal(0, model.Version);
    }

    [Fact]
    public void StsStoredFileDataModel_RoundTripsThePathAndTheContentUnchanged()
    {
        var model = new StsStoredFileDataModel
        {
            Path = @"D:\1WorkSecurity\ა b+c\appsettings.json",
            Content = "{\r\n  \"Key\": \"made-up\",\t\"Text\": \"ა\\\"\"\r\n}\r\n",
            Version = 7
        };

        var read = JsonConvert.DeserializeObject<StsStoredFileDataModel>(JsonConvert.SerializeObject(model))!;

        Assert.Equal(model.Path, read.Path);
        Assert.Equal(model.Content, read.Content);
        Assert.Equal(7, read.Version);
    }

    [Fact]
    public void StsStoredFileDataModel_LimitsTheContentToOneMebibyte()
    {
        Assert.Equal(1048576, StsStoredFileDataModel.ContentMaxBytes);
    }

    //The server writes the time of the last change in UTC, with Z (System.Text.Json, camel case)
    [Fact]
    public void StsStoredFileInfoDataModel_ReadsTheValuesOfTheServerWithTheTimeInUtc()
    {
        var model = JsonConvert.DeserializeObject<StsStoredFileInfoDataModel>(
            """{"path":"D:\\1WorkSecurity\\a.json","sha256":"BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD","length":3,"updatedAtUtc":"2026-10-06T08:15:30.123Z","version":2}""")!;

        Assert.Equal(@"D:\1WorkSecurity\a.json", model.Path);
        Assert.Equal("BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD", model.Sha256);
        Assert.Equal(3, model.Length);
        Assert.Equal(new DateTime(2026, 10, 6, 8, 15, 30, 123, DateTimeKind.Utc), model.UpdatedAtUtc);
        Assert.Equal(DateTimeKind.Utc, model.UpdatedAtUtc.Kind);
        Assert.Equal(2, model.Version);
    }

    [Fact]
    public void StsStoredFileInfoDataModel_ReadsAMissingVersionAsZero()
    {
        var model = JsonConvert.DeserializeObject<StsStoredFileInfoDataModel>(
            """{"Path":"D:\\1WorkSecurity\\a.json","Sha256":"E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855"}""")!;

        Assert.Equal(0, model.Length);
        Assert.Equal(default, model.UpdatedAtUtc);
        Assert.Equal(0, model.Version);
    }
}
