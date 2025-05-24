namespace SupportToolsServerApiContracts.Models;

public sealed class GitIgnoreFileDto
{
    public required string Name { get; set; }
    public required string Content { get; set; }
}