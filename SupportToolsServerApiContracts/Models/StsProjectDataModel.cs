using System.Collections.Generic;

namespace SupportToolsServerApiContracts.Models;

//პროექტი, SupportToolsParameters.Projects-ის ჩანაწერი (კლიენტის ProjectModel), ერთ აგრეგატად (README G7). ProjectType
//კლიენტის EProjectType-ის სახელია. EditorConfigPatternName .editorconfig შაბლონის, GitProjectNames და
//ScaffoldSeederGitProjectNames git-ების, FrontNpmPackageNames კი npm პაკეტების სახელებია; null ან ცარიელი სახელი
//ნიშნავს, რომ მითითება არ არის, არარსებული სახელები კი 404 ReferencedRecordsNotFound-ია. გზები კანონიკური ფორმითაა
//(README G3). KeyGuidPart საიდუმლოა: ღიად გადაიცემა, მაგრამ არსად იბეჭდება. DevDatabaseParameters და
//ProdCopyDatabaseParameters null-ია, თუ პროექტს ისინი არ აქვს. სიები სიმრავლეებია: სერვერი მათ სახელით დალაგებულს
//აბრუნებს (OrdinalIgnoreCase), endpoint-ებსა და route კლასებს Name-ით, ServerInfo-ებს კი ServerName-ითა და
//EnvironmentName-ით, რომ კლიენტის ჰეში რიგზე არ იყოს დამოკიდებული. AllowToolsList კლიენტის EProjectTools-ის სახელებია.
//ჩანაწერები სახელით ემთხვევა, რეგისტრის გარეშე. upsert-ში Version მოსალოდნელი ვერსიაა: 0 ნიშნავს ახალ ჩანაწერს, N კი
//განახლებას მხოლოდ მაშინ, თუ სერვერზე შენახული ვერსია N-ია. განახლება მთელ პროექტს ანაცვლებს, ბაზის პარამეტრების,
//სიებისა და ServerInfo-ების ჩათვლით
public sealed class StsProjectDataModel
{
    public required string Name { get; set; }
    public required string ProjectType { get; set; }
    public string? ProjectGroupName { get; set; }
    public string? ProjectDescription { get; set; }
    public int MajorVersion { get; set; }
    public int MinorVersion { get; set; }
    public bool UseAlternativeWebAgent { get; set; }
    public string? EditorConfigPatternName { get; set; }
    public string? MainProjectName { get; set; }
    public string? ApiContractsProjectName { get; set; }
    public string? SpaProjectName { get; set; }
    public string? DbContextName { get; set; }
    public string? ProjectShortPrefix { get; set; }
    public string? ScaffoldSeederProjectName { get; set; }
    public string? DbContextProjectName { get; set; }
    public string? NewDataSeedingClassLibProjectName { get; set; }
    public string? ProgramArchiveDateMask { get; set; }
    public string? ProgramArchiveExtension { get; set; }
    public string? ParametersFileDateMask { get; set; }
    public string? ParametersFileExtension { get; set; }
    public string? ProjectFolderName { get; set; }
    public string? SolutionFileName { get; set; }
    public string? ProjectSecurityFolderPath { get; set; }
    public string? MigrationStartupProjectFilePath { get; set; }
    public string? MigrationProjectFilePath { get; set; }
    public string? DataSeederRulesByTableStartupProjectFilePath { get; set; }
    public string? OldDataConvertorForDataSeeder { get; set; }
    public string? SeedProjectFilePath { get; set; }
    public string? SeedProjectParametersFilePath { get; set; }
    public string? ExcludesRulesParametersFilePath { get; set; }
    public string? AppSetEnKeysJsonFileName { get; set; }
    public string? MigrationSqlFilesFolder { get; set; }
    public string? PrepareProdCopyDatabaseProjectFilePath { get; set; }
    public string? PrepareProdCopyDatabaseProjectParametersFilePath { get; set; }
    public string? PairedDbObjectsResultFileName { get; set; }
    public string? KeyGuidPart { get; set; }
    public StsDatabaseParametersDataModel? DevDatabaseParameters { get; set; }
    public StsDatabaseParametersDataModel? ProdCopyDatabaseParameters { get; set; }
    public List<string> GitProjectNames { get; set; } = [];
    public List<string> ScaffoldSeederGitProjectNames { get; set; } = [];
    public List<string> FrontNpmPackageNames { get; set; } = [];
    public List<string> RedundantFileNames { get; set; } = [];
    public List<string> AllowToolsList { get; set; } = [];
    public List<StsProjectEndpointDataModel> Endpoints { get; set; } = [];
    public List<StsProjectRouteClassDataModel> RouteClasses { get; set; } = [];
    public List<StsServerInfoDataModel> ServerInfos { get; set; } = [];
    public int Version { get; set; }
}