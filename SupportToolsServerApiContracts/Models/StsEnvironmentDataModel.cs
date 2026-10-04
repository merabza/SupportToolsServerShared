namespace SupportToolsServerApiContracts.Models;

//გარემო, SupportToolsParameters.Environments-ის ჩანაწერი: სახელი და აღწერა. ჩანაწერები სახელით ემთხვევა,
//რეგისტრის გარეშე. upsert-ში Version მოსალოდნელი ვერსიაა: 0 ნიშნავს ახალ ჩანაწერს, N კი განახლებას მხოლოდ მაშინ,
//თუ სერვერზე შენახული ვერსია N-ია
public sealed class StsEnvironmentDataModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Version { get; set; }
}