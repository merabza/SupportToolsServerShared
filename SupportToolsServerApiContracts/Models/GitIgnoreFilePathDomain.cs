namespace SupportToolsServerApiContracts.Models;

public sealed class GitIgnoreFilePathDomain
{
    public required string GitIgnorePath { get; set; }
    public required string GitIgnorePathName { get; set; }
}