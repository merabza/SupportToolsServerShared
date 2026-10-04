namespace SupportToolsServerApiContracts.Models;

//dotnet-ის ხელსაწყო, SupportToolsParameters.DotnetTools-ის ჩანაწერის საერთო ნაწილი: სახელი (dictionary-ის key),
//NuGet-ის პაკეტის Id, მაქსიმალური ვერსია (ცარიელი ნიშნავს ბოლო ვერსიას) და აღწერა. InstalledVersion, LatestVersion და
//CommandName კომპიუტერისაა და აქ არ არის. ჩანაწერები სახელით ემთხვევა, რეგისტრის გარეშე. upsert-ში Version
//მოსალოდნელი ვერსიაა: 0 ნიშნავს ახალ ჩანაწერს, N კი განახლებას მხოლოდ მაშინ, თუ სერვერზე შენახული ვერსია N-ია
public sealed class StsDotnetToolDataModel
{
    public required string Name { get; set; }
    public required string PackageId { get; set; }
    public string? MaxVersion { get; set; }
    public string? Description { get; set; }
    public int Version { get; set; }
}