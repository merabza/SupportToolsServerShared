namespace SupportToolsServerApiContracts.Models;

//პროექტის შემქმნელის პარამეტრები: კლიენტის AppProjectCreatorAllParameters, Templates-ის გარეშე (შაბლონები
//StsProjectTemplateDataModel-ებია). ProjectsFolderPathReal და SecretsFolderPathReal კანონიკური გზებია: სერვერი მათ
//ინახავს ისე, როგორც მოვიდა. ProductionServerName სერვერის, ProductionEnvironmentName გარემოს,
//DeveloperDbConnectionName ბაზის კავშირის, DatabaseExchangeFileStorageName ფაილსაცავის, UseSmartSchema კი ჭკვიანი
//სქემის სახელია; null ან ცარიელი ნიშნავს, რომ მითითება არ არის, არარსებული სახელები კი 404
//ReferencedRecordsNotFound-ია.
//ჩანაწერი ერთადერთია (singleton), ამიტომ გასაღები არ აქვს. სანამ ის შეიქმნება, GET ცარიელ კონტრაქტს აბრუნებს
//Version = 0-ით. upsert-ში Version მოსალოდნელი ვერსიაა: 0 ნიშნავს პირველ შექმნას, N კი განახლებას მხოლოდ მაშინ, თუ
//სერვერზე შენახული ვერსია N-ია. განახლება მთელ ჩანაწერს ანაცვლებს
public sealed class StsProjectCreatorSettingsDataModel
{
    public int IndentSize { get; set; }
    public string? FakeHostProjectName { get; set; }
    public string? ProjectsFolderPathReal { get; set; }
    public string? SecretsFolderPathReal { get; set; }
    public string? ProductionServerName { get; set; }
    public string? ProductionEnvironmentName { get; set; }
    public string? DeveloperDbConnectionName { get; set; }
    public string? DatabaseExchangeFileStorageName { get; set; }
    public string? UseSmartSchema { get; set; }
    public int Version { get; set; }
}