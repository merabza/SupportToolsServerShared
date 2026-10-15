namespace SupportToolsServerApiContracts.Models;

//ფაილსაცავი, SupportToolsParameters.FileStorages-ის ჩანაწერი: სახელი (dictionary-ის key), გზა (URL ან ლოკალური გზა,
//სერვერი მას ინახავს ისე, როგორც მოვიდა), მომხმარებელი, პაროლი და FTP-ის ფაილების სიის წაკითხვის პარამეტრები.
//პაროლი საიდუმლოა: ღიად გადაიცემა, მაგრამ არსად იბეჭდება. ჩანაწერები სახელით ემთხვევა, რეგისტრის გარეშე. upsert-ში
//Version მოსალოდნელი ვერსიაა: 0 ნიშნავს ახალ ჩანაწერს, N კი განახლებას მხოლოდ მაშინ, თუ სერვერზე შენახული ვერსია N-ია
public sealed class StsFileStorageDataModel
{
    public required string Name { get; set; }
    public string? FileStoragePath { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public int FileNameMaxLength { get; set; }
    public int FileSizeSplitPositionInRow { get; set; }
    public int FtpSiteLsFileOffset { get; set; }
    public int Version { get; set; }
}