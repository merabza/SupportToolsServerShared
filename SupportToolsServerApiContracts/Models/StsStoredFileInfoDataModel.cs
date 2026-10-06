using System;

namespace SupportToolsServerApiContracts.Models;

//საიდუმლო ფაილის მეტამონაცემები შიგთავსის გარეშე (GET files). Path კანონიკური გზაა (README G3) და ჩანაწერის
//გასაღებია, რეგისტრის გარეშე. Sha256 შიგთავსის UTF-8 ბაიტების (BOM-ის გარეშე) SHA-256-ია, hex დიდი ასოებით, Length კი
//იმავე ბაიტების რაოდენობა. ორივეს სერვერი ითვლის, ამიტომ კლიენტს ლოკალური ფაილის შედარება შიგთავსის ჩამოტვირთვის
//გარეშე შეუძლია. UpdatedAtUtc ბოლო შექმნის ან განახლების დროა, UTC
public sealed class StsStoredFileInfoDataModel
{
    public required string Path { get; set; }
    public required string Sha256 { get; set; }
    public int Length { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public int Version { get; set; }
}
