using SupportToolsServerApiContracts.Models;
using System.Collections.Generic;

namespace SupportToolsServerApiContracts.Requests;

public sealed class SyncGitRequest
{
    public required Dictionary<string, GitDataDto> Gits { get; set; }
    public required List<GitIgnoreFileDto> GitIgnoreFiles { get; set; }

}