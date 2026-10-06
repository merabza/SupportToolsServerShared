namespace SupportToolsServerApiContracts.Models;

//პროექტის შაბლონი, კლიენტის AppProjectCreatorAllParameters.Templates-ის ჩანაწერი (TemplateModel). SupportProjectType
//კლიენტის ESupportProjectType-ის სახელია (Console, Razor, Api, ScaffoldSeeder). ReactTemplateName React-ის შაბლონის
//(ReactAppTemplate) სახელია; null ან ცარიელი ნიშნავს, რომ შაბლონი არ არის, არარსებული სახელი კი 404
//ReferencedRecordsNotFound-ია. ჩანაწერები სახელით ემთხვევა, რეგისტრის გარეშე. upsert-ში Version მოსალოდნელი ვერსიაა:
//0 ნიშნავს ახალ ჩანაწერს, N კი განახლებას მხოლოდ მაშინ, თუ სერვერზე შენახული ვერსია N-ია
public sealed class StsProjectTemplateDataModel
{
    public required string Name { get; set; }
    public required string SupportProjectType { get; set; }
    public string? TestProjectName { get; set; }
    public string? TestProjectShortName { get; set; }
    public bool UseDatabase { get; set; }
    public bool UseDbPartFolderForDatabaseProjects { get; set; }
    public bool UseMenu { get; set; }
    public bool UseHttps { get; set; }
    public bool UseReact { get; set; }
    public bool UseCarcass { get; set; }
    public bool UseIdentity { get; set; }
    public bool UseReCounter { get; set; }
    public bool UseSignalR { get; set; }
    public bool UseFluentValidation { get; set; }
    public string? ReactTemplateName { get; set; }
    public int Version { get; set; }
}