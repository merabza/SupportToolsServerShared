namespace SupportToolsServerApiContracts.Models;

//საიდუმლო ფაილი შიგთავსით: GET files/content-ის პასუხი და POST files/update-ის ტანი. Path კანონიკური გზაა (README G3):
//Windows-ის აბსოლუტური ფორმა (X:\...), "."/".." სეგმენტებისა და აკრძალული სიმბოლოების გარეშე. Content ტექსტია:
//შეიძლება ცარიელი იყოს, მაგრამ არა null, და UTF-8-ში ContentMaxBytes-ს არ უნდა აღემატებოდეს. შიგთავსი საიდუმლოა: ღიად
//გადაიცემა (README G2), მაგრამ არსად იბეჭდება. upsert-ში Version მოსალოდნელი ვერსიაა: 0 ნიშნავს ახალ ფაილს, N კი
//განახლებას მხოლოდ მაშინ, თუ სერვერზე შენახული ვერსია N-ია
public sealed class StsStoredFileDataModel
{
    //შიგთავსის ზღვარი UTF-8 ბაიტებში (1 MiB). უფრო დიდ ფაილს სერვერი არ იღებს, ამიტომ კლიენტი მას არ ტვირთავს
    public const int ContentMaxBytes = 1024 * 1024;

    public required string Path { get; set; }
    public required string Content { get; set; }
    public int Version { get; set; }
}