using System;

namespace SupportToolsServerApiContracts.Models;

public sealed class StsGitIgnoreFileTypeDataModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Content { get; set; }

    //ჩანაწერის ვერსია (optimistic concurrency). სერვერი მას GET-ის პასუხში აბრუნებს, ატვირთვები კი არ კითხულობს,
    //ამიტომ ძველი კლიენტის JSON, რომელშიც ის არ არის (0), ისევ მუშაობს
    public int Version { get; set; }
}