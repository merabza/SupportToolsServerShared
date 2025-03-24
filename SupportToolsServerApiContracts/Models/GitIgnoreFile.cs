namespace SupportToolsServerApiContracts.Models;

public sealed class GitIgnoreFile
{
    public required string Name { get; set; }
    public required string Content { get; set; }
}