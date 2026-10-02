namespace SupportToolsServerApiContracts.Models;

//.editorconfig შაბლონი. სერვერი ჩანაწერებს სახელით ადარებს, ამიტომ Id კლიენტს არ სჭირდება
public sealed class StsEditorConfigFileTypeDataModel
{
    public required string Name { get; set; }
    public required string Content { get; set; }

    //ჩანაწერის ვერსია (optimistic concurrency). სერვერი მას GET-ის პასუხში აბრუნებს, ატვირთვა კი არ კითხულობს,
    //ამიტომ ძველი კლიენტის JSON, რომელშიც ის არ არის (0), ისევ მუშაობს
    public int Version { get; set; }
}