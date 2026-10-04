namespace SupportToolsServerApiContracts.Models;

//React აპლიკაციის შაბლონი, SupportToolsParameters.ReactAppTemplates-ის ჩანაწერი: სახელი და create-react-app-ის
//--template მნიშვნელობა. ჩანაწერები სახელით ემთხვევა, რეგისტრის გარეშე. upsert-ში Version მოსალოდნელი ვერსიაა:
//0 ნიშნავს ახალ ჩანაწერს, N კი განახლებას მხოლოდ მაშინ, თუ სერვერზე შენახული ვერსია N-ია
public sealed class StsReactAppTemplateDataModel
{
    public required string Name { get; set; }
    public required string Template { get; set; }
    public int Version { get; set; }
}