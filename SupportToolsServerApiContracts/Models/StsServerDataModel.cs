namespace SupportToolsServerApiContracts.Models;

//სერვერი, SupportToolsParameters.Servers-ის ჩანაწერი, IsLocal-ის გარეშე: ის კომპიუტერზეა დამოკიდებული და იქვე
//გამოითვლება. WebAgentName და WebAgentInstallerName ვებაგენტების ApiClient-ების სახელებია, Runtime კი Runtime-ის
//სახელი; null ან ცარიელი ნიშნავს, რომ მითითება არ არის, არარსებული სახელები კი 404 ReferencedRecordsNotFound-ია.
//ServerSideDownloadFolder და ServerSideDeployFolder სამიზნე სერვერის გზებია და გარდაქმნის გარეშე გადაიცემა.
//ჩანაწერები სახელით ემთხვევა, რეგისტრის გარეშე. upsert-ში Version მოსალოდნელი ვერსიაა: 0 ნიშნავს ახალ ჩანაწერს, N კი
//განახლებას მხოლოდ მაშინ, თუ სერვერზე შენახული ვერსია N-ია. სახელი AppSettings-ის დაშიფვრის გასაღების ნაწილია, ამიტომ
//სერვერის გადარქმევა წაშლა და ახლის შექმნაა
public sealed class StsServerDataModel
{
    public required string Name { get; set; }
    public string? WebAgentName { get; set; }
    public string? WebAgentInstallerName { get; set; }
    public string? FilesUserName { get; set; }
    public string? FilesUsersGroupName { get; set; }
    public string? Runtime { get; set; }
    public string? ServerSideDownloadFolder { get; set; }
    public string? ServerSideDeployFolder { get; set; }
    public int Version { get; set; }
}
