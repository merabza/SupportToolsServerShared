namespace SupportToolsServerApiContracts.Models;

public sealed class StsGitDataModel
{
    public required string GitProjectName { get; set; }
    public required string GitProjectAddress { get; set; }
    public required string GitProjectFolderName { get; set; }
    public required string GitIgnorePatternName { get; set; }

    //ჩანაწერის ვერსია (optimistic concurrency). სერვერი მას GET-ის პასუხში აბრუნებს, updategitrepo და uploadgitrepos
    //კი არ კითხულობს, ამიტომ ძველი კლიენტის JSON, რომელშიც ის არ არის (0), ისევ მუშაობს
    public int Version { get; set; }
}