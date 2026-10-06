namespace SupportToolsServerApiContracts.Models;

//API კლიენტი, SupportToolsParameters.ApiClients-ის ჩანაწერი: სახელი (dictionary-ის key), სერვერის მისამართი და API key.
//გასაღები საიდუმლოა: ღიად გადაიცემა, მაგრამ არსად იბეჭდება. ჩანაწერები სახელით ემთხვევა, რეგისტრის გარეშე. upsert-ში
//Version მოსალოდნელი ვერსიაა: 0 ნიშნავს ახალ ჩანაწერს, N კი განახლებას მხოლოდ მაშინ, თუ სერვერზე შენახული ვერსია N-ია
public sealed class StsApiClientDataModel
{
    public required string Name { get; set; }
    public string? Server { get; set; }
    public string? ApiKey { get; set; }
    public int Version { get; set; }
}